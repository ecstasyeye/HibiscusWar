//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// ゲームの起動設定データ
	/// </summary>
	[System.Serializable]
	public partial class AdvBootSetting
	{

		[System.Serializable]
		public class DefaultDirInfo
		{
			public string defaultDir;		//デフォルトのディレクトリ
			public string defaultExt;		//デフォルトの拡張子

			public string FileNameToPath(string fileName)
			{
				return FileNameToPath(fileName, "");
			}
		
			public string FileNameToPath(string fileName, string LocalizeDir)
			{
				if (string.IsNullOrEmpty(fileName)) return fileName;

				string path;
				//既に絶対URLならそのまま
				if (FilePathUtil.IsAbsoluteUri(fileName))
				{
					path = fileName;
				}
				else
				{
					try
					{
						//拡張子がなければデフォルト拡張子を追加
						if (string.IsNullOrEmpty(FilePathUtil.GetExtension(fileName)))
						{
							fileName += defaultExt;
						}
						path = defaultDir + LocalizeDir + "/" + fileName;
					}
					catch (System.Exception e)
					{
						Debug.LogError(fileName + "  " + e.ToString());
						path = defaultDir + LocalizeDir + "/" + fileName;
					}
				}

				//プラットフォームが対応する拡張子にする(mp3とoggを入れ替え)
				return ExtensionUtil.ChangeSoundExt(path);
			}
		};

		public string ResorceDir { get { return resorceDir; } }		//リソースのルートディレクトリ
		string resorceDir;

		/// <summary>
		/// キャラクターテクスチャのパス情報
		/// </summary>
		public DefaultDirInfo CharacterDirInfo { get { return characterDirInfo; } }
		DefaultDirInfo characterDirInfo;

		/// <summary>
		/// 背景テクスチャのパス情報
		/// </summary>
		public DefaultDirInfo BgDirInfo { get { return bgDirInfo; } }
		DefaultDirInfo bgDirInfo;

		/// <summary>
		/// イベントCGテクスチャのパス情報
		/// </summary>
		public DefaultDirInfo EventDirInfo { get { return eventDirInfo; } }
		DefaultDirInfo eventDirInfo;

		/// <summary>
		/// スプライトテクスチャのパス情報
		/// </summary>
		public DefaultDirInfo SpriteDirInfo { get { return spriteDirInfo; } }
		DefaultDirInfo spriteDirInfo;

		/// <summary>
		/// サムネイルテクスチャのパス情報
		/// </summary>
		public DefaultDirInfo ThumbnailDirInfo { get { return thumbnailDirInfo; } }
		DefaultDirInfo thumbnailDirInfo;

		/// <summary>
		/// BGMのパス情報
		/// </summary>
		public DefaultDirInfo BgmDirInfo { get { return bgmDirInfo; } }
		DefaultDirInfo bgmDirInfo;

		/// <summary>
		/// SEのパス情報
		/// </summary>
		public DefaultDirInfo SeDirInfo { get { return seDirInfo; } }
		DefaultDirInfo seDirInfo;

		/// <summary>
		/// 環境音のパス情報
		/// </summary>
		public DefaultDirInfo AmbienceDirInfo { get { return ambienceDirInfo; } }
		DefaultDirInfo ambienceDirInfo;

		/// <summary>
		/// ボイスのパス情報
		/// </summary>
		public DefaultDirInfo VoiceDirInfo { get { return voiceDirInfo; } }
		DefaultDirInfo voiceDirInfo;

	
		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="resorceDir">リソースディレクトリ</param>
		public void BootInit( string resorceDir )
		{
			this.resorceDir = resorceDir;
			characterDirInfo = new DefaultDirInfo { defaultDir = @"Texture/Character", defaultExt = ".png" };
			bgDirInfo = new DefaultDirInfo { defaultDir = @"Texture/BG", defaultExt = ".jpg" };
			eventDirInfo = new DefaultDirInfo { defaultDir = @"Texture/Event", defaultExt = ".jpg" };
			spriteDirInfo = new DefaultDirInfo { defaultDir = @"Texture/Sprite", defaultExt = ".png" };
			thumbnailDirInfo = new DefaultDirInfo { defaultDir = @"Texture/Thumbnail", defaultExt = ".jpg" };
			bgmDirInfo = new DefaultDirInfo { defaultDir = @"Sound/BGM", defaultExt = ".wav" };
			seDirInfo = new DefaultDirInfo { defaultDir = @"Sound/SE", defaultExt = ".wav" };
			ambienceDirInfo = new DefaultDirInfo { defaultDir = @"Sound/Ambience", defaultExt = ".wav" };
			voiceDirInfo = new DefaultDirInfo { defaultDir = @"Sound/Voice", defaultExt = ".wav" };


			InitDefaultDirInfo(resorceDir, characterDirInfo);
			InitDefaultDirInfo(resorceDir, bgDirInfo);
			InitDefaultDirInfo(resorceDir, eventDirInfo);
			InitDefaultDirInfo(resorceDir, spriteDirInfo);
			InitDefaultDirInfo(resorceDir, thumbnailDirInfo);
			InitDefaultDirInfo(resorceDir, bgmDirInfo);
			InitDefaultDirInfo(resorceDir, seDirInfo);
			InitDefaultDirInfo(resorceDir, ambienceDirInfo);
			InitDefaultDirInfo(resorceDir, voiceDirInfo);
		}
		void InitDefaultDirInfo(string root, DefaultDirInfo info)
		{
			info.defaultDir = FilePathUtil.Combine( root,info.defaultDir );
		}

		string FileNameToPath(string fileName, string csvDir )
		{
			//既に絶対URLならそのまま
			if (FilePathUtil.IsAbsoluteUri(fileName))
			{
				return fileName;
			}
			else
			{
				return csvDir + fileName;
			}
		}

		public string GetLocalizeVoiceFilePath( string file )
		{
			if (LanguageManagerBase.Instance.IgnoreLocalizeVoice)
			{
				return VoiceDirInfo.FileNameToPath(file);
			}
			else
			{
				string language = LanguageManagerBase.Instance.CurrentLanguage;
				return VoiceDirInfo.FileNameToPath(file, language);
			}
		}

		public static string GetTagName(string sheetName)
		{
			if (AdvSettingDataManager.IsParamSettingsSheet(sheetName))
			{
				sheetName = "Param";
			}
			return sheetName + "Setting";
		}
	}
}