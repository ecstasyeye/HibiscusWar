//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
#if LegacyUtageUi

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Utage
{
	//ADVエンジンオブジェクト作成の管理エディタウイドウ
	public class CreateAdvEngineWindow : EditorWindow
	{
		[SerializeField]
		int gameScreenWidth;
		[SerializeField]
		int gameScreenHeight;

		[SerializeField]
		FontData font;
		[SerializeField]
		AudioClip clickSe;

		[SerializeField]
		Sprite transitionFadeBg;
		[SerializeField]
		Sprite msgWindowSprite;
		[SerializeField]
		bool isEnableCloseButton;
		[SerializeField]
		Sprite closeButtonSprite;

		[SerializeField]
		AdvUiSelection selectionItemPrefab;

		[SerializeField]
		bool isEnableBackLog;
		[SerializeField]
		Sprite backLogButtonSprite;
		[SerializeField]
		Sprite backLogFilterSprite;
		[SerializeField]
		Sprite backLogScrollUpArrow;
		[SerializeField]
		AdvUiBacklog backLogItemPrefab;
		[SerializeField]
		Sprite backLogCloseButtonSprite;

		/// <summary>
		/// エディタ上に保存してあるデータをロード
		/// </summary>
		void Load()
		{
			gameScreenWidth = UtageEditorPrefs.LoadInt(UtageEditorPrefs.Key.GameScreenWidth, 800);
			gameScreenHeight = UtageEditorPrefs.LoadInt(UtageEditorPrefs.Key.GameScreenHegiht, 600);
			font = UtageEditorPrefs.LoadAsset<FontData>(UtageEditorPrefs.Key.CreateAdvEngineWindowFont,
				"Assets/Utage/Examples/ScriptableObject/Example FontData.asset");
			clickSe = UtageEditorPrefs.LoadAsset<AudioClip>(UtageEditorPrefs.Key.CreateAdvEngineWindowClickSe,
				"Assets/Utage/Examples/Audio/mouse_click.wav");

			transitionFadeBg = UtageEditorPrefs.LoadAsset<Sprite>(UtageEditorPrefs.Key.CreateAdvEngineWindowTransitionFadeBg,
				"Assets/Utage/Examples/Textures/UI/transitionFadeBg.png");

			msgWindowSprite = UtageEditorPrefs.LoadAsset<Sprite>(UtageEditorPrefs.Key.CreateAdvEngineWindowMsgWindowSprite,
				"Assets/Utage/Examples/Textures/UI/MessageWindow.png");
			isEnableCloseButton = UtageEditorPrefs.LoadBool(UtageEditorPrefs.Key.CreateAdvEngineWindowIsEnableCloseButton, true);
			closeButtonSprite = UtageEditorPrefs.LoadAsset<Sprite>(UtageEditorPrefs.Key.CreateAdvEngineWindowCloseButtonSprite,
				"Assets/Utage/Examples/Textures/UI/CloseIcon.png");

			selectionItemPrefab = UtageEditorPrefs.LoadPrefab<AdvUiSelection>(UtageEditorPrefs.Key.CreateAdvEngineWindowSelectionItemPrefab,
				"Assets/Utage/Examples/Prefabs/SelectionItem.prefab");

			isEnableBackLog = UtageEditorPrefs.LoadBool(UtageEditorPrefs.Key.CreateAdvEngineWindowIsEnableCloseButton,true);
			backLogButtonSprite = UtageEditorPrefs.LoadAsset<Sprite>(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogButtonSprite,
				"Assets/Utage/Examples/Textures/UI/SystemButtonSS.png");
			backLogFilterSprite = UtageEditorPrefs.LoadAsset<Sprite>(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogFilterSprite,
				"Assets/Utage/Examples/Textures/UI/filterBg.png");
			backLogScrollUpArrow = UtageEditorPrefs.LoadAsset<Sprite>(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogScrollUpArrow,
				"Assets/Utage/Examples/Textures/UI/AllowUp.png");
			backLogItemPrefab = UtageEditorPrefs.LoadPrefab<AdvUiBacklog>(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogItemPrefab,
				"Assets/Utage/Examples/Prefabs/BacklogItem.prefab");
			backLogCloseButtonSprite = UtageEditorPrefs.LoadAsset<Sprite>(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogCloseButtonSprite,
				"Assets/Utage/Examples/Textures/UI/SystemButtonSS.png");
		}

		/// <summary>
		/// エディタ上に保存してあるデータをセーブ
		/// </summary>
		void Save()
		{
			UtageEditorPrefs.SaveInt(UtageEditorPrefs.Key.GameScreenWidth, gameScreenWidth);
			UtageEditorPrefs.SaveInt(UtageEditorPrefs.Key.GameScreenHegiht, gameScreenHeight);
			UtageEditorPrefs.SaveAsset(UtageEditorPrefs.Key.CreateAdvEngineWindowFont, font);
			UtageEditorPrefs.SaveAsset(UtageEditorPrefs.Key.CreateAdvEngineWindowClickSe, clickSe);

			UtageEditorPrefs.SaveAsset(UtageEditorPrefs.Key.CreateAdvEngineWindowTransitionFadeBg, transitionFadeBg);

			UtageEditorPrefs.SaveAsset(UtageEditorPrefs.Key.CreateAdvEngineWindowMsgWindowSprite, msgWindowSprite);
			UtageEditorPrefs.SaveBool(UtageEditorPrefs.Key.CreateAdvEngineWindowIsEnableCloseButton, isEnableCloseButton);
			UtageEditorPrefs.SaveAsset(UtageEditorPrefs.Key.CreateAdvEngineWindowCloseButtonSprite, closeButtonSprite);

			UtageEditorPrefs.SaveAsset(UtageEditorPrefs.Key.CreateAdvEngineWindowSelectionItemPrefab, selectionItemPrefab);
			
			UtageEditorPrefs.SaveBool(UtageEditorPrefs.Key.CreateAdvEngineWindowIsEnableCloseButton, isEnableBackLog);
			UtageEditorPrefs.SaveAsset(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogButtonSprite, backLogButtonSprite);
			UtageEditorPrefs.SaveAsset(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogFilterSprite, backLogFilterSprite);
			UtageEditorPrefs.SaveAsset(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogScrollUpArrow, backLogScrollUpArrow);
			UtageEditorPrefs.SaveAsset(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogItemPrefab, backLogItemPrefab);
			UtageEditorPrefs.SaveAsset(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogCloseButtonSprite, backLogCloseButtonSprite);
		}

		/// <summary>
		/// エディタ上に保存してあるデータをﾃﾞﾌｫﾙﾄ値に戻す
		/// </summary>
		void ResetSave()
		{
			DeleteAllSaveData();
			Load();
		}

		/// <summary>
		/// セーブデータを全て削除
		/// </summary>
		void DeleteAllSaveData()
		{
			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.GameScreenWidth);
			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.GameScreenHegiht);
			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.CreateAdvEngineWindowFont);
			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.CreateAdvEngineWindowClickSe);

			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.CreateAdvEngineWindowTransitionFadeBg);

			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.CreateAdvEngineWindowMsgWindowSprite);
			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.CreateAdvEngineWindowIsEnableCloseButton);
			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.CreateAdvEngineWindowCloseButtonSprite);

			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.CreateAdvEngineWindowSelectionItemPrefab);

			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.CreateAdvEngineWindowIsEnableCloseButton);
			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogButtonSprite);
			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogFilterSprite);
			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogScrollUpArrow);
			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogItemPrefab);
			UtageEditorPrefs.Delete(UtageEditorPrefs.Key.CreateAdvEngineWindowBackLogCloseButtonSprite);
		}


		void OnFocus()
		{
			Load();
		}
		void OnLostFocus()
		{
			Save();
		}
		void OnDestroy()
		{
			Save();
		}

		void OnGUI()
		{
			SerializedObject serializedObject = new SerializedObject(this);
			serializedObject.Update();
			DrawProperties(serializedObject);
			if( serializedObject.ApplyModifiedProperties() )
			{
				Save();
			}
		}

		void DrawProperties(SerializedObject serializedObject)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Reset to default values");
			if (GUILayout.Button("Reset", GUILayout.MaxWidth(80)))
			{
				ResetSave();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(8f);

			GUILayout.Space(4f);
			int width = EditorGUILayout.IntField("Width", gameScreenWidth);
			if (gameScreenWidth != width && width > 0)
			{
				gameScreenWidth = width;
				Save();
			}
			int height = EditorGUILayout.IntField("Hegiht", gameScreenHeight);
			if (gameScreenHeight != height && height > 0)
			{
				gameScreenHeight = height;
				Save();
			}

			GUILayout.Space(4f);
			UtageEditorToolKit.PropertyField(serializedObject, "font", "Font");
			UtageEditorToolKit.PropertyField(serializedObject, "clickSe", "Click SE");

			//トランジション
			UtageEditorToolKit.BeginGroup("Transition");
			UtageEditorToolKit.PropertyField(serializedObject, "transitionFadeBg", "Transition Fade Bg Sprite");
			UtageEditorToolKit.EndGroup();

			//選択肢
			UtageEditorToolKit.BeginGroup("Selection");
			AdvUiSelection selectionPrefab = UtageEditorToolKit.PrefabField<AdvUiSelection>("itemPrefab", this.selectionItemPrefab);
			if (this.selectionItemPrefab != selectionPrefab)
			{
				this.selectionItemPrefab = selectionPrefab;
				Save();
			}
			UtageEditorToolKit.EndGroup();

			//メッセージウィンドウ
			UtageEditorToolKit.BeginGroup("Message Window");
			UtageEditorToolKit.PropertyField(serializedObject, "msgWindowSprite", "Message Window Sprite");
			UtageEditorToolKit.PropertyField(serializedObject, "isEnableCloseButton", "Is Enable Close Button");
			EditorGUI.BeginDisabledGroup(!this.isEnableCloseButton);
			UtageEditorToolKit.PropertyField(serializedObject, "closeButtonSprite", "Close Button Sprite");
			EditorGUI.EndDisabledGroup();
			UtageEditorToolKit.EndGroup();

			//バックログ
			UtageEditorToolKit.BeginGroup("BackLog");
			UtageEditorToolKit.PropertyField(serializedObject, "isEnableBackLog", "Is Enable BackLog");
			EditorGUI.BeginDisabledGroup(!this.isEnableBackLog);
			UtageEditorToolKit.PropertyField(serializedObject, "backLogButtonSprite", "Open Button Sprite");
			UtageEditorToolKit.PropertyField(serializedObject, "backLogFilterSprite", "Filter Sprite");
			UtageEditorToolKit.PropertyField(serializedObject, "backLogCloseButtonSprite", "Close Button Sprite");
			UtageEditorToolKit.PropertyField(serializedObject, "backLogScrollUpArrow", "Scroll Arrow");
			AdvUiBacklog backLogItemPrefab = UtageEditorToolKit.PrefabField<AdvUiBacklog>("itemPrefab", this.backLogItemPrefab);
			if (this.backLogItemPrefab != backLogItemPrefab)
			{
				this.backLogItemPrefab = backLogItemPrefab;
				Save();
			}
			EditorGUI.EndDisabledGroup();
			UtageEditorToolKit.EndGroup();

			bool isEnable = (font != null && transitionFadeBg != null && msgWindowSprite != null && selectionItemPrefab != null);
			EditorGUI.BeginDisabledGroup(!isEnable);
			if (GUILayout.Button("Create"))
			{
				CreateAdvEngile();
			}
			EditorGUI.EndDisabledGroup();
		}

		void CreateAdvEngile()
		{
			GameObject go = new GameObject("AdvEngine");
			AdvEngine engine = go.AddComponent<AdvEngine>();

			//レイヤーマネージャー
			AdvLayerManager layerManager = UtageToolKit.AddChildGameObjectComponent<AdvLayerManager>(engine.transform, "LayerManager");
			Node2D node2dLayerManager = layerManager.GetComponent<Node2D>();
			node2dLayerManager.SortKey = "LayerManager";

			//トランジションマネージャー
			AdvTransitionManager transitionManager = UtageToolKit.AddChildGameObjectComponent<AdvTransitionManager>(engine.transform, "TransitionManager");
			transitionManager.GetComponent<Node2D>().SortKey = "Transition";
			Sprite2D transtionBg = UtageToolKit.AddChildGameObjectComponent<Sprite2D>(transitionManager.transform, "Bg");
			transtionBg.Sprite = transitionFadeBg;
			transtionBg.SizeType = Sprite2D.SpriteSizeType.StrechBoth;

			//UIマネージャー
			AdvUiManager uiManager = UtageToolKit.AddChildGameObjectComponent<AdvUiManager>(engine.transform, "UI");
			
			//インプットマネージャー
			AdvLegacyInputManager input = UtageToolKit.AddChildGameObjectComponent<AdvLegacyInputManager>(uiManager.transform, "InputManager");
			input.InitOnCreate(engine, 1.0f / 100 * gameScreenWidth, 1.0f / 100 * gameScreenHeight);
			input.GetComponent<Node2D>().SortKey = "InputManager";

			//メッセージウィンドウ
			float fontSize = 30;
			float windowWidth = msgWindowSprite.rect.width;
			float windowHeight = msgWindowSprite.rect.height;
			AdvUiMessageWindow msgWindow = UtageToolKit.AddChildGameObjectComponent<AdvUiMessageWindow>(uiManager.transform, "MessageWindow");
			msgWindow.GetComponent<Node2D>().SortKey = "MessageWindow";
			GameObject windowRootChildren = UtageToolKit.AddChildGameObject(msgWindow.transform, "RootChildren");
			TextArea2D messageText = UtageToolKit.AddChildGameObjectComponent<TextArea2D>(windowRootChildren.transform, "MessageText");
			messageText.Type = TextArea2D.ViewType.Outline;
			messageText.Font = font;
			messageText.text = "Message";
			messageText.Size = fontSize;
			messageText.LayoutType = TextArea2D.Layout.TopLeft;
			messageText.MaxWidth = (int)(windowWidth - fontSize*4);
			messageText.MaxHeight = (int)(windowHeight - (fontSize*2+50));
			messageText.transform.localPosition = new Vector3(-(float)messageText.MaxWidth / 2 / 100, (windowHeight / 2 - (fontSize+40)) / 100, 0);
			messageText.LetterSpace = 2;
			messageText.LineSpace = 8;
			messageText.WordWrapType = TextArea2D.WordWrap.Default | TextArea2D.WordWrap.JapaneseKinsoku;
			messageText.LocalOrderInLayer = 10;

			//名前表示
			TextArea2D nameText = UtageToolKit.AddChildGameObjectComponent<TextArea2D>(windowRootChildren.transform, "NameText");
			nameText.Type = TextArea2D.ViewType.Outline;
			nameText.Font = font;
			nameText.text = "Name";
			nameText.Size = 30;
			nameText.LayoutType = TextArea2D.Layout.TopLeft;
			nameText.transform.localPosition = new Vector3(-(float)messageText.MaxWidth / 2 / 100, (windowHeight / 2 - 25) / 100, 0);
			nameText.LocalOrderInLayer = 10;

			//閉じるボタン
			if (isEnableCloseButton)
			{
				LegacyUiButton closeButton = UtageToolKit.AddChildGameObjectComponent<LegacyUiButton>(windowRootChildren.transform, "CloseButton");
				Sprite2D closeButtonBg = UtageToolKit.AddChildGameObjectComponent<Sprite2D>(closeButton.transform, "Bg");
				closeButtonBg.Sprite = closeButtonSprite;
				closeButton.transform.localPosition = new Vector3(((windowWidth - closeButtonSprite.rect.width) / 2 - 10) / 100, ((windowHeight - closeButtonSprite.rect.height) / 2 - 5) / 100, -0.01f);
				closeButton.Target = msgWindow.gameObject;
				closeButton.FunctionName = "OnTapCloseWindow";
				closeButton.Se = clickSe;
			}

			//メッセージウィンドウ背景
			Node2D windowBgRoot = UtageToolKit.AddChildGameObjectComponent<Node2D>(windowRootChildren.transform, "WindowRoot");
			Sprite2D windowBg = UtageToolKit.AddChildGameObjectComponent<Sprite2D>(windowBgRoot.transform, "Bg");
			windowBg.Sprite = msgWindowSprite;

			msgWindow.InitOnCreate(engine, messageText, nameText, windowRootChildren, windowBgRoot);
			msgWindow.transform.localPosition = new Vector3(0, (-gameScreenHeight + windowHeight) / 2 / 100, 0);

			//選択肢
			AdvUiSelectionManager selection = UtageToolKit.AddChildGameObjectComponent<AdvUiSelectionManager>(uiManager.transform, "Selection");
			selection.InitOnCreate(engine, selectionItemPrefab);
			selection.GetComponent<Node2D>().SortKey = "Selection";
			selection.ListView.ClipWidthPx = Mathf.Max(gameScreenWidth / 2, gameScreenWidth - 100);
			selection.ListView.ClipHeightPx = Mathf.Max(gameScreenHeight / 2, gameScreenHeight - 100);
			selection.ListView.Type = LegacyUiListView.LitViewType.Vertical;

			//バックログ
			AdvUiBacklogManager backLogManger = null;
			if (isEnableBackLog)
			{
				LegacyUiButton openButton = UtageToolKit.AddChildGameObjectComponent<LegacyUiButton>(windowRootChildren.transform, "BackLogButton");
				UtageToolKit.AddChildGameObjectComponent<Sprite2D>(openButton.transform, "Bg").Sprite = backLogButtonSprite;
				TextArea2D text = UtageToolKit.AddChildGameObjectComponent<TextArea2D>(openButton.transform, "Text");
				text.Font = font;
				text.text = "Log";
				text.LocalOrderInLayer = 10;
				openButton.transform.localPosition = new Vector3(((windowWidth - backLogButtonSprite.rect.width) / 2 - closeButtonSprite.rect.width - 20) / 100, ((windowHeight - closeButtonSprite.rect.height) / 2 - 5) / 100, -0.01f);
				openButton.Target = msgWindow.gameObject;
				openButton.FunctionName = "OnTapBackLog";
				openButton.Se = clickSe;

				backLogManger = UtageToolKit.AddChildGameObjectComponent<AdvUiBacklogManager>(uiManager.transform, "BackLog");
				backLogManger.InitOnCreate(engine, backLogItemPrefab);
				backLogManger.GetComponent<Node2D>().SortKey = "BackLog";
				backLogManger.ListView.ClipWidthPx = Mathf.Max(gameScreenWidth / 2, gameScreenWidth - 100);
				backLogManger.ListView.ClipHeightPx = Mathf.Max(gameScreenHeight / 2, gameScreenHeight - 100);
				backLogManger.ListView.Type = LegacyUiListView.LitViewType.Vertical;
				backLogManger.ListView.IsRepositionReverse = true;

				Sprite2D filter = UtageToolKit.AddChildGameObjectComponent<Sprite2D>(backLogManger.transform, "filter");
				filter.Sprite = backLogFilterSprite;
				filter.SizeType = Sprite2D.SpriteSizeType.StrechBoth;
				filter.LocalColor = Color.black;
				filter.LocalAlpha = 0.5f;

				float arrowY = (float)backLogManger.ListView.ClipHeightPx/100/2;
				Sprite2D minArrow = UtageToolKit.AddChildGameObjectComponent<Sprite2D>(backLogManger.transform, "MinArrow");
				minArrow.Sprite = backLogScrollUpArrow;
				minArrow.LocalOrderInLayer = 30;
				minArrow.transform.localPosition = new Vector3(0, arrowY);
				backLogManger.ListView.MinArrow = minArrow.gameObject;
				Sprite2D maxArrow = UtageToolKit.AddChildGameObjectComponent<Sprite2D>(backLogManger.transform, "MaxArrow");
				maxArrow.Sprite = backLogScrollUpArrow;
				maxArrow.LocalOrderInLayer = 30;
				maxArrow.transform.localPosition = new Vector3(0, -arrowY);
				maxArrow.transform.localEulerAngles = new Vector3(0, 0, 180);
				backLogManger.ListView.MaxArrow = maxArrow.gameObject;

				LegacyUiButton closeButton = UtageToolKit.AddChildGameObjectComponent<LegacyUiButton>(backLogManger.transform, "CloseButton");
				Sprite2D closeButtonBg = UtageToolKit.AddChildGameObjectComponent<Sprite2D>(closeButton.transform, "Bg");
				closeButtonBg.Sprite = backLogCloseButtonSprite;
				closeButton.transform.localPosition = new Vector3((-1.0f * (gameScreenWidth - backLogCloseButtonSprite.rect.width) / 2 + 5) / 100, ((gameScreenHeight - backLogCloseButtonSprite.rect.height) / 2 - 5) / 100, -0.01f);
				closeButton.Target = backLogManger.gameObject;
				closeButton.FunctionName = "OnTapBack";
				closeButton.Se = clickSe;
				TextArea2D textClose = UtageToolKit.AddChildGameObjectComponent<TextArea2D>(closeButton.transform, "Text");
				textClose.Font = font;
				textClose.text = "";
				textClose.LocalOrderInLayer = 10;
				LegacyLocalize localize = textClose.gameObject.AddComponent<LegacyLocalize>();
				localize.Key = SystemText.Back.ToString();

			}


			engine.InitOnCreate(layerManager, transitionManager, uiManager);

			AdvLegacyUiManager legacyUiManager = uiManager as AdvLegacyUiManager;
			if (legacyUiManager != null)
			{
				legacyUiManager.InitOnCreate(engine, msgWindow, selection, backLogManger);
			}
			Selection.activeGameObject = go;
			Undo.RegisterCreatedObjectUndo(go, "CreateAdvEngile");
		}
	}
}
#endif
