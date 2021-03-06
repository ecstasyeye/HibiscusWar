//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Utage
{

	/// <summary>
	/// ファイル管理
	/// </summary>
	public partial class AssetFileManager : MonoBehaviour
	{
		//エディター上でビュワー描画として呼び出す
		static public void OnGuiViwer()
		{
			GetInstance().OnGuiViwerInEditor();
		}

		void OnGuiViwerInEditor()
		{
			foreach (var keyValue in fileTbl)
			{
				OnGuiFile(keyValue.Key, keyValue.Value);
			}
		}

		GUILayoutOption paramWidth0 = GUILayout.Width(96);
//		GUILayoutOption paramWidth1 = GUILayout.Width(96);
		GUIStyle style = GUIStyle.none;
		void OnGuiFile(string name, AssetFile file)
		{
			Status status = GetFileStatus(file);
			EditorGUILayout.BeginHorizontal();
			{
				style.normal.textColor = GetStatusColor(status);
				style.richText = true;
				GUILayout.Label(status.ToString(), style, paramWidth0);

				GUILayout.Label("" + file.Version, paramWidth0);
				GUILayout.Label(name);
			}
			EditorGUILayout.EndHorizontal();
		}

		enum Status
		{
			WaitingToLoad,
			Loading,
			Using,
			Pooling,
			NotLoaded,
			NeedsToCache,
		}
		Status GetFileStatus(AssetFile file)
		{
			if (loadingFileList.Contains(file)) return Status.Loading;
			if (loadWaitFileList.Contains(file)) return Status.WaitingToLoad;
			if (usingFileList.Contains(file)) return Status.Using;
			if (unuesdFileList.Contains(file)) return Status.Pooling;
			if (file.NeedsToCache)
			{
				return Status.NeedsToCache;
			}
			else
			{
				return Status.NotLoaded;
			}
		}

		Color GetStatusColor(Status status)
		{
			switch (status)
			{
				case Status.WaitingToLoad:
					return ColorUtil.Magenta;
				case Status.Loading:
					return ColorUtil.Red;
				case Status.Using:
					return ColorUtil.Green;
				case Status.Pooling:
					return ColorUtil.Lime;
				case Status.NotLoaded:
				default:
					return ColorUtil.Grey;
			}
		}
	}
}
#endif
