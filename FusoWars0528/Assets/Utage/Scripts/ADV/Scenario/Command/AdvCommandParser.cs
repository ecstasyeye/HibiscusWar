//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// 各コマンドの基底クラス
	/// </summary>
	public static class AdvCommandParser
	{
		//独自コマンドを作成するためのコールバック
		//独自にカスタムしたい、IDのコマンドだけ作成すればいい
		public delegate void CreateCustomCommnadFromID( string id, StringGridRow row, AdvSettingDataManager dataManger, ref AdvCommand commnad );
		static public CreateCustomCommnadFromID OnCreateCustomCommnadFromID;
		
		[System.Obsolete]
		static public Func<string, StringGridRow, AdvSettingDataManager, AdvCommand> CallBackCreateCustomCommnadFromID;

		//独自コマンドを作成するためのコールバック
		//コマンド名なし（テキスト表示やキャラ表示）の場合や、シナリオラベルの解析など、どのコマンドIDを使うかの時点からカスタムしたい場合はこっちを使う
		[System.Obsolete]
		static public Func<StringGridRow, AdvSettingDataManager, AdvCommand> CallBackCreateCustomCommnad;

		//選択肢などの連続終了用のカスタムコマンド作成用のコールバック
		[System.Obsolete]
		static public Func<AdvCommand, AdvSettingDataManager, AdvCommand> CallBackCreateCustomContiunesEndCommand;

		/// <summary>
		/// コマンド生成
		/// </summary>
		/// <param name="row">行データ</param>
		/// <param name="dataManager">データマネージャー</param>
		/// <returns>生成されたコマンド</returns>
		static public AdvCommand CreateCommand(StringGridRow row, AdvSettingDataManager dataManager)
		{
			if (row.IsCommentOut || IsComment(row))
			{
				//コメント
				return null;
			}
			///基本のコマンド解析処理
			AdvCommand command = CreateCommand(ParseCommandID(row), row, dataManager);

			if (command == null)
			{
				//列名がついたセル全て空かどうか
				if (row.IsAllEmptyCellNamedColumn())
				{
				}
				else
				{
					//記述ミス
					Debug.LogError(row.ToErrorString(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.CommnadParseNull)));
				}
			}

			return command;
		}

		public const string IdError = "Error";						//構文エラー
		public const string IdComment = "Comment";					//コメント
		public const string IdCharacter = "Character";				//キャラクター＆台詞表示
		public const string IdCharacterOff = "CharacterOff";		//キャラクター表示オフ
		public const string IdText = "Text";						//テキスト表示（地の文）
		public const string IdBg = "Bg";							//背景表示・切り替え
		public const string IdBgOff = "BgOff";						//キャラクター表示オフ
		public const string IdBgEvent = "BgEvent";					//イベント絵表示・切り替え
		public const string IdBgEventOff = "BgEventOff";			//イベント絵表示
		public const string IdSprite = "Sprite";					//スプライト表示
		public const string IdSpriteOff = "SpriteOff";				//スプライト表示オフ
		public const string IdMovie = "Movie";						//ムービー再生

		public const string IdSe = "Se";							//SE再生
		public const string IdStopSe = "StopSe";					//SE停止
		public const string IdBgm = "Bgm";							//BGM再生
		public const string IdStopBgm = "StopBgm";					//BGM停止
		public const string IdAmbience = "Ambience";				//環境音再生
		public const string IdStopAmbience = "StopAmbience";		//環境音停止
		public const string IdVoice = "Voice";						//ボイス再生
		public const string IdStopVoice = "StopVoice";				//ボイス停止
		public const string IdStopSound = "StopSound";				//サウンドの停止

		public const string IdSelection = "Selection";				//選択肢表示
		public const string IdSelectionEnd = "SelectionEnd";		//選択肢追加終了
		public const string IdSelectionClick = "SelectionClick";	//クリックによる選択肢表示
		public const string IdJump = "Jump";						//他シナリオにジャンプ
		public const string IdJumpRandom = "JumpRandom";			//ランダムジャンプ
		public const string IdJumpRandomEnd = "JumpRandomEnd";		//ランダムジャンプ終了
	
		public const string IdJumpSubroutine = "JumpSubroutine";			//サブルーチンへ飛ぶ
		public const string IdJumpSubroutineRandom = "JumpSubroutineRandom";			//ランダムにサブルーチンへ飛ぶ
		public const string IdJumpSubroutineRandomEnd = "JumpSubroutineRandomEnd";	//ランダムにサブルーチンへ飛ぶ終了
		public const string IdEndSubroutine = "EndSubroutine";		//サブルーチン終了

		public const string IdBeginMacro = "BeginMacro";			//マクロ定義開始
		public const string IdEndMacro = "EndMacro";				//マクロ定義終了

		public const string IdEffect = "Effect";					//エフェクト表示

		public const string IdWait = "Wait";						//一定時間のウェイト
		public const string IdWaitInput = "WaitInput";				//入力ウェイト
		public const string IdWaitCustom = "WaitCustom";			//カスタムウェイト
		
		public const string IdTween = "Tween";						//Tweenアニメ
		public const string IdFadeIn = "FadeIn";					//フェードイン
		public const string IdFadeOut = "FadeOut";					//フェードアウト
		public const string IdShake = "Shake";						//シェイク
		public const string IdVibrate = "Vibrate";					//バイブレーション
		

		public const string IdImageEffectStart = "ImageEffectStart";		//イメージエフェクトの開始
		public const string IdImageEffectEnd = "ImageEffectEnd";			//イメージエフェクトの終了

		public const string IdParam = "Param";						//パラメーター代入
		public const string IdIf = "If";							//パラメーター代入
		public const string IdElseIf = "ElseIf";					//パラメーター代入
		public const string IdElse = "Else";						//パラメーター代入
		public const string IdEndIf = "EndIf";						//パラメーター代入

		public const string IdShowMessageWindow = "ShowMessageWindow";		//メッセージウィンドウを強制表示
		public const string IdHideMessageWindow = "HideMessageWindow";		//メッセージウィンドウを強制非表示

		public const string IdShowMenuButton = "ShowMenuButton";			//メニューボタンを強制表示
		public const string IdHideMenuButton = "HideMenuButton";			//メニューボタンを強制非表示
