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
	/// シナリオラベルで区切られたデータ
	/// </summary>
	public class AdvScenarioLabelData
	{
		//ページデータ
		public List<AdvScenarioPageData> PageDataList{ get{ return pageDataList; } }
		List<AdvScenarioPageData> pageDataList = new List<AdvScenarioPageData>();

		//シナリオラベル
		string scenaioLabel;
		public string ScenaioLabel
		{
			get { return scenaioLabel; }
		}
		public int PageNum
		{
			get { return pageDataList.Count; }
		}

		//セーブポイントが設定されているか
		public bool IsSavePoint
		{
			get
			{
				return (scenarioLabelCommand == null) ? false : scenarioLabelCommand.Type == AdvCommandScenarioLabel.ScenarioLabelType.SavePoint;
			}
		}

		//セーブのタイトルが設定されているか
		public string SaveTitle
		{
			get
			{
				return (scenarioLabelCommand == null) ? "" : scenarioLabelCommand.Title;
			}
		}

		//コマンドのリスト
		public List<AdvCommand> CommnadList { get { return commnadList; } }
		List<AdvCommand> commnadList = new List<AdvCommand>();

		AdvCommandScenarioLabel scenarioLabelCommand;

		//コンストラクタ
		internal AdvScenarioLabelData(string scenaioLabel, AdvCommandScenarioLabel scenarioLabelCommand)
		{
			this.scenaioLabel = scenaioLabel;
			this.scenarioLabelCommand = scenarioLabelCommand;
		}

		//コマンドの追加
		internal void AddCommand(AdvCommand command)
		{
			this.CommnadList.Add(command);
		}

		//ページ情報の初期化
		internal void InitPages()
		{
			if (CommnadList.Count <= 0) return;

			this.pageDataList.Clear();

			{
				AdvScenarioPageData page = new AdvScenarioPageData(this, this.PageDataList.Count);
				pageDataList.Add(page);
				for (int i = 0; i < CommnadList.Count; ++i)
				{
					AdvCommand command = CommnadList[i];
					page.AddCommand(command);
					//ページデータの作成（ページ末端判定にも使うのでここで行う）
					command.MakePageData(page);
					//ページが最後かチェック
					if (command.IsTypePageEnd() && i + 1 < CommnadList.Count)
					{
						page = new AdvScenarioPageData(this, this.PageDataList.Count);
						pageDataList.Add(page);
					}
				}
			}

			foreach (AdvScenarioPageData page in pageDataList)
			{
				page.Init();
			}
		}


		//データのダウンロード
		public void Download(AdvDataManager dataManager)
		{
			pageDataList.ForEach( (item)=>item.Download(dataManager) );
		}

		//ページデータの取得
		public AdvScenarioPageData GetPageData(int page)
		{
			return (page < pageDataList.Count) ? pageDataList[page] : null;
		}

		//エラー文字列
		public string ToErrorString(string str, string gridName)
		{
			if (scenarioLabelCommand!=null)
			{
				return scenarioLabelCommand.RowData.ToErrorString(str);
			}
			else
			{
				return str + " "+ gridName;
			}
		}

		//サブルーチンコマンドのシナリオラベル内のインデックスを取得
		internal int CountSubroutineCommandIndex(AdvCommand command)
		{
			int index = 0;
			foreach (AdvScenarioPageData page in pageDataList)
			{
				foreach (AdvCommand cmd in page.CommnadList)
				{
					System.Type type = cmd.GetType();
					if (type == typeof(AdvCommandJumpSubroutine) || type == typeof(AdvCommandJumpSubroutineRandom))
					{
						if (cmd == command)
						{
							return index;
						}
						else
						{
							++index;
						}
					}
				}
			}
			Debug.LogError("Not found Subroutine Command");
			return -1;
		}

		//サブルーチンの帰り先を見つけて情報を設定
		internal void SetSubroutineRetunInfo(int subroutineCommandIndex, SubRoutineInfo info)
		{
			info.ReturnLabel = ScenaioLabel;

			AdvCommand calledCommand=null;
			int index = 0;
			foreach(AdvScenarioPageData page in PageDataList)
			{
				foreach (AdvCommand cmd in page.CommnadList)
				{
					//呼び出し元のコマンドを探す
					System.Type type = cmd.GetType();
					if (calledCommand == null)
					{
						if (type == typeof(AdvCommandJumpSubroutine) || type == typeof(AdvCommandJumpSubroutineRandom))
						{
							if (index == subroutineCommandIndex)
							{
								calledCommand = cmd;
							}
							else
							{
								++index;
							}
						}
					}
					else
					{
						//呼び出しもとは見つかってるので、飛び先のコマンドを見つける
						if (calledCommand.GetType() == typeof(AdvCommandJumpSubroutine))
						{
							//呼び出し元のコマンドの次のコマンド
							info.ReturnPageNo = page.PageNo;
							info.ReturnCommand = cmd;
							return;
						}
						if (calledCommand.GetType() == typeof(AdvCommandJumpSubroutineRandom))
						{
							if (type != typeof(AdvCommandJumpSubroutineRandom) && type != typeof(AdvCommandJumpSubroutineRandom))
							{
								//ランダムサブルーチンが終わったところ
								info.ReturnPageNo = page.PageNo;
								info.ReturnCommand = cmd;
								return;
							}
						}
					}
				}
			}
			Debug.LogError("Not found Subroutine Return Command");
		}



		//ファイルのプリロードを終わらせるべきか
		public bool IsEndPreLoad()
		{
			if(CommnadList.Count<=0) return false;

			//シナリオ分岐系のコマンドだったら、プリロードは終了
			AdvCommand lastCommand = CommnadList[CommnadList.Count-1];
			if( lastCommand is AdvCommandEndScenario  ) return true;
			if( lastCommand is AdvCommandSelectionEnd ) return true;
			if( lastCommand is AdvCommandSelectionClickEnd ) return true;
			if( lastCommand is AdvCommandJumpRandomEnd ) return true;

			//自動分岐は条件式を考慮する
			if( (lastCommand is AdvCommandJump) || 
				(lastCommand is AdvCommandJumpSubroutine) ||
				(lastCommand is AdvCommandJumpSubroutineRandom )
				)
			{
				if( lastCommand.IsEmptyCell( AdvColumnName.Arg2 ) )
				{
					return true;
				}
			}
			return false;
		}


#if UNITY_EDITOR
		// 文字数オーバー　チェック
		internal int EditorCheckCharacterCount(AdvEngine engine, Dictionary<string, AdvUguiMessageWindow> windows)
		{
			int count = 0;
			string currentWindowName = "";
			foreach (AdvScenarioPageData page in pageDataList)
			{
				count += page.EditorCheckCharacterCount(engine, ref currentWindowName, windows);
			}
			return count;
		}

		//エディタ上のビュワーで表示するラベル
		public string FileLabel
		{
			get
			{
				foreach (AdvScenarioPageData page in pageDataList)
				{
					foreach (AdvCommand command in page.CommnadList)
					{
						if (command.RowData != null && command.RowData.Grid != null)
						{
							string name = command.RowData.Grid.Name;
							int index = name.LastIndexOf("/");
							return name.Substring(index,name.Length-index);
						}
					}
				}
				return "Unknown";
			}
		}
#endif
	}
}