//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Utage
{

	//「Utage」のファイルリストコンバーター
	public class AdvFileListConverter
	{
		public string OutputResourcesPath { get; protected set; }

		//ファイルの入出力に使う
		public FileIOManager FileIOManager { get; protected set; }
		//ファイルのロード設定
		public AssetFileManagerSettings Settings { get; protected set; }
		
		public ConvertFileList ConvertFileList { get; protected set; }

		public AdvFileListConverter(string outPutDirectoryPath, FileIOManager fileIOManager, AssetFileManagerSettings settings)
		{
			OutputResourcesPath = outPutDirectoryPath;
			FileIOManager = fileIOManager;
			Settings = settings;
		}

		//リソースをコンバートしてバージョンアップする
		public void VersionUp(System.Action<AdvFileListConverter> CallbackConvertFiles)
		{
			this.Read();
			CallbackConvertFiles(this);
			this.Write();
		}

		//コンバートファイルリストのファイルを読み込む
		void Read()
		{
			//出力先のアセットバンドル情報を読み込む
			string convertFileListPath = FilePathUtil.Combine( OutputResourcesPath,Path.GetFileNameWithoutExtension(OutputResourcesPath) + ExtensionUtil.ConvertFileList);
			bool isEncoded = Settings.FindSettingFromPath(convertFileListPath).EncodeType == AssetFileEncodeType.AlreadyEncoded;
			if (isEncoded) convertFileListPath += ExtensionUtil.UtageFile;

			ConvertFileList = new ConvertFileList(convertFileListPath);
			//ファイルから読み込む
			if (File.Exists(convertFileListPath))
			{
				byte[] bytes = File.ReadAllBytes(convertFileListPath);
				if (isEncoded)
				{
					bytes = FileIOManager.Decode(bytes);
				}
				BinaryUtil.BinaryRead(bytes, ConvertFileList.Read);
			}
		}

		//コンバートファイルリストのファイルを書き込む
		void Write()
		{
			byte[] bytes = BinaryUtil.BinaryWrite(ConvertFileList.Write);
			bool isEncoded = Settings.FindSettingFromPath(ConvertFileList.FilePath).EncodeType == AssetFileEncodeType.AlreadyEncoded;

			if (isEncoded)
			{
				bytes = FileIOManager.Encode(bytes);
			}
			File.WriteAllBytes(ConvertFileList.FilePath, bytes);
		}
		
		//ログファイルを書き込む
		public void WriteLog(bool isAssetBundle)
		{
			string logFileListPath = FilePathUtil.Combine( OutputResourcesPath, Path.GetFileNameWithoutExtension(OutputResourcesPath) + ExtensionUtil.ConvertFileListLog);
			logFileListPath += ExtensionUtil.Txt;
			File.WriteAllText(logFileListPath, ConvertFileList.ToLogString(isAssetBundle));
		}
		
		//リソースファイルをバージョンアップ
		public void VersionUpResouces(string keyFiles, Object resourcesDir, bool isOnlyNew)
		{
			//指定ディレクトリ以下のアセットを全て取得
			MainAssetInfo inputDirAsset = new MainAssetInfo(resourcesDir);
			List<MainAssetInfo> assets = inputDirAsset.GetAllChidren();

			string inputDir = inputDirAsset.AssetPath;

			List<ConvertFileList.CusomFileVersionUpInfo> customFileVersionUpInfoList = new List<ConvertFileList.CusomFileVersionUpInfo>();
			int convertFileCount = 0;
			int versionUpCount = 0;
			foreach (MainAssetInfo asset in assets)
			{
				if (asset.IsDirectory) continue;
				AssetFileSetting assetFileSetting = Settings.FindSettingFromPath(asset.AssetPath);
				AssetFileEncodeType encodeType = assetFileSetting.EncodeType;
				switch (encodeType)
				{
					case AssetFileEncodeType.AlreadyEncoded:
						{
							++convertFileCount;
							bool isVersionUp = TryConvertToUtageFile(asset, inputDir, isOnlyNew);
							if (isVersionUp)
							{
								++versionUpCount;
							}
							string fileKey = FilePathUtil.RemoveDirectory(asset.AssetPath, inputDir);
							customFileVersionUpInfoList.Add(new ConvertFileList.CusomFileVersionUpInfo(fileKey, isVersionUp));
						}
						break;
					default:
						break;
				}
			}
			ConvertFileList.EditorVersionUp(keyFiles,customFileVersionUpInfoList);

			Debug.Log(string.Format("{0}/{1}files version up", versionUpCount, convertFileCount));
		}

		public bool TryVersionUpFileFromMem(string fileKey, byte[] buffer, out ConvertFileList.CusomFileVersionUpInfo versionUpInfo)
		{
			string outputFileName = FilePathUtil.Combine( OutputResourcesPath,fileKey);

			AssetFileSetting setting = Settings.FindSettingFromPath(outputFileName);
			if (setting.EncodeType == AssetFileEncodeType.AlreadyEncoded) outputFileName += ExtensionUtil.UtageFile;

			bool isVersionUp = TryConvertFileFromMem(outputFileName, buffer);
			versionUpInfo = new ConvertFileList.CusomFileVersionUpInfo(fileKey, isVersionUp);
			return isVersionUp;
		}

		//宴独自形式へのコンバート
		bool TryConvertToUtageFile(MainAssetInfo asset, string inputDir, bool isOnlyNew)
		{
			string outputFileName = ToOutPutFilePath(asset.AssetPath, inputDir);
			AssetFileSetting setting = Settings.FindSettingFromPath(outputFileName);
			if (setting.EncodeType == AssetFileEncodeType.AlreadyEncoded) outputFileName += ExtensionUtil.UtageFile;

			//新しいファイルのみコピー
			if (isOnlyNew)
			{
				if (File.Exists(outputFileName) && File.GetLastWriteTime(asset.FullPath) <= File.GetLastWriteTime(outputFileName))
				{
					return false;
				}
			}

			return TryConvertFileFromMem( outputFileName, File.ReadAllBytes(asset.FullPath) );
		}

		//指定のパスのファイルをメモリからバージョンアップ。
		//ファイルとメモリが同じだったら、書き込まずfalseを返す。
		bool TryConvertFileFromMem(string outputFileName, byte[] buffer)
		{
			AssetFileSetting setting = Settings.FindSettingFromPath(outputFileName);

			byte[] oldBytes = File.Exists(outputFileName) ? File.ReadAllBytes(outputFileName) : new byte[0];
			if (setting.EncodeType == AssetFileEncodeType.AlreadyEncoded)
			{
				//エンコードが必要なタイプはエンコードする
				switch (setting.FileType)
				{
					case AssetFileType.Sound:
						Debug.LogError("Sound File not support to encode file");
						return false;
					case AssetFileType.Texture:
						buffer = FileIOManager.EncodeNoCompress(buffer);
						break;
					default:
						buffer = FileIOManager.Encode(buffer);
						break;
				}
			}
			if (IsEqualBytes(oldBytes, buffer))
			{
				return false;
			}
			else
			{
				//ディレクトリがなければ作る
				string dir = Path.GetDirectoryName(outputFileName);
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				File.WriteAllBytes(outputFileName, buffer);
				return true;
			}
		}

		bool IsEqualBytes( byte[] bytes1, byte[] bytes2)
		{
			int len = bytes1.Length;
			if ( len!= bytes2.Length)
				return false;
			for (int i = 0; i < len; ++i)
			{
				if( bytes1[i] != bytes2[i] ) return false;
			}
			return true;
		}

		string ToOutPutFilePath( string assetPath, string inputDir )
		{
			return assetPath.Replace(inputDir, OutputResourcesPath);
		}

	}
}