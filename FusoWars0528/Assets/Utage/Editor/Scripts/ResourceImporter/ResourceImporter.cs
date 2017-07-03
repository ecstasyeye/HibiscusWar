//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Utage
{
	public sealed class ResourceImporter : AssetPostprocessor
	{
		//Audioファイルのインポート設定
		void OnPreprocessAudio()
		{
			//インポート時のAudioファイルを設定するクラス
			AudioImporter importer = assetImporter as AudioImporter;

			//宴のリソースかチェック
			if (!IsCustomImportAudio(importer))
			{
				return;
			}
			//各設定
			WrapperUnityVersion.SetAudioImporterTreeD(importer, false);
		}

		//Textureファイルのインポート設定 Textureファイルがインポートされる直前に呼び出される
		void OnPreprocessTexture()
		{
			//インポート時のTextureファイルを設定するクラス
			TextureImporter importer = assetImporter as TextureImporter;

			//宴のリソースかチェック
			AdvScenarioDataProject.TextureType textureType = ParseCustomImportTextureType(importer);
			if (textureType == AdvScenarioDataProject.TextureType.Unknown)
			{
				return;
			}

			importer.textureType = TextureImporterType.Default;
			importer.spriteImportMode = SpriteImportMode.None;
/*			switch (textureType)
			{
				case AdvScenarioDataProject.TextureType.Character:
				case AdvScenarioDataProject.TextureType.Sprite:
					importer.isReadable = true;
					break;
				default:
					importer.isReadable = false;
					break;
			}
*/
			//各設定
//			importer.textureType = TextureImporterType.Sprite;					//スプライトに設定
			importer.mipmapEnabled = false;										//MipMapはオフに
			importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;	//True Color
            importer.maxTextureSize = 4096;										//テクスチャサイズの設定
			importer.alphaIsTransparency = true;								//アルファの透明設定
			importer.wrapMode = TextureWrapMode.Clamp;							//Clamp設定
			importer.npotScale = TextureImporterNPOTScale.None;					//Non Power of 2
			AssetDatabase.WriteImportSettingsIfDirty(AssetDatabase.GetAssetPath(importer));
		}

		//カスタムインポート対象のオーディオか
		bool IsCustomImportAudio(AssetImporter importer)
		{
			AdvScenarioDataProject project = AdvScenarioDataBuilderWindow.ProjectData;
			if(project)
			{
				return project.IsCustomImportAudio(importer);
			}
			return false;
		}

		//カスタムインポート対象のテクスチャか
		AdvScenarioDataProject.TextureType ParseCustomImportTextureType(AssetImporter importer)
		{
			AdvScenarioDataProject project = AdvScenarioDataBuilderWindow.ProjectData;
			if (project)
			{
				return project.ParseCustomImportTextureType(importer);
			}
			return AdvScenarioDataProject.TextureType.Unknown;
		}

		//カスタムインポート対象のMovieか
		bool IsCustomImportMovie(AssetImporter importer)
		{
			AdvScenarioDataProject project = AdvScenarioDataBuilderWindow.ProjectData;
			if (project)
			{
				return project.IsCustomImportMovie(importer);
			}
			return false;
		}


/*
		const string UtageAudioResouceLabel = "UtageAudio";
		const string UtageSpriteResouceLabel = "UtageSprite";


		//インポーターと、そのルートまでの各親フォルダに、指定のラベルがあるかチェック
		bool CheckImporterLabel(AssetImporter importer, string label, bool addSelfLabel )
		{
			//自身にラベルがあるか
			if (CheckAssetLabel(importer, label))
			{
				return true;
			}
			//親にラベルがあるか
			if (CheckParentsDirectoryLabel(importer.assetPath, label))
			{
				if (addSelfLabel) SetLabel(importer, label);
				return true;
			}
			return false;
		}

		//ルートまでの各親フォルダに、指定のラベルがあるかチェック
		bool CheckParentsDirectoryLabel(string assetPath, string label)
		{
			string fullPath = UtageEditorToolKit.AssetPathToSystemIOFullPath(assetPath);
			System.IO.DirectoryInfo info = System.IO.Directory.GetParent(fullPath);
			while (info!=null)
			{
				string path = UtageEditorToolKit.SystemIOFullPathToAssetPath(info.FullName);
				if (string.IsNullOrEmpty(path))
				{
					break;
				}
				if (CheckDirectoryLabel(path, label))
				{
					return true;
				}
				info = info.Parent;
			}
			return false;
		}

		//フォルダに指定のラベルがあるかチェック
		bool CheckDirectoryLabel(string assetPath, string label)
		{
			Object assetDirectory = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
			if (assetDirectory)
			{
				return CheckAssetLabel(assetDirectory, label);
			}
			return false;
		}

		//アセットに指定のラベルがあるかチェック
		bool CheckAssetLabel(Object asset, string label)
		{
			string[] labels = AssetDatabase.GetLabels(asset);
			return ArrayUtility.Contains<string>(labels,label);
		}

		//ラベルを設定		
		void SetLabel(Object asset, string label)
		{
			string[] labels = {label};
			AssetDatabase.SetLabels(asset,labels);
		}
 */
	}
}