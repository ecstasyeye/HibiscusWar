//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
/*
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{

	//プロジェクトのパッケージを全て出力する
	public class ExportProjectPackage
	{
		/// <summary>
		/// セーブデータフォルダを開く
		/// </summary>
		[MenuItem(MeuToolOpen.MeuToolRoot + "Tools/Export Project Package", priority = 45)]
		static void Open()
		{
			string path = EditorUtility.SaveFilePanel("Export Project Package...", "../", "", "unitypackage");
			if (!string.IsNullOrEmpty(path))
			{
				AssetDatabase.ExportPackage("Assets", path,
					ExportPackageOptions.Recurse | ExportPackageOptions.Interactive | ExportPackageOptions.IncludeLibraryAssets);
			}
		}
	}
}
*/