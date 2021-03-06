//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// シナリオデータの管理
	/// </summary>
	public class AdvSettingDataManager
	{
		//インポートされた章データ
		public AdvImportScenarios ImportedScenarios { get; set; }

		//DLなどランタイムでロードした章データ
		public List<AdvChapterData> RunTimeChapters { get { return runTimeChapters; } }
		List<AdvChapterData> runTimeChapters = new List<AdvChapterData>();

		/// <summary>
		/// 基本設定データ
		/// </summary>
		public AdvBootSetting BootSetting { get { return this.bootSetting; } }
		AdvBootSetting bootSetting = new AdvBootSetting();
		/*
				/// <summary>
				/// シナリオファイル設定
				/// </summary>
				public AdvScenarioSetting ScenarioSetting { get { return this.scenarioSetting; } }
				AdvScenarioSetting scenarioSetting;
		*/
		/// <summary>
		/// キャラクターテクスチャ設定
		/// </summary>
		public AdvCharacterSetting CharacterSetting { get { return this.characterSetting; } }
		AdvCharacterSetting characterSetting = new AdvCharacterSetting();

		/// <summary>
		/// テクスチャ設定
		/// </summary>
		public AdvTextureSetting TextureSetting { get { return this.textureSetting; } }
		AdvTextureSetting textureSetting = new AdvTextureSetting();

		/// <summary>
		/// サウンドファイル設定
		/// </summary>
		public AdvSoundSetting SoundSetting { get { return this.soundSetting; } }
		AdvSoundSetting soundSetting = new AdvSoundSetting();

		/// <summary>
		/// レイヤー設定
		/// </summary>
		public AdvLayerSetting LayerSetting { get { return this.layerSetting; } }
		AdvLayerSetting layerSetting = new AdvLayerSetting();

		/// <summary>
		/// パラメーター設定
		/// </summary>
		public AdvParamManager DefaultParam { get { return this.defaultParam; } }
		AdvParamManager defaultParam = new AdvParamManager();

		/// <summary>
		/// シーン回想設定
		/// </summary>
		public AdvSceneGallerySetting SceneGallerySetting { get { return this.sceneGallerySetting; } }
		AdvSceneGallerySetting sceneGallerySetting = new AdvSceneGallerySetting();

		/// <summary>
		/// ローカライズ設定
		/// </summary>
		public AdvLocalizeSetting LocalizeSetting { get { return this.localizeSetting; } }
		AdvLocalizeSetting localizeSetting = new AdvLocalizeSetting();

		List<IAdvSettingData> SettingDataList
		{
			get
			{
				if (settingDataList == null)
				{
					settingDataList = new List<IAdvSettingData>();
					settingDataList.Add(layerSetting);
					settingDataList.Add(characterSetting);
					settingDataList.Add(textureSetting);
					settingDataList.Add(soundSetting);
					settingDataList.Add(defaultParam);
					settingDataList.Add(sceneGallerySetting);
					settingDataList.Add(localizeSetting);
				}
				return settingDataList;
			}
		}
		List<IAdvSettingData> settingDataList = null;

		internal AdvChapterData FindChapter(string chapterName)
		{
			if (RunTimeChapters == null)
			{
				runTimeChapters = new List<AdvChapterData>();
			}
			return RunTimeChapters.Find(x => x.ChapterName == chapterName);
		}

		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="rootDirResource">ルートディレクトリのリソース</param>
		public void BootInit(string rootDirResource)
		{
			BootSetting.BootInit(rootDirResource);
			if (this.ImportedScenarios != null)
			{
				foreach (AdvChapterData chapter in this.ImportedScenarios.Chapters)
				{
					BootInitChapter(chapter);
				}
			}
			foreach (var chapter in this.RunTimeChapters)
			{
				BootInitChapter(chapter);
			}
		}

		internal void BootInitChapter(AdvChapterData chapter)
		{
			chapter.BootInit();
			foreach (var grid in chapter.SettingList)
			{
				IAdvSettingData data = FindSettingData(grid.SheetName);
				if (data != null)
				{
					data.ParseGrid(grid, BootSetting);
				}
			}
		}

		/// <summary>
		/// 全リソースをバックグラウンドでダウンロード
		/// </summary>
		internal void DownloadAll()
		{
			SettingDataList.ForEach(x => x.DownloadAll());
		}


		//****************************　エクセルのロード用　****************************//

		public const string SheetNameBoot = "Boot";
		public const string SheetNameScenario = "Scenario";
		public const string SheetNameCharacter = "Character";
		public const string SheetNameTexture = "Texture";
		public const string SheetNameSound = "Sound";
		public const string SheetNameLayer = "Layer";
		public const string SheetNameSceneGallery = "SceneGallery";
		public const string SheetNameLocalize = "Localize";

		/// <summary>
		/// 起動設定用のエクセルシートか判定
		/// </summary>
		/// <param name="sheetName">シート名</param>
		/// <returns>起動用ならtrue。違うならfalse</returns>
		public static bool IsBootSheet(string sheetName)
		{
			switch (sheetName)
			{
				case SheetNameBoot:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// 無効なシート名か判定
		/// </summary>
		/// <param name="sheetName">シート名</param>
		/// <returns>設定用ならtrue。違うならfalse</returns>
		public static bool IsDisabelSheetName(string sheetName)
		{
			switch (sheetName)
			{
				case SheetNameBoot:
				case SheetNameScenario:
					return true;
				default:
					return false;
			}
		}


		/// <summary>
		/// 設定用のエクセルシートか判定
		/// </summary>
		/// <param name="sheetName">シート名</param>
		/// <returns>設定用ならtrue。違うならfalse</returns>
		public static bool IsSettingsSheet(string sheetName)
		{
			switch (sheetName)
			{
				case SheetNameScenario:
				case SheetNameCharacter:
				case SheetNameTexture:
				case SheetNameSound:
				case SheetNameLayer:
				case SheetNameSceneGallery:
				case SheetNameLocalize:
					return true;
				default:
					return IsParamSettingsSheet(sheetName);
			}
		}
		/// <summary>
		/// パラメーター設定用のエクセルシートか判定
		/// </summary>
		/// <param name="sheetName">シート名</param>
		/// <returns>設定用ならtrue。違うならfalse</returns>
		public static bool IsParamSettingsSheet(string sheetName)
		{
			return AdvParamManager.IsParamSheetName(sheetName);
		}

		public static bool IsScenarioSheet(string sheetName)
		{
			if (IsDisabelSheetName(sheetName)) return false;
			if (IsSettingsSheet(sheetName)) return false;
			return true;
		}

		/// <summary>
		/// 設定データを探す
		/// </summary>
		IAdvSettingData FindSettingData(string sheetName)
		{
			switch (sheetName)
			{
				case SheetNameCharacter:
					return CharacterSetting;
				case SheetNameTexture:
					return TextureSetting;
				case SheetNameSound:
					return SoundSetting;
				case SheetNameLayer:
					return LayerSetting;
				case SheetNameSceneGallery:
					return SceneGallerySetting;
				case SheetNameLocalize:
					return LocalizeSetting;
				default:
					if (AdvParamManager.IsParamSheetName(sheetName))
					{
						return DefaultParam;
					}
					else
					{
						Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotSettingSheet, sheetName));
						return null;
					}
			}
		}

	}
}