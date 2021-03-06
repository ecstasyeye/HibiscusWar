//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;


namespace Utage
{
	/// <summary>
	/// UI全般の管理
	/// </summary>
	[AddComponentMenu("Utage/ADV/UiManager")]
	public class AdvUguiManager : AdvUiManager
	{
		/// <summary>
		/// メッセージウィンドウ
		/// </summary>
		public AdvUguiMessageWindowManager MessageWindow{ get { return messageWindow ?? (messageWindow = GetMessageWindowManagerCreateIfMissing());}}

		[SerializeField]
		AdvUguiMessageWindowManager messageWindow;

		[SerializeField]
		AdvUguiSelectionManager selection;

		[SerializeField]
		AdvUguiBacklogManager backLog;

		[SerializeField]
		bool disableMouseWheelBackLog = false;

		public override void Open()
		{
			this.gameObject.SetActive(true);
			ChangeStatus(UiStatus.Default);
		}

		public override void Close()
		{
			this.gameObject.SetActive(false);
			MessageWindow.Close();
			if (selection != null) selection.Close();
			if (backLog != null) backLog.Close();
		}

		protected override void ChangeStatus(UiStatus newStatus)
		{
			switch (newStatus)
			{
				case UiStatus.Backlog:
					if (backLog == null) return;

					MessageWindow.Close();
					if (selection != null) selection.Close();
					if (backLog != null) backLog.Open();
					Engine.Config.IsSkip = false;
					break;
				case UiStatus.HideMessageWindow:
					MessageWindow.Close();
					if (selection != null) selection.Close();
					if (backLog != null) backLog.Close();
					Engine.Config.IsSkip = false;
					break;
				case UiStatus.Default:
					MessageWindow.Open();
					if (selection != null) selection.Open();
					if (backLog != null) backLog.Close();
					break;
			}
			this.status = newStatus;
		}

		//ウインドウ閉じるボタンが押された
		void OnTapCloseWindow()
		{
			Status = UiStatus.HideMessageWindow;
		}

		protected virtual void Update()
		{
			//読み進みなどの入力
			bool IsInput = (Engine.Config.IsMouseWheelSendMessage && InputUtil.IsInputScrollWheelDown())
								|| InputUtil.IsInputKeyboadReturnDown();
			switch (Status)
			{
				case UiStatus.Backlog:
					break;
				case UiStatus.HideMessageWindow:	//メッセージウィンドウが非表示
					//右クリック
					if (InputUtil.IsMousceRightButtonDown())
					{	//通常画面に復帰
						Status = UiStatus.Default;
					}
					else if (!disableMouseWheelBackLog && InputUtil.IsInputScrollWheelUp())
					{
						//バックログ開く
						Status = UiStatus.Backlog;
					}
					break;
				case UiStatus.Default:
					if (IsShowingMessageWindow)
					{
						//テキストの更新
						Engine.Page.UpdateText();
					}
					if (IsShowingMessageWindow || Engine.SelectionManager.IsWaitInput)
					{	//入力待ち
						if (InputUtil.IsMousceRightButtonDown())
						{	//右クリックでウィンドウ閉じる
							Status = UiStatus.HideMessageWindow;
						}
						else if (!disableMouseWheelBackLog && InputUtil.IsInputScrollWheelUp())
						{	//バックログ開く
							Status = UiStatus.Backlog;
						}
						else
						{
							if (IsInput)
							{
								//メッセージ送り
								Engine.Page.InputSendMessage();
								base.IsInputTrig = true;
							}
						}
					}
					else
					{
						if (IsInput)
						{
							base.IsInputTrig = false;
						}
					}
					break;
			}
		}

		/// <summary>
		/// タッチされたとき
		/// </summary>
		public void OnPointerDown(BaseEventData data)
		{
			if ((data as PointerEventData).button != PointerEventData.InputButton.Left) return;

			switch (Status)
			{
				case UiStatus.Backlog:
					break;
				case UiStatus.HideMessageWindow:	//メッセージウィンドウが非表示
					Status = UiStatus.Default;
					break;
				case UiStatus.Default:
					if (Engine.Config.IsSkip)
					{
						//スキップ中ならスキップ解除
						Engine.Config.ToggleSkip();
					}
					else
					{
						if (IsShowingMessageWindow)
						{
							if (!Engine.Config.IsSkip)
							{
								//文字送り
								Engine.Page.InputSendMessage();
							}
						}
						base.OnPointerDown(data as PointerEventData);
					}
					break;
			}
		}

		

		//旧バージョンとの互換性のための処理、メッセージウィンドウがなかったら作る
		public AdvUguiMessageWindowManager GetMessageWindowManagerCreateIfMissing()
		{
			AdvUguiMessageWindowManager[] managers = GetComponentsInChildren<AdvUguiMessageWindowManager>(true);
			if (managers.Length > 0)
			{
				return managers[0];
			}
			else
			{
				//旧バージョンとの互換性のため、なかったら作る
				AdvUguiMessageWindowManager manager = UtageToolKit.AddChildGameObjectComponent<AdvUguiMessageWindowManager>(this.transform, "MessageWindowManager");
				RectTransform rect = manager.gameObject.AddComponent<RectTransform>();
				rect.anchorMin = Vector2.zero;
				rect.anchorMax = Vector2.one;
				rect.sizeDelta = Vector2.zero;
				rect.SetAsFirstSibling();

				AdvUguiMessageWindow[] windows = GetComponentsInChildren<AdvUguiMessageWindow>(true);
				foreach (var window in windows)
				{
					window.transform.SetParent(rect, true);
					if (window.transform.localScale == Vector3.zero)
					{
						window.transform.localScale = Vector3.one;
					}
				}
				return manager;
			}
		}
	}
}