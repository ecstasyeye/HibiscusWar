//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utage
{

	/// <summary>
	/// シナリオのデータ
	/// </summary>
	public class AdvScenarioData
	{
		/// <summary>
		/// シナリオ名
		/// </summary>
		string Name { get { return this.name; } }
		string name;

		//グリッドデータ
		public StringGrid DataGrid{ get; private set; }

		/// <summary>
		/// データグリッド名
		/// </summary>
		public string DataGridName { get { return DataGrid.Name; } }

		/// <summary>
		/// 初期化済みか
		/// </summary>
		public bool IsInit { get { return this.isInit; } }
		bool isInit = false;

		/// <summary>
		/// バックグランドでのロード処理を既にしているか
		/// </summary>
		public bool IsAlreadyBackGroundLoad { get { return this.isAlreadyBackGroundLoad; } }
		bool isAlreadyBackGroundLoad = false;
	
		/// <summary>
		/// このシナリオからリンクするジャンプ先のシナリオラベル
		/// </summary>
		public List<AdvScenarioJumpData> JumpScenarioData { get { return this.jumpScenarioData; } }
		List<AdvScenarioJumpData> jumpScenarioData = new List<AdvScenarioJumpData>();

		/// <summary>
		/// このシナリオ内のページデータ
		/// </summary>
		public List<AdvScenarioLabelData> ScenarioLabelData { get { return this.scenarioLabelData; } }
		List<AdvScenarioLabelData> scenarioLabelData = new List<AdvScenarioLabelData>();


		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="name">シナリオ名</param>
		/// <param name="grid">シナリオデータ</param>
		public AdvScenarioData(string name, StringGrid grid)
		{
			this.name = name;
			this.DataGrid = grid;
		}

		//シナリオデータとして解析
		public void Init(AdvSettingDataManager dataManager, AdvMacroManager macroManger)
		{
			isInit = false;
			List<AdvCommand> commandList = new List<AdvCommand>();
			foreach (StringGridRow row in DataGrid.Rows)
			{
				if (row.RowIndex < DataGrid.DataTopRow) continue;			//データの行じゃない
				if (row.IsEmptyOrCommantOut) continue;								//データがない

				List<AdvCommand> macroCommnadList;
				AdvCommand.StartCheckEntity( dataManager.DefaultParam.GetParameter );
				bool isMacro = macroManger.TryParseMacro(row, dataManager, out macroCommnadList);
				AdvCommand.EndCheckEntity();
				if (isMacro)
				{
					//マクロの場合
					commandList.AddRange(macroCommnadList);
				}
				else
				{
					//通常コマンド
					AdvCommand command = AdvCommandParser.CreateCommand(row, dataManager);
					if (null != command) commandList.Add(command);
				}
			}
			//選択肢終了などの特別なコマンドを自動解析して追加
			AddExtraCommand(commandList, dataManager);
			//シナリオラベルデータを作成
			MakeScanerioLabelData(commandList);
			isInit = true;
		}

		/// <summary>
		/// 選択肢終了などの特別なコマンドを自動解析して追加
		/// </summary>
		/// <param name="continuousCommand">連続しているコマンド</param>
		/// <param name="currentCommand">現在のコマンド</param>
		void AddExtraCommand(List<AdvCommand> commandList, AdvSettingDataManager dataManager)
		{
			int index = 0;
			while (index < commandList.Count)
			{
				AdvCommand current = commandList[index];
				AdvCommand next = index +1 < commandList.Count ? commandList[index+1] : null;
				++index;
				string[] idArray = current.GetExtraCommandIdArray(next);
				if (idArray!=null)
				{
					foreach(string id in idArray)
					{
						AdvCommand extraCommand = AdvCommandParser.CreateCommand(id,null,dataManager);
						commandList.Insert(index, extraCommand);
						++index;
					}
				}
			}
		}


		//シナリオラベル区切りのデータを作成
		void MakeScanerioLabelData(List<AdvCommand> commandList)
		{
			if (commandList.Count <= 0) return;

			//最初のラベル区切りデータは自身の名前（シート名）
			AdvScenarioLabelData data = new AdvScenarioLabelData(Name, null);
			int commandIndex = 0;
			while (true)
			{
				//重複してないかチェック
				if (IsContainsScenarioLabel(data.ScenaioLabel))
				{
					Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.RedefinitionScenarioLabel, data.ScenaioLabel, DataGridName));
					break;
				}
				scenarioLabelData.Add(data);

				//コマンドを追加
				while (commandIndex < commandList.Count)
				{
					AdvCommand command = commandList[commandIndex];
					//シナリオラベルがあるので、終了
					if (!string.IsNullOrEmpty(command.GetScenarioLabel()))
					{
						break;
					}
					data.AddCommand(command);
					///このシナリオからリンクするジャンプ先のシナリオラベルを取得
					string[] jumpLabels = command.GetJumpLabels();
					if (jumpLabels != null)
					{
						foreach (var jumpLabel in jumpLabels)
						{
							jumpScenarioData.Add(new AdvScenarioJumpData(jumpLabel, command.RowData));
						}
					}
					++commandIndex;
				}
				//ページデータの初期化処理
				data.InitPages();
				if (commandIndex >= commandList.Count) break;
				data = new AdvScenarioLabelData(commandList[commandIndex].GetScenarioLabel(), commandList[commandIndex] as AdvCommandScenarioLabel);
				++commandIndex;
			}
		}

