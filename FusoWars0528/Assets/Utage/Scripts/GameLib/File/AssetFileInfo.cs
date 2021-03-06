//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
#if false
using System;
using System.IO;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// 管理中のファイル情報
	/// これはシステム内部で使うので外から使うことは想定していない
	/// </summary>
	[System.Serializable]
	public class AssetFileInfo : SerializableDictionaryKeyValue
	{
		/// <summary>
		/// ファイルパス
		/// </summary>
		public string FilePath { get { return filePath; } }
		string filePath;

		/// <summary>
		/// コンバートファイルの情報
		/// </summary>
		public ConvertFileInfo ConvertFileInfo { get { return convertFileInfo; } }
		ConvertFileInfo convertFileInfo;

		/// <summary>
		/// バージョン
		/// </summary>
		public int Version {
			get { return version; } 
			set
			{
				if (value < 0 || Version < 0)
				{
					//負の値が設定される場合はそれを優先に
					version = Mathf.Min(value, Version);
				}
				else
				{	//基本的には大きいバージョンを優先
					version = Mathf.Max(value, Version);
				}
			}
		}
		int version;

		/// <summary>
		/// キャッシュファイルのバージョン
		/// </summary>
		public int CacheVersion { get { return cacheVersion; } }
		int cacheVersion = -1;

		/// <summary>
		/// キャッシュパス
		/// </summary>
		public string CachePath { get { return this.cachePath; } }
		string cachePath = "";

		/// <summary>
		/// 昔のキャッシュパス
		/// </summary>
		public string OldCachePath { get { return this.oldCachePath; } }
		string oldCachePath = "";

		/// <summary>
		/// ファイルタイプ
		/// </summary>
		public AssetFileType FileType { get { return this.Setting.FileType; } }

		/// <summary>
		/// アセットバンドル
		/// </summary>
		public bool IsAssetBundle { get { return EncodeType == AssetFileEncodeType.AssetBundle; } }

		//ファイル設定
		AssetFileSetting setting;
		public AssetFileSetting Setting
		{
			get { return this.setting; }
		}
		/// <summary>
		/// ファイルのおき場所のタイプ
		/// </summary>
		public AssetFileStrageType StrageType
		{
			get { return strageType; }
		}
		AssetFileStrageType strageType;

		/// <summary>
		/// サーバーからロードするか
		/// </summary>
		public bool IsLoadFromServer
		{
			get {
				switch (StrageType)
				{
					case AssetFileStrageType.Web:
					case AssetFileStrageType.WebNocache:
						return true;
					default:
						return false;
				}
			}
		}

		/// <summary>
		/// ファイルのおき場所のタイプ
		/// </summary>
		public AssetFileEncodeType EncodeType
		{
			get { return Setting.GetRunTimeEncodeType(IsLoadFromServer); }
		}

		/// <summary>
		/// ロードフラグ
		/// </summary>
		public AssetFileLoadFlags LoadFlags { get { return this.loadFlags; } }
		AssetFileLoadFlags loadFlags;

		/// <summary>
		/// ロードフラグを追加
		/// </summary>
		public void AddLoadFlag(AssetFileLoadFlags flags)
		{
			loadFlags |= flags;
		}

		/// <summary>
		/// オーディオのタイプ
		/// </summary>
		public AudioType AudioType { get { return ExtensionUtil.GetAudioType(FilePath); } }

		/// <summary>
		/// ストリーミングにするか
		/// </summary>
		public bool IsStreamingType
		{
			get
			{
				bool isStreaming = ((loadFlags & AssetFileLoadFlags.Streaming) != AssetFileLoadFlags.None);
				bool isSound = (FileType == AssetFileType.Sound);
				bool notCrypt = (EncodeType == AssetFileEncodeType.None);
				bool webNocache = (StrageType == AssetFileStrageType.WebNocache);	//サーバーからの直接ロードは、回線切断を考慮してストリーミングをしない
				return isStreaming && isSound && notCrypt && !webNocache;
			}
		}

		/// <summary>
		/// 3Dサウンドか？
		/// </summary>
		public bool IsAudio3D { get { return (loadFlags & AssetFileLoadFlags.Audio3D) != AssetFileLoadFlags.None; } }

		/// <summary>
		/// テクスチャにミップマップを使うか？
		/// </summary>
		public bool IsTextureMipmap { get { return (loadFlags & AssetFileLoadFlags.TextureMipmap) != AssetFileLoadFlags.None; } }

		/// <summary>
		/// CSVをロードする際にTSV形式でロードするか？
		/// </summary>
		public bool IsTsv { get { return (loadFlags & AssetFileLoadFlags.Tsv) != AssetFileLoadFlags.None; } }

		/// <summary>
		/// キャッシュデータを削除
		/// </summary>
		public void DeleteCache()
		{
			if (!string.IsNullOrEmpty(CachePath))
			{
				if (System.IO.File.Exists(CachePath))
				{
					System.IO.File.Delete(CachePath);
				}
			}
			this.cacheVersion = -1;
			this.cachePath = "";
		}

		/// <summary>
		/// 古いキャッシュデータを削除
		/// </summary>
		public void DeleteOldCacheFile()
		{
			if (!string.IsNullOrEmpty(OldCachePath))
			{
				if (System.IO.File.Exists(OldCachePath))
				{
					System.IO.File.Delete(OldCachePath);
				}
			}
		}


		/// <summary>
		/// キャッシュファイルがあるか
		/// </summary>
		public bool ExistCahce
		{
			get
			{
				return (this.cacheVersion >= 0 && !string.IsNullOrEmpty(this.cachePath));
			}
		}

		/// <summary>
		/// キャシュ書き込みの準備
		/// </summary>
		/// <param name="id">キャッシュ番号</param>
		/// <param name="cacheRootDir">キャッシュのディレクトリ</param>
		/// <param name="isDebugFileName">デバッグ用のファイル名か？(ファイル名を隠蔽しないか)</param>
		/// <returns>キャッシュファイルパス</returns>
		public string ReadyToWriteCache(int id, string cacheRootDir, bool isDebugFileName)
		{
			oldCachePath = cachePath;
			//キャッシュ書き込みするものはパスを作る
			if (StrageType == AssetFileStrageType.Web)
			{
				if (isDebugFileName)
				{
					//デバッグ用に、DL元と同じファイル構成を再現
					cachePath = FilePathUtil.Combine(cacheRootDir,new Uri(FilePath).Host + new Uri(FilePath).AbsolutePath);
				}
				else
				{
					//キャッシュファイルIDで管理
					cachePath = FilePathUtil.Combine(cacheRootDir,id.ToString());
				}
				cacheVersion = Mathf.Max(0,Version);
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.NoChacheTypeFile));
				cachePath = "";
			}
			return cachePath;
		}

		/// <summary>
		/// 現在のバージョンがキャッシュされているか
		/// </summary>
		public bool IsCaching
		{
			get
			{
				if (StrageType == AssetFileStrageType.Web && Version >= 0 && !IsAssetBundle)
				{
					return cacheVersion >= Version;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// 現在のバージョンをキャッシュに書きこむ必要があるか
		/// </summary>
		public bool IsWriteNewCache
		{
			get
			{
				//IgnoreLoad
				//サウンド

				if (StrageType == AssetFileStrageType.Web && !IsAssetBundle)
				{
					return (Version < 0) || (cacheVersion < Version);
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// WWWでロードするパス
		/// </summary>
		public string LoadWWWPath
		{
			get
			{
				if (IsCaching)
				{
					return "file://" + CachePath;
				}
				else if (StrageType == AssetFileStrageType.Resources)
				{
					return FilePath;
				}
				else if (this.IsAssetBundle && ConvertFileInfo !=null)
				{
					string url = ConvertFileInfo.RuntimeLoadPath;
					url = FilePathUtil.ToCacheClearUrl(url);
					return FilePathUtil.EncodeUrl(url);
				}
				else
				{
					string url = FilePath;
					if (this.EncodeType == AssetFileEncodeType.AlreadyEncoded)
					{
						url = FilePathUtil.AddDoubleExtensiton(url, ExtensionUtil.UtageFile);
					}

					switch (StrageType)
					{
						case AssetFileStrageType.Web:
						case AssetFileStrageType.WebNocache:
							//OSのキャッシュクリアを考慮したURLを返す
							//サウンドだけ設定が効かないのでサウンドのみ無視
							if (FileType != AssetFileType.Sound)
							{
								url = FilePathUtil.ToCacheClearUrl(url);
							}
						break;
						case AssetFileStrageType.StreamingAssets:
							url = FilePathUtil.ToStreamingAssetsUrl(url);
							break;
						default:
							break;
					};
					url = FilePathUtil.EncodeUrl(url);

					if (url.Contains(" "))
					{
						Debug.LogError( FilePath + "is contains white space" );
					}
					return url;
				}
			}
		}

		public AssetFileInfo(string path, AssetFileManagerSettings settings, ConvertFileListManager convertFileListManager)
		{
			InitSub(path, settings, convertFileListManager);
		}

		void InitSub(string path, AssetFileManagerSettings settings, ConvertFileListManager convertFileListManager)
		{
			InitKey(path);
			this.filePath = Key;
			//ファイル設定を取得
			this.setting = settings.FindSettingFromPath(FilePath);
			if (FileType == AssetFileType.Csv && FilePathUtil.CheckExtentionWithOutDouble(FilePath, ExtensionUtil.TSV, ExtensionUtil.UtageFile))
			{
				loadFlags |= AssetFileLoadFlags.Tsv;
			}
			//保存場所の設定
			this.strageType = ParseStrageType();

			//コンバートファイル情報を設定
			if (convertFileListManager.IsInitialized)
			{
				SetConvertFileInfo(convertFileListManager);
			}
		}

		//コンバートファイル情報を設定
		public void SetConvertFileInfo(ConvertFileListManager convertFileListManager)
		{
			//コンバートファイル情報を取得
			if (convertFileListManager.TryGetValue(Key, EncodeType, out this.convertFileInfo))
			{
				this.Version = convertFileInfo.Version;
				if (this.Version > 0)
				{
//					Debug.Log(Key + "Version Up to " + Version);
				}
			}
			else
			{
				if (EncodeType == AssetFileEncodeType.AssetBundle)
				{
					Debug.LogError("NotFound AssetBundle path : " + filePath);
				}
			}
		}

		//ストレージタイプを解析
		AssetFileStrageType ParseStrageType()
		{
			//URLならWeb系の
			if (FilePathUtil.IsAbsoluteUri(FilePath))
			{
#if UNITY_WEBPLAYER || UNITY_WEBGL
				return AssetFileStrageType.WebNocache;
#else
				return AssetFileStrageType.Web;
#endif
			}
			else if (Setting.IsStreamingAssets)
			{
				return AssetFileStrageType.StreamingAssets;
			}
			else
			{
				return AssetFileStrageType.Resources;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="fileVersion">バイナリリーダー</param>
		public AssetFileInfo(BinaryReader reader, AssetFileManagerSettings settings, ConvertFileListManager assetBundleManager)
		{
			string key = reader.ReadString();
			this.cacheVersion = reader.ReadInt32();
			this.cachePath = reader.ReadString();
			InitSub(key, settings, assetBundleManager);
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="fileVersion">バイナリリーダー</param>
		public static AssetFileInfo ReadOld(BinaryReader reader, AssetFileManagerSettings settings, ConvertFileListManager assetBundleManager)
		{
			string key = reader.ReadString();
			reader.ReadInt32();
			AssetFileInfo info = new AssetFileInfo(key, settings, assetBundleManager);
			info.cacheVersion = reader.ReadInt32();
			info.cachePath = reader.ReadString();
			reader.ReadInt32();
			return info;
		}

		/// <summary>
		/// キャッシュデータテーブルをバイナリ書き込み
		/// </summary>
		/// <param name="writer">バイナリライター</param>
		public void Write(BinaryWriter writer)
		{
			writer.Write(Key);
			writer.Write(cacheVersion);
			writer.Write(cachePath);
		}
	}


	/// <summary>
	/// アセットファイル情報のDictionary
	/// </summary>
	[System.Serializable]
	public class AssetFileInfoDictionary : SerializableDictionary<AssetFileInfo>
	{
		int cacheFileID;		//最新のキャッシュファイルのID

		/// <summary>
		/// キャッシュIDを加算
		/// </summary>
		/// <returns>加算後のキャッシュID</returns>
		public int IncCacheID()
		{
			return ++cacheFileID;
		}

		//キャッシュデータを全て削除
		public void DeleteCacheAll()
		{
			foreach (AssetFileInfo fileInfo in this.List)
			{
				//キャッシュファイル削除
				fileInfo.DeleteCache();
			}
			cacheFileID = 0;
		}


		static readonly int MagicID = FileIOManagerBase.ToMagicID('C', 'a', 'c', 'h');	//識別ID
		const int Version = 2;	//キャッシュ情報のファイルバージョン
		const int Version1 = 1;	//キャッシュ情報のファイルバージョン

		/// <summary>
		/// キャッシュデータテーブルをバイナリ読み込み
		/// </summary>
		/// <param name="reader">バイナリリーダー</param>
		public void Read(BinaryReader reader, AssetFileManagerSettings settings, ConvertFileListManager convertFileListManager)
		{
			int magicID = reader.ReadInt32();
			if (magicID != MagicID)
			{
				throw new System.Exception("Read File Id Error");
			}

			int fileVersion = reader.ReadInt32();
			if (fileVersion == Version)
			{
				cacheFileID = reader.ReadInt32();
				int count = reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					Add(new AssetFileInfo(reader, settings, convertFileListManager));
				}
			}
			else if (fileVersion == Version1)
			{
				cacheFileID = reader.ReadInt32();
				int count = reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					Add(AssetFileInfo.ReadOld(reader, settings, convertFileListManager));
				}
			}
			else
			{
				throw new System.Exception(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, fileVersion));
			}
		}

		/// <summary>
		/// キャッシュデータテーブルをバイナリ書き込み
		/// </summary>
		/// <param name="writer">バイナリライター</param>
		public void Write(BinaryWriter writer)
		{
			writer.Write(MagicID);
			writer.Write(Version);
			writer.Write(cacheFileID);
			int cacheCount = 0;
			foreach (AssetFileInfo info in List)
			{
				if (info.ExistCahce )
				{
					++cacheCount;
				}
			}
			writer.Write(cacheCount);
			foreach (AssetFileInfo info in List)
			{
				if (info.ExistCahce )
				{
					info.Write(writer);
				}
			}
		}
	};
}
#endif
