//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace Utage
{

	/// <summary>
	/// シナリオのページデータ
	/// </summary>
	public class AdvScenarioPageData
	{
		//コマンドのリスト
		public List<AdvCommand> CommnadList { get { return commnadList; } }
		List<AdvCommand> commnadList = new List<AdvCommand>();

		//ページ内のテキストデータのリスト
		//自動改行処理などのために、ページ内のテキストをあらかじめ全部知る必要がある
		public List<AdvScenarioTextDataInPage> TextDataList { get; private set; }

		/// <summary>
		/// シナリオラベル
		/// </summary>
		public AdvScenarioLabelData ScenarioLabelData { get; private set; }

		/// <summary>
		/// ページ番号
		/// </summary>
		public int PageNo { get; private set; }

		//メッセージウィンドウ名
		public string MessageWindowName { get; set; }


		public AdvScenarioPageData( AdvScenarioLabelData scenarioLabelData, int pageNo )
		{
			TextDataList = new List<AdvScenarioTextDataInPage>();
			ScenarioLabelData = scenarioLabelData;
			PageNo = pageNo;
		}

		//コマンドの追加
		public void AddCommand(AdvCommand command)
		{
			commnadList.Add(command);
		}

		//初期化
		public void Init()
		{
			foreach (AdvCommand command in commnadList)
			{
				command.InitFromPageData(this);
			}
		}

		//データのダウンロード
		public void Download(AdvDataManager dataManager)
		{
			commnadList.ForEach((item) => item.Download(dataManager));
		}

		//指定インデックスのコマンドを取得
		public AdvCommand GetCommand(int index)
		{
			return (index < commnadList.Count) ? commnadList[index] : null;
		}
		
		//ファイルセットを追加
		public void AddToFileSet( HashSet<AssetFile> fileSet)
		{
			foreach( AdvCommand command in commnadList )
			{
				if (command.IsExistLoadFile())
				{
					command.LoadFileList.ForEach((item) => fileSet.Add(item));
				}
			}
		}

		internal AdvScenarioTextDataInPage AddTextDataInPage(AdvCommand command)
		{
			AdvScenarioTextDataInPage textData = new AdvScenarioTextDataInPage(command);
			TextDataList.Add(textData);
			return textData;
		}


		internal void InitMessageWindowName(AdvCommand command, string messageWindowName)
		{
			if (string.IsNullOrEmpty(messageWindowName)) return;

			if (string.IsNullOrEmpty(MessageWindowName) )
			{
				MessageWindowName = messageWindowName;
			}
			else if (MessageWindowName != messageWindowName)
			{
				Debug.LogError(command.ToErrorString(messageWindowName + ": WindowName already set is this page"));
			}
		}

		//テキストデータを作成
		//自動改行処理などのために、ページ内のテキストをあらかじめ全部知る必要がある
		public string MakeText()
		{
			StringBuilder builder = new StringBuilder();
			foreach (var item in TextDataList)
			{
				if (item.IsEmptyText) continue;
				builder.Append(item.Command.ParseCellLocalizedText());
				if (item.IsNextBr) builder.Append("\n");
			}
			return builder.ToString();
		}

		//指定のデータまでのテキストを取得
		public string MakeText(AdvScenarioTextDataInPage currentTextDataInPage)
		{
			StringBuilder builder = new StringBuilder();
			foreach (var item in TextDataList)
			{
				if (item.IsEmptyText) continue;
				builder.Append(item.Command.ParseCellLocalizedText());
				if (item.IsNextBr) builder.Append("\n");
				if (item == currentTextDataInPage)
				{
					return builder.ToString();
				}
			}
			return "";
		}

		public bool IsEmptyText
		{ 
			get
			{
				foreach (var item in TextDataList)
				{
					if( !item.IsEmptyText ) return false;
				}
				return true;
			}
		}

#if UNITY_EDITOR

		// 文字数オーバー　チェック
		internal int EditorCheckCharacterCount(AdvEngine engine, ref string currentWindowName, Dictionary<string, AdvUguiMessageWindow> windows)
		{
			AdvUguiMessageWindow messageWindow;
			if (!string.IsNullOrEmpty(MessageWindowName)) currentWindowName = MessageWindowName;

			if (!windows.TryGetValue(currentWindowName, out messageWindow))
			{
				foreach (var window in windows.Values)
				{
					messageWindow = window;
					break;
				}
			}
			bool isActive = messageWindow.gameObject.activeSelf;
			if (!isActive)
			{
				messageWindow.gameObject.SetActive(true);
			}
			UguiLocalizeBase[] localizeArray = messageWindow.GetComponentsInChildren<UguiLocalizeBase>();
			foreach( var item in localizeArray )
			{
				item.EditorRefresh();
			}			

			UguiNovelText textGUI = messageWindow.Text;
			string oldText = textGUI.text;
			string text = MakeText();
			string errorString;			
			int len;
			if (!textGUI.TextGenerator.EditorCheckRect(text, out len, out errorString) )
			{
				Debug.LogError("TextOver:" + TextDataList[0].Command.RowData.ToStringOfFileSheetLine() + "\n" + errorString);
			}
			textGUI.text = oldText;
			foreach (var item in localizeArray)
			{
				item.ResetDefault();
			}
			messageWindow.gameObject.SetActive(isActive);
			return len;
		}
#endif

		//ロード直後のときなどのために、Ifスキップ
		internal int GetIfSkipCommandIndex(int index)
		{
			for (int i = index; i < commnadList.Count; ++i)
			{
				AdvCommand command = commnadList[i];
				//AdvCommandIfで始まっていない場合は、AdvCommandEndIfまでスキップする
				if (command.IsIfCommand)
				{
					if (command.GetType() == typeof(AdvCommandIf))
					{
						return index;
					}
					else
					{
						for (int j = index + 1; j < commnadList.Count; ++j)
						{
							if (commnadList[j].GetType() == typeof(AdvCommandEndIf))
							{
								return j;
							}
						}
					}
				}
			}
			return index;
		}

	}
}