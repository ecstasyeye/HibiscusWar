//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{
	/// <summary>
	/// 章データ
	/// </summary>
	[System.Serializable]
	public class AdvChapterData
	{
		//章の名前
		public string ChapterName { get { return chapterName; } }
		[SerializeField]
		string chapterName = "";

		//データのリスト
		List<AdvImportBook> DataList { get { return dataList; } }
		[SerializeField]
		List<AdvImportBook> dataList = new List<AdvImportBook>();

		List<StringGrid> settingList = null;
		public List<StringGrid> SettingList { get { return settingList; } }

		List<StringGrid> scenarioList = null;
		public List<StringGrid> ScenarioList { get { return scenarioList; } }

		List<StringGrid> runtimeGridList = null;
		public List<StringGrid> RuntimeGridList { get { return runtimeGridList ?? (runtimeGridList = new List<StringGrid>()); } }

		List<AdvScenarioData> scenarioDataList = null;
		public List<AdvScenarioData> ScenarioDataList { get { return scenarioDataList ?? (scenarioDataList = new List<AdvScenarioData>()); } }

		public bool IsInited { get; set; }

		public AdvChapterData(string name)
		{
			this.chapterName = name;
		}

		public void BootInit()
		{
			IsInited = true;
			settingList = new List<StringGrid>();
			scenarioList = new List<StringGrid>();
			foreach (AdvImportBook item in DataList)
			{
				foreach (var grid in item.GridList)
				{
					InitData(grid);
				}
			}
			foreach (var grid in RuntimeGridList)
			{
				InitData(grid);
			}			
		}

		void InitData(StringGrid grid)
		{
			grid.InitLink();
			string sheetName = grid.SheetName;
			if (AdvSettingDataManager.IsDisabelSheetName(sheetName))
			{
				Debug.LogError(sheetName + " is invalid name");
				return;
			}
			if (AdvSettingDataManager.IsSettingsSheet(grid.SheetName))
			{
				SettingList.Add(grid);
			}
			else
			{
				ScenarioList.Add(grid);
			}
		}

		//****************************　TSVのロード用　****************************//
		/// <summary>
		/// TSVをロード
		/// </summary>
		/// <param name="url">ファイルパス</param>
		/// <param name="version">シナリオバージョン（-1以下で必ずサーバーからデータを読み直す）</param>
		/// <returns></returns>
		internal IEnumerator CoLoadFromTsv(string url, int version)
		{
			//起動ファイルの読み込み
			AssetFile bootFile = AssetFileManager.Load(url, version, this);
//			Debug.Log("Load Chapter : " + url + " :Ver " + bootFile.Version);
			Debug.Log("Load Chapter : " + ChapterName + " :Ver " + bootFile.Version);
			while (!bootFile.IsLoadEnd) yield return 0;

			string rootDir = FilePathUtil.GetDirectoryPath(url);
			//設定ファイルの読み込み
			List<AssetFile> settingFileList = new List<AssetFile>();
			{
				StringGrid grid = bootFile.Csv;
				foreach (StringGridRow row in grid.Rows)
				{
					if (row.RowIndex < grid.DataTopRow) continue;			//データの行じゃない
					if (row.IsEmptyOrCommantOut) continue;					//データがない
					string path = AdvParser.ParseCell<string>(row, AdvColumnName.Param1);
					int ver = AdvParser.ParseCell<int>(row, AdvColumnName.Version);
					settingFileList.Add(AssetFileManager.Load(FilePathUtil.Combine(rootDir, path), ver, this));
				}
			}

			//設定ファイルの読み込み
			List<AssetFile> scenarioFileList = new List<AssetFile>();
			foreach (var item in settingFileList)
			{
				while (!item.IsLoadEnd) yield return 0;
				if (!item.IsLoadError)
				{
					StringGrid grid = item.Csv;
					if (grid.SheetName != AdvSettingDataManager.SheetNameScenario)
					{
						this.RuntimeGridList.Add(grid);
					}
					else
					{
						foreach (StringGridRow row in grid.Rows)
						{
							if (row.RowIndex < grid.DataTopRow) continue;			//データの行じゃない
							if (row.IsEmptyOrCommantOut) continue;					//データがない
							string path = AdvParser.ParseCell<string>(row, AdvColumnName.FileName);
							int ver = AdvParser.ParseCellOptional<int>(row, AdvColumnName.Version, 0);

							//旧形式（ﾌｧｲﾙ分割なし）に対応
							if (!path.Contains("/"))
							{
								path = "Scenario/" + path;
							}
							path += ".tsv";
							scenarioFileList.Add(AssetFileManager.Load(FilePathUtil.Combine(rootDir, path), ver, this));
						}
					}
				}
				item.Unuse(this);
			}

			foreach (var item in scenarioFileList)
			{
				while (!item.IsLoadEnd) yield return 0;
				if (!item.IsLoadError)
				{
					this.RuntimeGridList.Add(item.Csv);
				}
				item.Unuse(this);
			}

			bootFile.Unuse(this);
//			Debug.Log("Load End Chapter : " + url + " :Ver " + bootFile.Version);
			Debug.Log("Load End Chapter : " + ChapterName + " :Ver " + bootFile.Version);
		}


/*
		/// <summary>
		/// CSVからデータ初期化
		/// </summary>
		/// <param name="row">初期化するためのデータ</param>
		/// <returns>成否</returns>
		internal void InitFromCsv(StringGrid grid, string url)
		{
			this.csvDir = url.Replace(FilePathUtil.GetFileName(url), "");
			scenarioDirInfo = new DefaultDirInfo { defaultDir = "", defaultExt = ".tsv" };
			InitDefaultDirInfo(csvDir, scenarioDirInfo);
			this.Grid = grid;
		}

		internal List<AssetFilePathInfo> SettingUrlList(StringGrid grid)
		{
			List<AssetFilePathInfo> list = new List<AssetFilePathInfo>();
			foreach (StringGridRow row in grid.Rows)
			{
				if (row.RowIndex < grid.DataTopRow) continue;			//データの行じゃない
				if (row.IsEmptyOrCommantOut) continue;					//データがない
				string path = AdvParser.ParseCell<string>(row, AdvColumnName.Param1);
				int version = AdvParser.ParseCell<int>(row, AdvColumnName.Version);
				list.Add(new AssetFilePathInfo(FileNameToPath(path, csvDir), version));
			}
			return list;
		}
/*
		bool CheckChapter(StringGridRow row)
		{
			//章の指定がない場合は読み込む
			if (string.IsNullOrEmpty(chapterName)) return true;
			string chapter = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Chapter, "");
			if (string.IsNullOrEmpty(chapter)) return true;

			//章の指定がある場合は、その章のみ
			return chapterName == chapter;
		}
*/
		internal void InitImportData(List<AdvImportBook> importDataList)
		{
			this.DataList.Clear();
			this.DataList.AddRange(importDataList);
		}
	}
}