//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	/// <summary>
	/// コンバートしたファイル（アセットバンドルや独自符号ファイル）の管理マネージャー
	/// </summary>
	[AddComponentMenu("Utage/Lib/File/ConvertFileListManager")]
	public class ConvertFileListManager : MonoBehaviour
	{
		//サーバー上のファイルリストのURL
		[SerializeField]
		List<string> serverUrlList = new List<string>();

		//ローカルのファイルリストのパス
		[SerializeField]
		List<string> localPathList = new List<string>();

		//ファイルリストの情報
		List<ConvertFileList> infoLists = new List<ConvertFileList>();

		List<AssetFile> files = new List<AssetFile>();

		//ファイルのパスから、ファイル情報を取得
		public bool TryGetValue(string filePath, AssetFileEncodeType encodeType, out ConvertFileInfo info)
		{
			info = null;
			foreach(ConvertFileList infoList in infoLists)
			{
				if (infoList.TryGetValue(filePath, encodeType, out info))
				{
					return true;
				}
			}
			return false;
		}

		//指定のパスのファイルリストをロード
		public void LoadStart(List<string> pathList, int version, System.Action onComplete = null)
		{
			foreach (string path in pathList)
			{
				if (!string.IsNullOrEmpty(path))
				{
					if (FilePathUtil.IsAbsoluteUri(path))
					{
						serverUrlList.Add(path);
					}
					else
					{
						localPathList.Add(path);
					}
				}
			}
			LoadStart(version,onComplete);
		}

		//設定されたファイルリストをロード
		public void LoadStart(int version, System.Action onComplete = null)
		{
			StartCoroutine(CoLoad(version,onComplete));
		}

		//ロード中か
		public bool IsLoading
		{
			get
			{
				foreach (AssetFile file in files)
				{
					if (!file.IsLoadEnd) return true;
				}
				return false;
			}
		}

		//初期化済みか
		public bool IsInitialized { get; protected set; }

		//ロード処理
		IEnumerator CoLoad(int version, System.Action onComplete)
		{
			foreach (string path in localPathList)
			{
				files.Add(AssetFileManager.Load(FilePathUtil.ToStreamingAssetsPath(path), this));
			}
			foreach (string url in serverUrlList)
			{
				files.Add(AssetFileManager.Load(url, version, this));
			}

			while (IsLoading) yield return 0;

			foreach (AssetFile file in files)
			{
				ConvertFileList infoList = new ConvertFileList(file.FileName);
				BinaryUtil.BinaryRead(file.Bytes, infoList.Read);
				infoLists.Add(infoList);
				file.Unuse(this);
			}
			files.Clear();
			IsInitialized = true;
			if (onComplete!=null) onComplete();
		}
	}
}
