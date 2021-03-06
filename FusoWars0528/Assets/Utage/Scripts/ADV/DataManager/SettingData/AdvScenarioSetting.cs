#if false
//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Utage
{

	/// <summary>
	/// シナリオの設定データ
	/// </summary>
	public class AdvScenarioSettingData : AdvSettingDataDictinoayItemBase
	{
		/// <summary>シナリオファイル</summary>
		public string ScenaioFile { get { return this.Key; } }

		/// <summary>シナリオシート名</summary>
		public string ScenaioSheetName { get; protected set; }

		/// <summary>バージョン</summary>
		public int Version { get { return this.version; } }
		[SerializeField]
		int version;

		// TODO:
		/// <summary>回想モードがオープンされているか？工事中</summary>
		public bool IsGalleryOpen { get { return this.isGalleryOpen; } set { this.isGalleryOpen = value; } }
		bool isGalleryOpen;

		public StringGridRow RowData { get; protected set; }


		/// <summary>
		/// StringGridの一行からデータ初期化
		/// </summary>
		/// <param name="row">初期化するためのデータ</param>
		/// <returns>成否</returns>
		public override bool InitFromStringGridRow(StringGridRow row)
		{
			this.RowData = row;
			string key = AdvParser.ParseCell<string>(row,AdvColumnName.FileName);
			InitKey(key);
			this.version = AdvParser.ParseCellOptional<int>(row, AdvColumnName.Version, 0);
			this.ScenaioSheetName = FilePathUtil.GetFileNameWithoutExtension(key);
			return true;
		}
	}

	/// <summary>
	/// シナリオの設定データ
	/// </summary>
	public class AdvScenarioSetting : AdvSettingDataDictinoayBase<AdvScenarioSettingData>
	{
		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="settingData">設定データ</param>
		public override void BootInit(AdvBootSetting settingData)
		{
			foreach (AdvScenarioSettingData data in List)
			{
				AssetFile file = AssetFileManager.GetFileCreateIfMissing(ScenaioFileToPath(data.ScenaioFile));
				file.Version = data.Version;
			}
		}

		/// <summary>
		/// ファイル名をパスに
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>ファイルパス</returns>
		public string ScenaioFileToPath(string scenaioFile)
		{
			//既に絶対URLならそのまま
			if (FilePathUtil.IsAbsoluteUri(scenaioFile))
			{
				return scenaioFile;
			}
			else
			{
				//拡張子がなければデフォルト拡張子を追加
				if (string.IsNullOrEmpty(FilePathUtil.GetExtension(scenaioFile)))
				{
					scenaioFile += defaultExt;
				}

				//旧形式（ﾌｧｲﾙ分割なし）に対応
				if (!scenaioFile.Contains("/"))
				{
					scenaioFile = "Scenario/" + scenaioFile;
				}
				return FilePathUtil.Combine(defaultDir, scenaioFile);
			}
		}

//#if UNITY_EDITOR

		/// <summary>
		/// エクセルからCSVファイルにコンバートする際に、シナリオ設定データをマージして作成する
		/// </summary>
		/// <param name="grid">シナリオ設定データ</param>
		/// <param name="scenarioSheetDictionary">シナリオデータ</param>
		/// <returns>マージしたシナリオ設定データ</returns>
		public static StringGrid MargeScenarioData(StringGrid grid, StringGridDictionary scenarioSheetDictionary, int version )
		{
			if (grid == null)
			{
				grid = new StringGrid(AdvSettingDataManager.SheetNameScenario, AdvSettingDataManager.SheetNameScenario,CsvType.Tsv);
				grid.AddRow(new List<string> { AdvParser.Localize(AdvColumnName.FileName), AdvParser.Localize(AdvColumnName.Version) });
				grid.ParseHeader();
			}

			List<string> addScnenarioList = new List<string>();
			foreach (string sheetName in scenarioSheetDictionary.Keys)
			{
				bool isFind = false;
				foreach (StringGridRow row in grid.Rows)
				{
					if (AdvParser.ParseCell<string>(row,AdvColumnName.FileName) == sheetName)
					{
						isFind = true;
					}
				}
				if (!isFind)
				{
					addScnenarioList.Add(sheetName);
				}
			}
			foreach (string sheetName in addScnenarioList)
			{
				grid.AddRow(new List<string> { sheetName, ""+version });
			}
			return grid;
		}
//#endif

		/// <summary>
		/// 全てのシナリオがロード済みか
		/// </summary>
		public bool IsLoadEndAllScenario { get; private set; }

		internal IEnumerator CoLoadAndiInit(System.Action<string, AssetFile> OnCompleteLoadScenario, System.Action OnCompleteLoadScenarioAll)
		{
			IsLoadEndAllScenario = false;
			//TSVシナリオファイルをロード
			foreach (AdvScenarioSettingData scenarioSetting in List)
			{
				string sheetName = scenarioSetting.ScenaioSheetName;
				string path = ScenaioFileToPath(scenarioSetting.ScenaioFile);
				AssetFile file = AssetFileManager.BackGroundLoad(path, this);
				while (!file.IsLoadEnd) yield return 0;
				OnCompleteLoadScenario(sheetName, file);
				file.Unuse(this);
			}
			OnCompleteLoadScenarioAll();
			IsLoadEndAllScenario = true;
		}
	}
}
#endif
