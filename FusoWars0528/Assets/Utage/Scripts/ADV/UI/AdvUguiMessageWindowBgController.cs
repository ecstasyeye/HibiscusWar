//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Utage;
using System.Collections.Generic;
/*
/// <summary>
/// メッセージウィドウ背景の制御
/// </summary>
[AddComponentMenu("Utage/ADV/UiMessageWindowBgController")]
public class AdvUguiMessageWindowBgController : MonoBehaviour
{
	[SerializeField]
	string firstType = "";

	[SerializeField]
	List<GameObject> bgList;

	string currentType = "";

	void Start()
	{
		ChangeWindowTypeSub(firstType);
	}

	public void ChangeWindowType(string type)
	{
		if (currentType == type) return;
		ChangeWindowTypeSub(type);
	}

	void ChangeWindowTypeSub(string type)
	{
		if(bgList.Count == 0) return;

		currentType = type;
		//指定の名前のオブジェクトだけActiveをTrueにする
		bool isFind = false;
		foreach (GameObject bg in bgList)
		{
			bool check =  (bg.name == type);
			bg.SetActive(check);
			isFind |= check;
		}
		//指定の名前のオブジェクトがなかったら先頭オブジェクトをtrueにする
		if (!isFind) bgList[0].SetActive(true);
	}
}

*/
