//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	/// <summary>
	/// 宴形式のアセット情報の管理
	/// </summary>
	[System.Serializable]
	internal class AssetFileUtageManager : SerializableDictionary<AssetFileInfo>
	{
		int cacheFileID;		//最新のキャッシュファイルのID

		AssetFileManager AssetFileManager { get; set; }
		FileIOManager FileIOManger { get { return AssetFileManager.FileIOManger; } }

		/// キャッシュファイルのルートディレクトリ
		string CacheRootDir { get; set; }
		/// キャッシュファイル管理テーブルのファイルパス
		string CacheTblPath { get; set; }


		internal AssetFileUtageManager(AssetFileManager assetFileManager)
		{
			AssetFileManager = assetFileManager;
			CacheRootDir = FilePathUtil.Combine(FileIOManager.SdkTemporaryCachePath, AssetFileManager.cacheDirectoryName);
			CacheTblPath = FilePathUtil.Combine(FileIOManager.SdkTemporaryCachePath, AssetFileManager.cacheTblFileName);
		}

		//起動時初期化
		internal void BootInit(AssetFileManagerSettings settings, ConvertFileListManager convertFileListManager)
		{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			this.Clear();
			if (!AssetFileManager.DontUseCache)
			{
				if (!FileIOManger.ReadBinaryDecode(CacheTblPath, (reader) => Read(reader, settings, convertFileListManager)))
				{
					this.Clear();
				}
			}
#endif
			if (AssetFileManager.isDebugBootDeleteChacheAll)
			{
				DeleteCacheFileAllSub();
			}
			else if (AssetFileManager.isDebugBootDeleteChacheTextAndBinary)
			{
				DeleteCacheFileAllSub(AssetFileType.Text);
				DeleteCacheFileAllSub(AssetFileType.Bytes);
				DeleteCacheFileAllSub(AssetFileType.Csv);
			}
		}

		internal void SetConvertFileInfo(ConvertFileListManager convertFileListManager)
		{
			foreach (var fileInfo in Values)
			{
				fileInfo.SetConvertFileInfo(convertFileListManager);
			}
		}

		/// <summary>
		/// キャッシュIDを加算
		/// </summary>
		/// <returns>加算後のキャッシュID</returns>
		int IncCacheID()
		{
			return ++cacheFileID;
		}

		//キャッシュデータを全て削除
		void DeleteCacheAll()
		{
			foreach (AssetFileInfo fileInfo in this.List)
			{
				//キャッシュファイル削除
				fileInfo.DeleteCache();
			}
			cacheFileID = 0;
		}

		//キャッシュファイル書き込み
		internal void WriteNewVersion(AssetFileUtage file)
		{
			if (AssetFileManager.DontUseCache) return;
			if (AssetFileManager.isOutPutDebugLog) Debug.Log("WriteCacheFile:" + file.FileName + " ver:" + file.Version + " cache:" + file.CacheVersion);

			//キャッシュファイル書き込み準備
			file.ReadyToWriteCache(IncCacheID(), CacheRootDir, AssetFileManager.isDebugCacheFileName);
			string cachePath = file.CachePath;

			//キャッシュ用のディレクトリがなければ作成
			FileIOManger.CreateDirectory(cachePath);

			//ファイル書き込み
			bool ret = false;
			if (file.EncodeType == AssetFileEncodeType.EncodeOnCache)
			{
				switch (file.FileType)
				{
					case AssetFileType.Sound:
						ret = FileIOManger.WriteSound(cachePath, file.WriteCacheFileSound);
						break;
					case AssetFileType.Texture:
						ret = FileIOManger.WriteEncodeNoCompress(cachePath, file.CacheWriteBytes);
						break;
					default:
						ret = FileIOManger.WriteEncode(cachePath, file.CacheWriteBytes);
						break;
				}
			}
			else
			{
				ret = FileIOManger.Write(cachePath, file.CacheWriteBytes);
			}

			WrapperUnityVersion.SetNoBackupFlag(cachePath);

			//キャッシュファイルテーブルを更新して上書き
			if (!ret)
			{
				if (AssetFileManager.isOutPutDebugLog) Debug.LogError("Write Failed :" + file.CachePath);
			}
			else
			{
				WriteCacheTbl();
				file.DeleteOldCacheFile();
			}
		}


		//キャッシュファイルテーブルを保存
		void WriteCacheTbl()
		{
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			//キャッシュ用のディレクトリがなければ作成
			string path = CacheTblPath;
			FileIOManger.CreateDirectory(path);
			FileIOManger.WriteBinaryEncode(path, Write);

			WrapperUnityVersion.SetNoBackupFlag(path);

#endif
		}

		//	キャッシュファイルを削除
		internal void DeleteCacheFileSub(string path)
		{
			AssetFileInfo fileInfo;
			if (TryGetValue(path, out fileInfo))
			{
				//キャッシュファイル削除
				fileInfo.DeleteCache();
				Remove(path);
			}
			WriteCacheTbl();
		}
		//	指定タイプのキャッシュファイルを全て削除
		internal void DeleteCacheFileAllSub(AssetFileType type)
		{
			List<string> removeFile = new List<string>();
			foreach (string key in Keys)
			{
				AssetFileInfo fileInfo = GetValue(key);
				if (fileInfo.FileType == type)
				{
					removeFile.Add(key);
				}
			}
			foreach (string key in removeFile)
			{
				AssetFileInfo fileInfo = GetValue(key);
				//キャッシュファイル削除
				fileInfo.DeleteCache();
				Remove(key);
			}
			WriteCacheTbl();
		}

		//	キャッシュファイルを全て削除
		internal void DeleteCacheFileAllSub()
		{
			foreach (AssetFileInfo fileInfo in this.List)
			{
				//キャッシュファイル削除
				fileInfo.DeleteCache();
			}
			cacheFileID = 0;
			Clear();
			WriteCacheTbl();
		}


		static readonly int MagicID = FileIOManagerBase.ToMagicID('C', 'a', 'c', 'h');	//識別ID
		const int Version = 2;	//キャッシュ情報のファイルバージョン
		const int Version1 = 1;	//キャッシュ情報のファイルバージョン

		/// <summary>
		/// キャッシュデータテーブルをバイナリ読み込み
		/// </summary>
		/// <param name="reader">バイナリリーダー</param>
		void Read(BinaryReader reader, AssetFileManagerSettings settings, ConvertFileListManager convertFileListManager)
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
		void Write(BinaryWriter writer)
		{
			writer.Write(MagicID);
			writer.Write(Version);
			writer.Write(cacheFileID);
			int cacheCount = 0;
			foreach (AssetFileInfo info in List)
			{
				if (info.ExistCahce)
				{
					++cacheCount;
				}
			}
			writer.Write(cacheCount);
			foreach (AssetFileInfo info in List)
			{
				if (info.ExistCahce)
				{
					info.Write(writer);
				}
			}
		}

		internal AssetFileUtage CreateFile(string path, AssetFileManagerSettings settings, StringGridRow rowData, ConvertFileListManager convertFileListManager)
		{
			//ファイル情報を取得or作成
			AssetFileInfo fileInfo;
			if (!TryGetValue(path, out fileInfo))
			{
				if (string.IsNullOrEmpty(path))
				{
					Debug.LogError(path);
				}
				fileInfo = new AssetFileInfo(path, settings, convertFileListManager);
				this.Add(fileInfo);
			}
			//ロードファイルクラスを作成
			return new AssetFileUtage(this, fileInfo, rowData, FileIOManger);
		}
	};
}
