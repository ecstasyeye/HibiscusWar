//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace Utage
{

	
	[System.Serializable]
	public class MessageWindowEvent : UnityEvent<AdvMessageWindowManager> { }

	/// <summary>
	/// メッセージウィンドウ管理
	/// </summary>
	[AddComponentMenu("Utage/ADV/Internal/MessageWindowManager")]
	public class AdvMessageWindowManager : MonoBehaviour, IAdvCustomSaveDataIO
	{
		//リセット時の処理
		public MessageWindowEvent OnReset { get { return onReset; } }
		[SerializeField]
		MessageWindowEvent onReset = new MessageWindowEvent();

		//アクティブなウィンドウが変わった
		public MessageWindowEvent OnChangeActiveWindows { get { return onChangeActiveWindows; } }
		[SerializeField]
		MessageWindowEvent onChangeActiveWindows = new MessageWindowEvent();

		//現在ページのウィンドウが変わった
		public MessageWindowEvent OnChangeCurrentWindow { get { return onChangeCurrentWindow; } }
		[SerializeField]
		MessageWindowEvent onChangeCurrentWindow = new MessageWindowEvent();

		//現在ページのテキストが変わった
		public MessageWindowEvent OnTextChage { get { return onTextChage; } }
		[SerializeField]
		MessageWindowEvent onTextChage = new MessageWindowEvent();		

		/// <summary>ADVエンジン</summary>
		public AdvEngine Engine { get { return this.engine ?? (this.engine = GetComponent<AdvEngine>()); } }
		AdvEngine engine;
		
		//管理対象のウィンドウ
		public Dictionary<string, AdvMessageWindow> AllWindows { get { return allWindows; } }
		Dictionary<string, AdvMessageWindow> allWindows = new Dictionary<string, AdvMessageWindow>();

		//起動時にアクティブにするウィンドウ
		List<string> defaultActiveWindowNameList = new List<string>();

		//現在アクティブになっているウィンドウ
		public Dictionary<string,AdvMessageWindow> ActiveWindows { get { return activeWindows; } }
		Dictionary<string, AdvMessageWindow> activeWindows = new Dictionary<string, AdvMessageWindow>();

		//現在ページのウィンドウ
		public AdvMessageWindow CurrentWindow { get; private set; }

		//切り替わる前のページウィンドウ
		public AdvMessageWindow LastWindow{ get; private set; }

		//指定の名前が現在ページのウィンドウか
		public bool IsCurrent(string name)
		{
			return CurrentWindow.Name == name;
		}

		//指定の名前がアィティブなウィンドウか
		public bool IsActiveWindow(string name)
		{
			return ActiveWindows.ContainsKey(name);
		}

		//ゲーム起動時の初期化
		void Awake()
		{
			IAdvMessageWindow[] windows = GetComponentsInChildren<IAdvMessageWindow>(true);
			foreach (var item in windows)
			{
				allWindows.Add(item.gameObject.name, new AdvMessageWindow(item));
			}
			foreach (var item in windows)
			{
				if (item.gameObject.activeSelf) defaultActiveWindowNameList.Add(item.gameObject.name);
			}

			//登録されたイベントを呼ぶ
			foreach (var item in windows)
			{
				item.OnInit(this);
			}
		}

		internal void ChangeActiveWindows(List<string> names)
		{
			//複数ウィンドウの設定
			ActiveWindows.Clear();
			foreach (var name in names)
			{
				AdvMessageWindow window;
				if (!allWindows.TryGetValue(name, out window))
				{
					Debug.LogError(name + " is not found in message windows");
				}
				else
				{
					ActiveWindows.Add(name, window);
				}
			}

			//登録されたイベントを呼ぶ
			CalllEventActiveWindows();
		}

		//現在のウィンドウかどうかが変わった
		void CalllEventActiveWindows()
		{
			foreach (var item in allWindows.Values)
			{
				item.ChangeActive(IsActiveWindow(item.Name));
			}
			OnChangeActiveWindows.Invoke(this);
		}

		//メッセージウィンドを変更
		internal void ChangeCurrentWindow(string name)
		{
			//設定なしならなにもしない
			if (string.IsNullOrEmpty(name)) return;

			if (CurrentWindow != null && CurrentWindow.Name == name)
			{
				//変化なし
				return;
			}
			else
			{
				AdvMessageWindow window;
				if (!ActiveWindows.TryGetValue(name, out window))
				{
					//アクティブなウィンドウにない場合、全ウィンドウから検索
					if (!allWindows.TryGetValue(name, out window))
					{
						//全ウィンドウにもない場合どうしようもないので、デフォルトウィンドウを
						Debug.LogWarning(name + "is not found in window manager");
						name = defaultActiveWindowNameList[0];
					}
					//非アクティブなウィンドウと交換
					if (CurrentWindow != null) ActiveWindows.Remove(CurrentWindow.Name);
					ActiveWindows.Add(name, window);

					//登録されたイベントを呼ぶ
					CalllEventActiveWindows();
				}
				LastWindow = CurrentWindow;
				CurrentWindow = window;
				//登録されたイベントを呼ぶ
				if (LastWindow != null) LastWindow.ChangeCurrent(false);
				CurrentWindow.ChangeCurrent(true);
				OnChangeCurrentWindow.Invoke(this);
			}
		}

		//指定の名前のウィンドウを検索
		internal AdvMessageWindow FindWindow(string name)
		{
			AdvMessageWindow window = CurrentWindow;
			if (!string.IsNullOrEmpty(name))
			{
				if (!AllWindows.TryGetValue(name, out window))
				{
					Debug.LogError(name + "is not found in all message windows");
				}
			}
			return window;
		}

		//テキストが変わった
		internal void OnPageTextChange(AdvPage page)
		{
			CurrentWindow.PageTextChange(page);
			OnTextChage.Invoke(this);
		}


		//セーブデータのキー
		public string SaveKey { get { return "MessageWindowManager"; } }

		//クリアする(初期状態に戻す)
		public virtual void OnClear()
		{
			if (defaultActiveWindowNameList.Count <= 0)
			{
				Debug.LogWarning("defaultWindowNameList is zero");
			}
			else
			{
				ChangeActiveWindows(defaultActiveWindowNameList);
				ChangeCurrentWindow(defaultActiveWindowNameList[0]);
				//登録されたイベントを呼ぶ
				foreach (var item in allWindows.Values)
				{
					item.Reset();
				}
				OnReset.Invoke(this);
			}
		}

		const int Version = 0;
		//書き込み
		public virtual void OnWrite(System.IO.BinaryWriter writer)
		{
			writer.Write(Version);

			writer.Write(ActiveWindows.Count);
			foreach( var item in ActiveWindows)
			{
				writer.Write(item.Key);
				byte[] buffer = BinaryUtil.BinaryWrite(item.Value.WritePageData);
				writer.Write(buffer.Length);
				writer.Write(buffer);
			}
			string currentWindowName = CurrentWindow == null ? "" : CurrentWindow.Name;
			writer.Write(currentWindowName);
		}

		//読み込み
		public virtual void OnRead(System.IO.BinaryReader reader)
		{
			//バージョンチェック
			int version = reader.ReadInt32();
			if (version == Version)
			{
				List<string> nameList = new List<string>();
				int count = reader.ReadInt32();
				for(int i = 0; i < count; ++i)
				{
					string key = reader.ReadString();
					byte[] buffer = reader.ReadBytes( reader.ReadInt32() );
					nameList.Add(key);
					BinaryUtil.BinaryRead( buffer, FindWindow(key).ReadPageData );
				}
				string currentWindowName = reader.ReadString();

				ChangeActiveWindows(nameList);
				ChangeCurrentWindow(currentWindowName);
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, version));
			}
		}
	}
}