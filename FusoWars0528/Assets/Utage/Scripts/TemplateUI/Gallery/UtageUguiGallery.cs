//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Utage;
using System.Collections.Generic;

/// <summary>
/// ギャラリー表示のサンプル
/// </summary>
[AddComponentMenu("Utage/TemplateUI/Gallery")]
public class UtageUguiGallery : UguiView
{
	public UguiView[] views;

	//一時的に表示オフ
	public void Sleep()
	{
		this.gameObject.SetActive(false);
	}

	//一時的な表示オフを解除
	public void WakeUp()
	{
		this.gameObject.SetActive(true);
	}

	public void OnTabIndexChanged( int index )
	{
		if (index >= views.Length)
		{
			Debug.LogError("index < views.Length");
			return;
		}
		for( int i = 0; i < views.Length; ++i )
		{
			if (i == index) continue;
			views[i].ToggleOpen(false);
		}
		views[index].ToggleOpen(true);
	}
}