#if UNITY_EDITOR
		// 文字数オーバー　チェック
		public int EditorCheckCharacterCount(AdvEngine engine, Dictionary<string, AdvUguiMessageWindow> windows)
		{
			int count = 0;
			foreach (AdvScenarioLabelData scenarioLabel in ScenarioLabelData)
			{
				count += scenarioLabel.EditorCheckCharacterCount(engine,windows);
			}
			return count;
		}
#endif

		/// <summary>
		/// バックグランドでダウンロードだけする
		/// </summary>
		/// <param name="dataManager">各種設定データ</param>
		public void Download(AdvDataManager dataManager)
		{
			ScenarioLabelData.ForEach( (item)=>item.Download(dataManager) );
			isAlreadyBackGroundLoad = true;
		}

		/// <summary>
		/// 指定のシナリオラベルがあるかチェック
		/// </summary>
		/// <param name="scenarioLabel">シナリオラベル</param>
		/// <returns>あったらtrue。なかったらfalse</returns>
		public bool IsContainsScenarioLabel(string scenarioLabel)
		{
			return FindScenarioLabelData(scenarioLabel) != null;
		}

		/// <summary>
		/// 指定のシナリオラベルがあるかチェック
		/// </summary>
		/// <param name="scenarioLabel">シナリオラベル</param>
		/// <returns>あったらtrue。なかったらfalse</returns>
		public AdvScenarioLabelData FindScenarioLabelData(string scenarioLabel)
		{
			return ScenarioLabelData.Find((item) => item.ScenaioLabel == scenarioLabel);
		}

		public AdvScenarioLabelData FindNextScenarioLabelData(string scenarioLabel)
		{
			for (int i = 0; i < ScenarioLabelData.Count-1; ++i)
			{
				if (ScenarioLabelData[i].ScenaioLabel == scenarioLabel)
				{
					return ScenarioLabelData[i + 1];
				}
			}
			return null;
		}

		public HashSet<AssetFile> MakePreloadFileList(string scenarioLabel, int page, int maxFilePreload)
		{
			for (int i = 0; i < ScenarioLabelData.Count; ++i)
			{
				if (ScenarioLabelData[i].ScenaioLabel == scenarioLabel)
				{
					return MakePreloadFileListSub(i, page, maxFilePreload);
				}
			}
			return null;
		}

		HashSet<AssetFile> MakePreloadFileListSub( int index, int page, int maxFilePreload)
		{
			HashSet<AssetFile> fileList = new HashSet<AssetFile>();
			for (int i = index; i < ScenarioLabelData.Count; ++i)
			{
				AdvScenarioLabelData data = ScenarioLabelData[i];
				for (int j = page; j < data.PageNum; ++j)
				{
					data.GetPageData(j).AddToFileSet(fileList);
					if (fileList.Count >= maxFilePreload)
					{
						return fileList;
					}
				}
				if (data.IsEndPreLoad())
				{
					break;
				}
				page = 0;
			}
			return fileList;
		}
	}
}