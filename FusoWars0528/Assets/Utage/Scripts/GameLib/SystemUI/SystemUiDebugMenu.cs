using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// デバッグメニュー表示
	/// </summary>
	[AddComponentMenu("Utage/Lib/System UI/DebugMenu")]
	public class SystemUiDebugMenu : MonoBehaviour
	{
		[SerializeField]
		GameObject buttonRoot;

		[SerializeField]
		GameObject buttonViewRoot;
		
		[SerializeField]
		UguiLocalize buttonText;

		[SerializeField]
		GameObject debugInfo;

		[SerializeField]
		Text debugInfoText;

		[SerializeField]
		GameObject debugLog;

		[SerializeField]
		Text debugLogText;
		//	public UILabel debugLogLabel;

		[SerializeField]
		GameObject rootDebugMenu;

		[SerializeField]
		GameObject targetDeleteAllSaveData;

		void Start()
		{
			if (!UnityEngine.Debug.isDebugBuild)
			{
				buttonRoot.SetActive(false);
			}

			ClearAll();
			ChangeMode(currentMode);
		}

		enum Mode
		{
			Hide,
			Info,
			Log,
			Memu,
			Max,
		};

		Mode currentMode = Mode.Hide;
		public void OnClickSwitchButton()
		{
			if (!UnityEngine.Debug.isDebugBuild) return;

			ChangeMode(currentMode + 1);
		}

		void ChangeMode(Mode mode)
		{
			if (currentMode == mode) return;
			if (mode >= Mode.Max) mode = 0;

			currentMode = mode;
			ClearAll();
			StopAllCoroutines();
			switch (currentMode)
			{
				case Mode.Info:
					StartCoroutine(CoUpdateInfo());
					break;
				case Mode.Log:
					StartCoroutine(CoUpdateLog());
					break;
				case Mode.Memu:
					StartCoroutine(CoUpdateMenu());
					break;
				case Mode.Hide:
					break;
			};
		}

		void ClearAll()
		{
			buttonViewRoot.SetActive(false);

			debugInfo.SetActive(false);
			debugLog.SetActive(false);

			rootDebugMenu.SetActive(false);
		}

		IEnumerator CoUpdateInfo()
		{
			buttonViewRoot.SetActive(true);
			buttonText.Key = SystemText.DebugInfo.ToString();

			debugInfo.SetActive(true);
			while (true)
			{
				debugInfoText.text = DebugPrint.GetDebugString();
				yield return 0;
			}
		}

		IEnumerator CoUpdateLog()
		{
			buttonViewRoot.SetActive(true);
			buttonText.Key = SystemText.DebugLog.ToString();

			debugLog.SetActive(true);
			debugLogText.text += DebugPrint.GetLogString();

			yield break;
		}

		IEnumerator CoUpdateMenu()
		{
			buttonViewRoot.SetActive(true);
			buttonText.Key = SystemText.DebugMenu.ToString();

			rootDebugMenu.SetActive(true);

			yield break;
		}

		//セーブデータを消去して終了
		public void OnClickDeleteAllSaveDataAndQuit()
		{
			UtageToolKit.SafeSendMessage(targetDeleteAllSaveData, "OnDeleteAllSaveDataAndQuit");
			PlayerPrefs.DeleteAll();
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
		
		//キャッシュファイルを全て削除
		public void OnClickDeleteAllCacheFiles()
		{
			AssetFileManager.DeleteCacheFileAll();
		}

		//言語切り替え
		public void OnClickChangeLanguage()
		{
			LanguageManagerBase langManager = LanguageManagerBase.Instance;
			if (langManager == null) return;
			if (langManager.Languages.Count < 1) return;

			//言語をシフトループ
			int index = langManager.Languages.IndexOf(langManager.CurrentLanguage);
			langManager.CurrentLanguage = langManager.Languages[(index+1) % langManager.Languages.Count];
		}
	}
}