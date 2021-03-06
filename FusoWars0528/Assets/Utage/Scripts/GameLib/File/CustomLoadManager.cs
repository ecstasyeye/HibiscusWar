//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Utage
{
	/// <summary>
	/// ロード処理を独自カスタムするためのマネージャー
	/// </summary>
	[AddComponentMenu("Utage/Lib/File/CustomLoadManager")]
	public class CustomLoadManager : MonoBehaviour
	{
		public AssetFile Find(string filePath, AssetFileManagerSettings settings, StringGridRow rowData)
		{
			if (OnFindAsset != null)
			{
				AssetFile asset = null;
				OnFindAsset(filePath, settings, rowData, ref asset);
				if (asset != null) return asset;
			}
			return null;
		}

		public delegate void FindAsset(string filePath, AssetFileManagerSettings settings, StringGridRow rowData, ref AssetFile asset);
		public FindAsset OnFindAsset{ get; set; }
	}
}
