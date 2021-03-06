//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{
	public class ButtonEventInfo
	{
		public ButtonEventInfo(string text, UnityAction callBackClicked)
		{
			this.text = text;
			this.callBackClicked = callBackClicked;
		}
		public string text;
		public UnityAction callBackClicked;
	};

	/// ダイアログを開イベント
	[System.Serializable]
	public class OpenDialogEvent : UnityEvent<string, List<ButtonEventInfo>> { }

	/// 1ボタンダイアログを開イベント
	[System.Serializable]
	public class Open1ButtonDialogEvent : UnityEvent<string,ButtonEventInfo> { }

	/// 2ボタンダイアログを開イベント
	[System.Serializable]
	public class Open2ButtonDialogEvent : UnityEvent<string, ButtonEventInfo, ButtonEventInfo> { }

	/// 3ボタンダイアログを開イベント
	[System.Serializable]
	public class Open3ButtonDialogEvent : UnityEvent<string, ButtonEventInfo, ButtonEventInfo, ButtonEventInfo> { }
}