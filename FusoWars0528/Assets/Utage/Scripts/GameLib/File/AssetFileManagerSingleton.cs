//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// ファイル管理
	/// </summary>
	public partial class AssetFileManager : MonoBehaviour
	{
		/// <summary>
		/// 割り当てる最大メモリサイズ
		/// </summary>
		static public int MaxMemSize { get { return (int)(GetInstance().maxMemSizeMB * 1024 * 1024); } }

		/// <summary>
		/// 最適化後のメモリサイズ
		/// </summary>
		static public int OptimizedMemSize { get { return (int)GetInstance().optimizedMemSizeMB * 1024 * 1024; } }

		/// <summary>
		/// ロード済みファイル（使用中とプール中の両方）の総メモリサイズ
		/// </summary>
		static public int TotalMemSize { get { return GetInstance().totalMemSize; } }

		/// <summary>
		/// 使用中ファイルの総メモリサイズ
		/// </summary>
		static public int TotalMemSizeUsing { get { return GetInstance().totalMemSizeUsing; } }

		//キャッシュファイルを使用しない
		//起動後にチェックのオン、オフをしてはいけない
		static public void SetDonUseCache(bool dontUseCache)
		{
			GetInstance().dontUseCache = dontUseCache;
		}

		/// <summary>
		/// ファイルリストをロードして初期化する
		/// </summary>
		static public void LoadInitFileList(List<string> pathList, int version)
		{
			GetInstance().LoadInitFileListSub(pathList, version);
		}

		//初期化が終わっているか
		internal static bool IsInitialized()
		{
			return GetInstance().ConvertFileListManager.IsInitialized;
		}

		/// <summary>
		/// ロード設定を初期化（ゲーム起動直後にすること）
		/// </summary>
		static public void InitLoadTypeSetting(AssetFileManagerSettings.LoadType loadTypeSetting)
		{
			GetInstance().Settings.LoadTypeSetting = loadTypeSetting;
		}
		
		/// <summary>
		/// エラー処理の設定
		/// </summary>
		/// <param name="callbackError">エラー時に呼ばれるコールバック</param>
		static public void InitError(Action<AssetFile> callbackError)
		{
			GetInstance().CallbackError = callbackError;
		}

		/// <summary>
		/// ファイル情報取得
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <returns>ファイル情報</returns>
		static public AssetFile GetFileCreateIfMissing(string path, StringGridRow settingData = null)
		{

			if (!IsEditorErrorCheck)
			{
				AssetFile file = GetInstance().AddSub(path, settingData);
				return file;
			}
			else
			{
				if (path.Contains(" "))
				{
					Debug.LogWarning("[" + path + "] contains white space");
				}
//				AssetFileWork dummy = new AssetFileWork();
				return null;
			}
		}

		/// <summary>
		/// ファイルのロード
		/// すぐ使うファイルに使用すること
		/// ロードの優先順位は一番高い
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <param name="referenceObj">ファイルを参照するオブジェクト</param>
		/// <returns>ファイル情報</returns>	
		static public AssetFile Load(string path, System.Object referenceObj)
		{
			return Load(GetInstance().AddSub(path), referenceObj);
		}
		/// <summary>
		/// ファイルのロード
		/// すぐ使うファイルに使用すること
		/// ロードの優先順位は一番高い
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <param name="referenceObj">ファイルを参照するオブジェクト</param>
		/// <returns>ファイル情報</returns>	
		static public AssetFile Load(string path, int version, System.Object referenceObj)
		{
			AssetFile file = GetInstance().AddSub(path);
			file.Version = version;
			return Load(file, referenceObj);
		}
		/// <summary>
		/// ファイルのロード
		/// すぐ使うファイルに使用すること
		/// ロードの優先順位は一番高い
		/// </summary>
		/// <param name="file">ロードするファイル</param>
		/// <param name="referenceObj">ファイルを参照するオブジェクト</param>
		/// <returns>ファイル情報</returns>	
		static public AssetFile Load(AssetFile file, System.Object referenceObj)
		{
			return GetInstance().LoadSub(file, referenceObj);
		}

		/// <summary>
		/// ファイルの先行ロード
		/// もうすぐ使うファイルに使用すること
		/// ロードの優先順位は二番目に高い
		/// 事前にロードをかけてロード時間を短縮しておくのが主な用途
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <param name="referenceObj">ファイルを参照するオブジェクト</param>
		static public void Preload(string path, System.Object referenceObj)
		{
			Preload(GetInstance().AddSub(path), referenceObj);
		}

		/// <summary>
		/// ファイルの先行ロード
		/// もうすぐ使うファイルに使用すること
		/// ロードの優先順位は二番目に高い
		/// 事前にロードをかけてロード時間を短縮しておくのが主な用途
		/// </summary>
		/// <param name="file">先行ロードするファイル</param>
		/// <param name="referenceObj">ファイルを参照するオブジェクト</param>
		static public void Preload(AssetFile file, System.Object referenceObj)
		{
			GetInstance().PreloadSub(file, referenceObj);
		}

		/// <summary>
		/// ファイルのバックグラウンドロード
		/// すぐには使わないが、そのうち使うであろうファイルに使用すること
		/// ロードの優先順位は三番目に高い
		/// 事前にロードをかけてロード時間を短縮しておくのが主な用途。
		/// </summary>
		/// <param name="file">ファイルパス</param>
		/// <param name="referenceObj">ファイルを参照するオブジェクト</param>
		static public AssetFile BackGroundLoad(string path, System.Object referenceObj)
		{
			return BackGroundLoad(GetInstance().AddSub(path), referenceObj);
		}
		/// <summary>
		/// ファイルのバックグラウンドロード
		/// すぐには使わないが、そのうち使うであろうファイルに使用すること
		/// ロードの優先順位は三番目に高い
		/// 事前にロードをかけてロード時間を短縮しておくのが主な用途。
		/// </summary>
		/// <param name="file">バックグラウンドロードするファイル</param>
		/// <param name="referenceObj">ファイルを参照するオブジェクト</param>
		static public AssetFile BackGroundLoad(AssetFile file, System.Object referenceObj)
		{
			return GetInstance().BackGroundLoadSub(file, referenceObj);
		}


		/// <summary>
		/// ファイルのダウンロードだけする
		/// ロードの優先順位は一番低い
		/// バックグラウンドでファイルのダウンロードをする。
		/// デバイスストレージに保存可能ならファイルをキャッシュしておく
		/// ロードしたアセットはメモリにもキャッシュにもしておくが
		/// メモリキャッシュはメモリが枯渇すると揮発するので、その場合は再ロードに時間がかかる
		/// </summary>
		/// <param name="path">パス</param>	
		static public void Download(string path)
		{
			Download(GetInstance().AddSub(path));
		}

		/// <summary>
		/// ファイルのダウンロードだけする
		/// ロードの優先順位は一番低い
		/// バックグラウンドでファイルのダウンロードをする。
		/// デバイスストレージに保存可能ならファイルをキャッシュしておく
		/// ロードしたアセットはメモリにもキャッシュにもしておくが
		/// メモリキャッシュはメモリが枯渇すると揮発するので、その場合は再ロードに時間がかかる
		/// </summary>
		/// <param name="file">ダウンロードするファイル</param>
		static public void Download(AssetFile file)
		{
			GetInstance().DownloadSub(file);
		}

		/// <summary>
		/// キャッシュファイルを削除
		/// </summary>
		/// <param name="path">削除するキャッシュファイルのパス</param>	
		static public void DeleteCacheFile(string path)
		{
			GetInstance().AssetFileUtageManager.DeleteCacheFileSub(path);
		}

		/// <summary>
		/// 指定タイプのキャッシュファイルを全て削除
		/// </summary>
		/// <param name="type">削除するキャッシュファイルのタイプ</param>
		static public void DeleteCacheFileAll(AssetFileType type)
		{
			GetInstance().AssetFileUtageManager.DeleteCacheFileAllSub(type);
		}

		/// <summary>
		/// キャッシュファイルを全て削除
		/// </summary>
		static public void DeleteCacheFileAll()
		{
			GetInstance().AssetFileUtageManager.DeleteCacheFileAllSub();
		}

		//ロード終了チェック
		internal static bool IsLoadEnd()
		{
			return GetInstance().IsLoadEnd(AssetFileLoadPriority.Preload);
		}
		//ダウンロード終了チェック
		internal static bool IsDownloadEnd()
		{
			return GetInstance().IsLoadEnd(AssetFileLoadPriority.DownloadOnly);
		}

		//ロード待ちのファイル数
		internal static int CountLoading()
		{
			return GetInstance().CountLoading(AssetFileLoadPriority.Preload);
		}

		//ロード待ちのファイル数
		internal static int CountDownloading()
		{
			return GetInstance().CountLoading(AssetFileLoadPriority.DownloadOnly);
		}

		//拡張子を追加
		internal static void AddAssetFileTypeExtensions(AssetFileType type, string[] extensions)
		{
			GetInstance().Settings.AddExtensions(type, extensions);
		}

		//static なアセットがあるか
		public static bool ContainsStaticAsset(UnityEngine.Object asset)
		{
			return GetInstance ().StaticAssetManager.Contains (asset);
		}

		//CustomLoadManagerを取得
		public static CustomLoadManager GetCustomLoadManager()
		{
			return GetInstance().CustomLoadManager;
		}

		//ロードエラーコールバックを設定
		static public void SetLoadErrorCallBack(Action<AssetFile> callbackError)
		{
			GetInstance().callbackError = callbackError;
		}

		//ファイルのリロード
		static public void ReloadFile(AssetFile file)
		{
			GetInstance().ReloadFileSub(file);
		}

		/// <summary>
		/// エディタ上のエラーチェックのために起動してるか
		/// </summary>
		static public bool IsEditorErrorCheck
		{
			get { return isEditorErrorCheck; }
			set { isEditorErrorCheck = value; }
		}
		static bool isEditorErrorCheck = false;

		static AssetFileManager instance;
		static AssetFileManager GetInstance()
		{
			if (instance==null)
			{
				instance = FindObjectOfType<AssetFileManager>();
				if(instance==null)
				{
					GameObject go = new GameObject("FileManager");
					instance = go.AddComponent<AssetFileManager>();
				}
			}
			return instance;
		}
	}
}