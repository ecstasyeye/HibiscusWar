//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions; 
using System;

using UnityEngine;

namespace Utage
{

	/// <summary>
	/// 各コマンドの基底クラス
	/// </summary>
	public abstract class AdvCommand
	{
		/// <summary>
		/// エンティティチェックを開始
		/// </summary>
		public static void StartCheckEntity(Func<string, object> callbackGetValueInCheckEntity)
		{
			CheckEntity = true;
			CallbackGetValueInCheckEntity = callbackGetValueInCheckEntity;
		}
		/// <summary>
		/// エンティティチェックを終了
		/// </summary>
		public static void EndCheckEntity()
		{
			CheckEntity = false;
			CallbackGetValueInCheckEntity = null;
		}

		/// <summary>
		/// エンティティチェックをするか
		/// </summary>
		static bool CheckEntity { get; set; }
		//文字列をキーにして値を返すコールバック（変数処理のため）
		static System.Func<string, object> CallbackGetValueInCheckEntity;

		/// <summary>
		/// エディタ上のエラーチェックのために起動してるか
		/// </summary>
		static public bool IsEditorErrorCheck
		{
			get { return isEditorErrorCheck; }
			set { isEditorErrorCheck = value; }
		}
		static bool isEditorErrorCheck = false;
		
		protected AdvCommand(StringGridRow row)
		{
			this.OriginalRowData = this.RowData = row;
			if (CheckEntity)
			{
				StringGridRow rowData;
				if (TryParseRowDataEntity(row, CallbackGetValueInCheckEntity, out rowData))
				{
					IsEntityType = true;
					this.RowData = rowData;
				}
			}
		}

		//データ
		public StringGridRow RowData { get; set; }

		//完全にオリジナルのデータ（エンティティ処理前）
		public StringGridRow OriginalRowData { get; set; }

		//エンティティが設定されている
		public bool IsEntityType { get; protected set; }

		//ロードの必要があるファイルリスト
		public List<AssetFile> LoadFileList { get { return loadFileList; } }
		List<AssetFile> loadFileList = null;

		///このシナリオからリンクするジャンプ先のシナリオラベル
		public virtual string[] GetJumpLabels() { return null; }
		///このシナリオに設定されているシナリオラベル
		public virtual string GetScenarioLabel() { return null; }

		//ロードの必要があるファイルがあるか
		public bool IsExistLoadFile()
		{
			if (null != loadFileList)
			{
				return loadFileList.Count > 0;
			}
			return false;
		}

		//ロードの必要があるファイルを追加
		public AssetFile AddLoadFile(string path, StringGridRow settingData = null )
		{
			if (IsEntityType) return null;
			return AddLoadFile(AssetFileManager.GetFileCreateIfMissing(path, settingData));
		}

/*
		//ロードの必要があるファイルを追加
		public AssetFile AddLoadFile(string path)
		{
			if (IsEntityType) return null;
			return AddLoadFile(AssetFileManager.GetFileCreateIfMissing(path));
		}
*/
		//ロードの必要があるファイルを追加
		AssetFile AddLoadFile(AssetFile file)
		{
			if (loadFileList == null) loadFileList = new List<AssetFile>();
			if (file == null)
			{
				if (!IsEditorErrorCheck)
				{
					Debug.LogError("file is not found");
				}
			}
			else
			{
				loadFileList.Add(file);
			}
			return file;
		}

		//ロードの必要があるファイルを追加
		public void AddLoadGraphic(GraphicInfoList graphic )
		{
			if(graphic==null) return;
			if (IsEntityType) return;

			foreach( var item in graphic.InfoList )
			{
				AddLoadFile(item.File);
			}
		}

		//DL処理する
		public void Download(AdvDataManager dataManager)
		{
			if (null != loadFileList)
			{
				foreach (AssetFile file in loadFileList)
				{
					AssetFileManager.Download(file);
				}
			}
		}

		//ロード処理
		public void Load()
		{
			if (null != loadFileList)
			{
				foreach (AssetFile file in loadFileList)
				{
					AssetFileManager.Load(file, this);
				}
			}
		}

		//ロードが終わったか
		public bool IsLoadEnd()
		{
			if (null != loadFileList)
			{
				foreach (AssetFile file in loadFileList)
				{
					if (!file.IsLoadEnd)
					{
						return false;
					}
				}
			}
			return true;
		}

		//コマンド実行
		public abstract void DoCommand(AdvEngine engine);

		//コマンド実行後に使ったファイル参照をクリア
		public void Unload()
		{
			if (null != loadFileList)
			{
				foreach (AssetFile file in loadFileList)
				{
					file.Unuse(this);
				}
			}
		}


		//コマンド終了待ち
		public virtual bool Wait(AdvEngine engine) { return false; }

		//ページ区切りのコマンドか
		public virtual bool IsTypePageEnd() { return false; }

		//IF文タイプのコマンドか
		public virtual bool IsIfCommand { get { return false; } }


		protected AdvScenarioTextDataInPage TextDataInPage { get; set; }
		//ページ用のデータを作成
		public virtual void MakePageData(AdvScenarioPageData pageData) { }
		protected virtual void InitTextDataInPage(AdvScenarioTextDataInPage textDataInPage)
		{
			TextDataInPage = textDataInPage;
		}

		//ページ用のデータからコマンドに必要な情報を初期化
		public virtual void InitFromPageData(AdvScenarioPageData pageData){}

		// 選択肢終了などの特別なコマンドを自動生成する場合、そのIDを返す
		public virtual string[] GetExtraCommandIdArray( AdvCommand next ) { return null; }

