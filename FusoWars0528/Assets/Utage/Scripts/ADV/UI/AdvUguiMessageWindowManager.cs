//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{
	/// <summary>
	/// メッセージウィンドウの管理
	/// </summary>
	[AddComponentMenu("Utage/ADV/MessageWindowManager")]
	public class AdvUguiMessageWindowManager : MonoBehaviour
	{
		internal void Close()
		{
			this.gameObject.SetActive(false);
		}

		internal void Open()
		{
			this.gameObject.SetActive(true);
		}
	}
}