/*
		public const string IdMessageWindowReset = "MwReset";			//メッセージウィンドウ操作　Reset
		public const string IdMessageWindowPosition = "MwPosition";		//メッセージウィンドウ操作　Position
		public const string IdMessageWindowSize = "MwSize";				//メッセージウィンドウ操作　Size
		public const string IdMessageWindowTexture = "MwTexture";		//メッセージウィンドウ操作　Texture
		public const string IdMessageWindowColor = "MwColor";			//メッセージウィンドウ操作　Color
*/
		public const string IdChangeMessageWindow = "ChangeMessageWindow";	//メッセージウィンドウ操作　変更
		public const string IdInitMessageWindow = "InitMessageWindow";		//メッセージウィンドウ操作　初期化


		public const string IdGuiReset = "GuiReset";				//GUI操作　Reset
		public const string IdGuiActive = "GuiActive";				//GUI操作　Active
		public const string IdGuiPosition = "GuiPosition";			//GUI操作　Position
		public const string IdGuiSize = "GuiSize";					//GUI操作　Size


		public const string IdSendMessage = "SendMessage";					//SendMessage処理（各ゲームに固有の処理のために）
		public const string IdSendMessageByName = "SendMessageByName";		//指定の名前のオブジェクトにSendMessage処理

		public const string IdEndScenario = "EndScenario";			//シナリオ終了
		public const string IdPauseScenario = "PauseScenario";		//シナリオ中断
		public const string IdEndSceneGallery = "EndSceneGallery";	//シーン回想終了

		public const string IdScenarioLabel = "ScenarioLabel";		//シナリオラベル
		public const string IdPageControler = "PageControl";		//ページの制御

		/// <summary>
		/// 基本のコマンド生成処理
		/// </summary>
		/// <param name="row">行データ</param>
		/// <param name="dataManager">データマネージャー</param>
		/// <returns>生成されたコマンド</returns>
		static public AdvCommand CreateCommand(string id, StringGridRow row, AdvSettingDataManager dataManager)
		{
			AdvCommand command = null;

			///独自のコマンド解析処理があるならそっちを先に
			if (OnCreateCustomCommnadFromID != null) OnCreateCustomCommnadFromID(id, row, dataManager, ref command);

			///基本のコマンド解析処理
			if (command == null) command = CreateCommandDefault(id, row, dataManager);
			return command;
		}

		/// <summary>
		/// 基本のコマンド生成処理
		/// </summary>
		/// <param name="id">コマンドID</param>
		/// <param name="dataManager">データマネージャー</param>
		/// <returns>生成されたコマンド</returns>
		static public AdvCommand CreateCommandDefault(string id, StringGridRow row, AdvSettingDataManager dataManager)
		{
			switch (id)
			{
				case IdCharacter:
					return new AdvCommandCharacter(row,dataManager);
				case IdText:
					return new AdvCommandText(row, dataManager);
				case IdCharacterOff:
					return new AdvCommandCharacterOff(row);
				case IdBg:
					return new AdvCommandBg(row, dataManager);
				case IdBgOff:
					return new AdvCommandBgOff(row);
				case IdBgEvent:
					return new AdvCommandBgEvent(row, dataManager);
				case IdBgEventOff:
					return new AdvCommandBgEventOff(row);
				case IdSprite:
					return new AdvCommandSprite(row, dataManager);
				case IdSpriteOff:
					return new AdvCommandSpriteOff(row);
				case IdMovie:
					return new AdvCommandMovie(row, dataManager);

				case IdTween:
					return new AdvCommandTween(row, dataManager);
				case IdFadeIn:
					return new AdvCommandFadeIn(row);
				case IdFadeOut:
					return new AdvCommandFadeOut(row);
				case IdShake:
					return new AdvCommandShake(row, dataManager);
				case IdVibrate:
					return new AdvCommandVibrate(row, dataManager);

				case IdImageEffectStart:
					return new AdvCommandImageEffectStart(row);
				case IdImageEffectEnd:
					return new AdvCommandImageEffectEnd(row);


				case IdSe:
					return new AdvCommandSe(row, dataManager);
				case IdStopSe:
					return new AdvCommandStopSe(row, dataManager);
				case IdBgm:
					return new AdvCommandBgm(row, dataManager);
				case IdStopBgm:
					return new AdvCommandStopBgm(row);
				case IdAmbience:
					return new AdvCommandAmbience(row, dataManager);
				case IdStopAmbience:
					return new AdvCommandStopAmbience(row);
				case IdVoice:
					return new AdvCommandVoice(row, dataManager);
				case IdStopVoice:
					return new AdvCommandStopVoice(row);
				case IdStopSound:
					return new AdvCommandStopSound(row);

				case IdWait:
					return new AdvCommandWait(row);
				case IdWaitInput:
					return new AdvCommandWaitInput(row);
				case IdWaitCustom:
					return new AdvCommandWaitCustom(row);
					

				case IdParam:
					return new AdvCommandParam(row, dataManager);
				case IdSelection:
					return new AdvCommandSelection(row, dataManager);
				case IdSelectionEnd:
					return new AdvCommandSelectionEnd(row, dataManager);
				case IdSelectionClick:
					return new AdvCommandSelectionClick(row, dataManager);
				case IdJump:
					return new AdvCommandJump(row, dataManager);
				case IdJumpRandom:
					return new AdvCommandJumpRandom(row, dataManager);
				case IdJumpRandomEnd:
					return new AdvCommandJumpRandomEnd(row, dataManager);

				case IdJumpSubroutine:
					return new AdvCommandJumpSubroutine(row, dataManager);
				case IdJumpSubroutineRandom:
					return new AdvCommandJumpSubroutineRandom(row, dataManager);
				case IdJumpSubroutineRandomEnd:
					return new AdvCommandJumpSubroutineRandomEnd(row, dataManager);
				case IdEndSubroutine:
					return new AdvCommandEndSubroutine(row, dataManager);

				case IdIf:
					return new AdvCommandIf(row, dataManager);
				case IdElseIf:
					return new AdvCommandElseIf(row, dataManager);
				case IdElse:
					return new AdvCommandElse(row);
				case IdEndIf:
					return new AdvCommandEndIf(row);

				case IdShowMessageWindow:
					return new AdvCommandShowMessageWindow(row);
				case IdHideMessageWindow:
					return new AdvCommandHideMessageWindow(row);
				case IdShowMenuButton:
					return new AdvCommandShowMenuButton(row);
				case IdHideMenuButton:
					return new AdvCommandHideMenuButton(row);


				case IdChangeMessageWindow:
					return new AdvCommandMessageWindowChangeCurrent(row);
				case IdInitMessageWindow:
					return new AdvCommandMessageWindowInit(row);

				case IdGuiReset:
					return new AdvCommandGuiReset(row);
				case IdGuiActive:
					return new AdvCommandGuiActive(row);
				case IdGuiPosition:
					return new AdvCommandGuiPosition(row);
				case IdGuiSize:
					return new AdvCommandGuiSize(row);

				case IdSendMessage:
					return new AdvCommandSendMessage(row);
				case IdSendMessageByName:
					return new AdvCommandSendMessageByName(row);
				
				case IdEndScenario:
					return new AdvCommandEndScenario(row);
				case IdPauseScenario:
					return new AdvCommandPauseScenario(row);
				case IdEndSceneGallery:
					return new AdvCommandEndSceneGallery(row);

				case IdPageControler:
					return new AdvCommandPageControler(row,dataManager);
				case IdScenarioLabel:
					return new AdvCommandScenarioLabel(row);
				default:
					return null;
			}
		}

		/// <summary>
		/// コマンド名なしの場合のコマンドIDを取得
		/// </summary>
		/// <param name="row">行データ</param>
		/// <param name="dataManager">データマネージャー</param>
		/// <returns>生成するコマンドID</returns>
		static string ParseCommandID(StringGridRow row)
		{
			string id = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Command, "");
			if (id == "")
			{
				//コマンドなしは、テキスト表示が基本
				string arg1 = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Arg1, "");
				if (!string.IsNullOrEmpty(arg1))
				{
					//パラメーターつきなので、キャラ表示
					return IdCharacter;
				}
				string text = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Text, "");
				string pageCtrl = AdvParser.ParseCellOptional<string>(row, AdvColumnName.PageCtrl, "");
				if (text.Length > 0 || pageCtrl.Length > 0 )
				{
					//地の文
					return IdText;
				}
				else
				{	//なにもないので空データ
					return null;
				}
			}
			else if (IsScenarioLabel(id))
			{
				//シナリオラベル
				id = IdScenarioLabel;
			}
			return id;
		}

		// シナリオラベルかチェック
		static public bool IsScenarioLabel(string str)
		{
			return ( !string.IsNullOrEmpty(str) && str.Length >= 2 && (str[0] == '*'));
		}
		//
		static public bool TryParseScenarioLabel(StringGridRow row, AdvColumnName columnName, out string scenarioLabel)
		{
			string label = AdvParser.ParseCell<string>(row, columnName);
			if (!IsScenarioLabel(label))
			{
				scenarioLabel = label;
				return false;
			}
			else
			{
				if (label.Length >= 3 && label[1] == '*')
				{
					scenarioLabel = row.Grid.SheetName + '*' + label.Substring(2);
				}
				else
				{
					scenarioLabel = label.Substring(1);
				}
				return true;
			}
		}

		//シナリオラベルを解析・取得
		static public string ParseScenarioLabel(StringGridRow row, AdvColumnName name)
		{
			string label;
			if (!TryParseScenarioLabel(row, name, out label))
			{
				Debug.LogError(row.ToErrorString(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotScenarioLabel, label)));
			}
			return label;
		}



		//コメントのコマンドかチェック
		static bool IsComment(StringGridRow row)
		{
			string command = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Command, "");
			if( string.IsNullOrEmpty(command) )
			{
				return false;
			}
			else if (command == IdComment)
			{
				return true;
			}
			else if (command.Length >= 2 && command.Substring(0, 2) == "//")
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}