		/// <summary>
		/// エラー用の文字列を取得
		/// </summary>
		/// <param name="msg">エラーメッセージ</param>
		/// <returns>エラー用のテキスト</returns>
		public string ToErrorString(string msg)
		{
			return this.RowData.ToErrorString(msg);
		}

		//セルが空かどうか
		public bool IsEmptyCell(AdvColumnName name)
		{
			return IsEmptyCell(name.ToString());
		}
		public bool IsEmptyCell(string name)
		{
			return this.RowData.IsEmptyCell(name);
		}

		//指定の名前のセルを、型Tとして解析・取得（データがなかったらエラーメッセージを出す）
		public T ParseCell<T>(AdvColumnName name)
		{
			return ParseCell<T>(name.ToString());
		}
		public T ParseCell<T>(string name)
		{
			return this.RowData.ParseCell<T>(name);
		}

		//指定の名前のセルを、型Tとして解析・取得（データがなかったらデフォルト値を返す）
		public T ParseCellOptional<T>(AdvColumnName name, T defaultVal)
		{
			return ParseCellOptional<T>(name.ToString(), defaultVal);
		}
		public T ParseCellOptional<T>(string name, T defaultVal)
		{
			return this.RowData.ParseCellOptional<T>(name, defaultVal);
		}

		//指定の名前のセルを、型Tとして解析・取得（データがなかったらfalse）
		public bool TryParseCell<T>(AdvColumnName name, out T val)
		{
			return TryParseCell<T>(name.ToString(), out val);
		}
		public bool TryParseCell<T>(string name, out T val)
		{
			return this.RowData.TryParseCell<T>(name, out val);
		}

		//指定の名前のセルを、型Tとして解析・取得（データがなかったらnull値を返す）
		public System.Nullable<T> ParseCellOptionalNull<T>(AdvColumnName name)where T : struct
		{
			return ParseCellOptionalNull<T>(name.ToString());
		}
		public System.Nullable<T> ParseCellOptionalNull<T>(string name)where T : struct
		{
			if (IsEmptyCell(name)) return null;
			return this.RowData.ParseCell<T>(name);
		}

		//現在の設定言語にローカライズされたテキストを取得
		public string ParseCellLocalizedText()
		{
			string columnName = AdvColumnName.Text.ToString();
			if (LanguageManager.Instance != null)
			{
				if (this.RowData.Grid.ContainsColumn(LanguageManager.Instance.CurrentLanguage))
				{
					columnName = LanguageManager.Instance.CurrentLanguage;
				}
			}
			if (this.RowData.IsEmptyCell(columnName))
			{	//指定の言語が空なら、デフォルトのText列を
				return this.RowData.ParseCellOptional<string>(AdvColumnName.Text.ToString(), "");
			}
			else
			{	//指定の言語を
				return this.RowData.ParseCellOptional<string>(columnName, "");
			}
		}


		//シナリオラベルを解析・取得
		public string ParseScenarioLabel(AdvColumnName name)
		{
			string label;
			if (!AdvCommandParser.TryParseScenarioLabel(this.RowData, name, out label))
			{
				Debug.LogError(ToErrorString(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotScenarioLabel, ParseCell<string>(name))));
			}
			return label;
		}

#if UNITY_EDITOR
		//エディタ表示で使うコマンドラベル
		public string CommandLabel
		{
			get
			{
				string commandName = this.GetType().ToString().Replace("Utage.AdvCommand", "");
				string no = (RowData == null) ? "?" : RowData.RowIndex.ToString();
				return no + " : " + commandName;				
			}
		}
#endif
		//今のコマンドから、エンティティ処理したコマンドを作成
		public AdvCommand CreateEntityCommand(AdvEngine engine, AdvScenarioPageData pageData )
		{
			StringGridRow row;
			if (!TryParseRowDataEntity( this.OriginalRowData, engine.Param.GetParameter, out row))
			{
				return this;
			}

			AdvCommand command = AdvCommandParser.CreateCommand(row, engine.DataManager.SettingDataManager);
			if (this.TextDataInPage != null)
			{
				command.InitTextDataInPage(this.TextDataInPage);
				this.TextDataInPage.Command = command;
			}
			return command;
		}

		//今のコマンドから、エンティティ処理したデータを作成
		static bool TryParseRowDataEntity(StringGridRow original, System.Func<string, object> GetParameter, out StringGridRow row)
		{
			bool ret = false;
			row = original.Clone(() => original.Grid);

			List<int> ignoreIndex = AdvParser.CreateMacroOrEntityIgnoreIndexArray(original.Grid);
			for (int i = 0; i < row.Strings.Length; ++i)
			{
				string str = row.Strings[i];
				if (ignoreIndex.Contains(i)) continue;
				if (str.Length <= 1) continue;
				if (!str.Contains("&")) continue;
				

				StringBuilder builder = new StringBuilder();
				int index = 0;
				while (index < str.Length)
				{
					if (str[index] == '&')
					{
						bool isEntity = false;
						int index2 = index + 1;
						while (index2 < str.Length)
						{
							if (index2 == str.Length - 1 || CheckEntitySeparator(str[index2 + 1]))
							{
								string key = str.Substring(index + 1, index2 - index);
								object param = GetParameter(key);
								if (param!=null)
								{
									builder.Append(param.ToString());
									index = index2 + 1;
									isEntity = true;
								}
								break;
							}
							else
							{
								++index2;
							}
						}
						if (isEntity)
						{
							ret = true;
							continue;
						}
					}

					builder.Append(str[index]);
					++index;
				}
				row.Strings[i] = builder.ToString();
			}
			return ret;
		}

		static bool CheckEntitySeparator(char c)
		{
			switch(c)
			{
				case '[':
				case ']':
				case '.':
					return true;
				default:
					return ExpressionToken.CheckSeparator(c);
			}
		}
	}
}