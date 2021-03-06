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

	//「Utage」のリソースコンバーター
	public class AdvResourcesConverter : CustomEditorWindow
	{
		const string KeyScenario = "Scenario";
		const string KeyResouces = "Resouces";
		//****************リソース****************//

		//リソースのコンバート
		[SerializeField]
		bool isConvertResources = true;
		public bool IsConvertResources
		{
			get { return isConvertResources; }
			set { isConvertResources = value; }
		}

		/// <summary>
		/// リソースのパス
		/// </summary>
		[SerializeField]
		Object resourcesDirectory;
		public Object ResourcesDirectory
		{
			get { return resourcesDirectory; }
			set { resourcesDirectory = value; }
		}

		/// <summary>
		/// 新しいファイルのみコピーするか
		/// </summary>
		[SerializeField]
		bool isOnlyNew = true;
		public bool IsOnlyNew
		{
			get { return isOnlyNew; }
			set { isOnlyNew = value; }
		}

		//****************シナリオ****************//

		/// <summary>
		/// シナリオファイルもコンバートするか
		/// </summary>
		[SerializeField]
		bool isConvertScenario = true;
		public bool IsConvertScenario
		{
			get { return isConvertScenario; }
			set { isConvertScenario = value; }
		}

		/// <summary>
		/// シナリオファイルコンバート用のプロジェクトファイル
		/// </summary>
		[SerializeField]
		AdvScenarioDataProject projectSetting;
		public AdvScenarioDataProject ProjectSetting
		{
			get { return projectSetting; }
			set { projectSetting = value; }
		}

		//****************アセットバンドル****************//

		//アセットバンドルのビルドをするか
		enum AssetBundleBuildMode
		{
			None,			//ビルドしない
			OnlyEditor,		//エディタ用のみビルドする
			AllPlatform,	//全プラットフォーム用のものをビルドする
		};
		[SerializeField]
		AssetBundleBuildMode assetBundleBuildMode = AssetBundleBuildMode.OnlyEditor;

		[SerializeField]
		Object assetBundleDirectory;
		public Object AssetBundleDirectory
		{
			get { return assetBundleDirectory; }
			set { assetBundleDirectory = value; }
		}

		//アセットバンドルのリネーム法則
		public enum AssetBundleRenameType
		{
			None,			//名前を変えない
			Rename,			//リネームする
			OnlyNotNamed,	//まだ名前が設定されていないものだけリネームする
		};
		[SerializeField]
		AssetBundleRenameType assetBundleRenameType = AssetBundleRenameType.Rename;

		[SerializeField, EnumFlags]
		AssetBundleTargetFlags buildTargetFlags = AssetBundleTargetFlags.WebPlayer;


		//****************出力設定****************//

		public enum OutputType
		{
			Default,
			Advance,
		};
		[SerializeField]
		OutputType outputType;

		[SerializeField]
		bool isOutputLocal;

		[SerializeField]
		bool isOutputLocalLog = true;

		/// <summary>
		/// ローカル用のリソースの出力先のパス
		/// </summary>
		[SerializeField]
		Object localDirectory;
		public Object LocalDirectory
		{
			get { return localDirectory; }
			set { localDirectory = value; }
		}

		[SerializeField]
		bool isOutputServer;

		[SerializeField]
		bool isOutputServerLog = true;

		/// <summary>
		/// サーバー用のリソースの出力先のパス
		/// </summary>
		[SerializeField, PathDialog(PathDialogAttribute.DialogType.Directory)]
		string outputServerResourcePath;
		public string OutputServerResourcePath
		{
			get { return outputServerResourcePath; }
			set { outputServerResourcePath = value; }
		}

		/// <summary>
		/// サーバー用のリソースの出力先のパス
		/// </summary>
		[SerializeField, PathDialog(PathDialogAttribute.DialogType.Directory)]
		string advancedOutputPath;
		public string AdvancedOutputPath
		{
			get { return advancedOutputPath; }
			set { advancedOutputPath = value; }
		}

		[SerializeField]
		bool isOutputAdvancedLog = true;


		void OnEnable()
		{
			//スクロールを有効にする
			this.isEnableScroll = true;
		}

		protected override bool DrawProperties()
		{
			SerializedObjectHelper.SerializedObject.Update();
			OnDrawCustom(this.SerializedObjectHelper);
			bool ret = SerializedObjectHelper.SerializedObject.ApplyModifiedProperties();

			if (!ret)
			{
				bool isEnableOutputResources =
					ResourcesDirectory != null
					&& !(IsConvertScenario && ProjectSetting == null)
					;

				EditorGUI.BeginDisabledGroup(!isEnableOutputResources);
				bool isButton = GUILayout.Button("Convert", GUILayout.Width(180));
				EditorGUI.EndDisabledGroup();
				GUILayout.Space(8f);

				if (isButton)
				{
					Convert();
				}
			}
			return ret;
		}

		void OnDrawCustom(SerializedObjectHelper helper)
		{
			helper.IsDrawScript = true;
			helper.DrawHeader();
			helper.BeginGroup("Resources");
			{
				helper.DrawProperty("isConvertResources", "Convert");
				if (this.IsConvertResources)
				{
					helper.DrawProperty("resourcesDirectory");
					helper.DrawProperty("isOnlyNew");
				}
			}
			helper.EndGroup();

			helper.BeginGroup("Scenario");
			{
				helper.DrawProperty("isConvertScenario", "Convert");
				if (this.IsConvertScenario)
				{
					helper.DrawProperty("projectSetting", "Project Setting");
				}
			}
			helper.EndGroup();

			helper.BeginGroup("AssetBundle");
			{
				helper.DrawProperty("assetBundleBuildMode", "BuildMode");
				if (this.assetBundleBuildMode != AssetBundleBuildMode.None)
				{
					helper.DrawProperty("assetBundleDirectory", "Directory");
					helper.DrawProperty("assetBundleRenameType", "RenameType");
					helper.DrawProperty("buildTargetFlags", "buildTarget");
				}
			}
			helper.EndGroup();

			helper.BeginGroup("Output Setting");
			{
				helper.DrawProperty("outputType");
				if (outputType == OutputType.Default)
				{
					helper.BeginGroup("Local");
					{
						helper.DrawProperty("isOutputLocal", "Output");
						if (isOutputLocal)
						{
							helper.DrawProperty("isOutputLocalLog", "Output Log");
							helper.DrawProperty("localDirectory", "Directory");
						}
					}
					helper.EndGroup();

					helper.BeginGroup("Server");
					{
						helper.DrawProperty("isOutputServer", "Output");
						if (isOutputServer)
						{
							helper.DrawProperty("isOutputServerLog", "Output Log");
							helper.DrawProperty("outputServerResourcePath", "Output Path");
						}
					}
					helper.EndGroup();
				}
				else
				{
					helper.DrawProperty("isOutputAdvancedLog", "Output Log");
					helper.DrawProperty("advancedOutputPath");
				}
			}
			helper.EndGroup();
		}

		//ファイルのコンバート
		void Convert()
		{
			try
			{
				AssetFileManager assetFileManager = FindObjectOfType<AssetFileManager>();
				if (assetFileManager == null)
				{
					Debug.LogError("FileManager is not found in current scene");
					return;
				}
				//ファイルの入出力に使う
				FileIOManager fileIOManager = assetFileManager.FileIOManger;
				AssetFileManagerSettings settings = assetFileManager.Settings;

				if (outputType == OutputType.Default)
				{
					AssetFileManagerSettings.LoadType loadType = settings.LoadTypeSetting;
					if (isOutputLocal && LocalDirectory != null)
					{
						//ローカルの場合は、LoadTypeをLocalCompressedに
						settings.LoadTypeSetting = AssetFileManagerSettings.LoadType.LocalCompressed;
						string output = new MainAssetInfo(LocalDirectory).FullPath;
						//リソースをバージョンアップ
						AdvFileListConverter converter = new AdvFileListConverter(output, fileIOManager, settings);
						converter.VersionUp(VersionUpLocalFiles);
						if (isOutputLocalLog) converter.WriteLog(false);
					}
					if (isOutputServer && !string.IsNullOrEmpty(OutputServerResourcePath))
					{
						//サーバーの場合は、LoadTypeをServerに
						settings.LoadTypeSetting = AssetFileManagerSettings.LoadType.Server;
						//シナリオやリソースをバージョンアップ
						AdvFileListConverter converter = new AdvFileListConverter(OutputServerResourcePath, fileIOManager, settings);
						converter.VersionUp(VersionUpServerFiles);
						if (isOutputServerLog) converter.WriteLog(false);
						//アセットバンドルをビルド
						BuildAssetBundles(OutputServerResourcePath, fileIOManager, settings, isOutputServerLog);
					}
					settings.LoadTypeSetting = loadType;
				}
				else
				{
					//シナリオやリソースをバージョンアップ
					AdvFileListConverter converter = new AdvFileListConverter(AdvancedOutputPath, fileIOManager, settings);
					converter.VersionUp(VersionUpAdvanceFiles);
					if (isOutputAdvancedLog) converter.WriteLog(false);
					//アセットバンドルをビルド
					BuildAssetBundles(AdvancedOutputPath, fileIOManager, settings, isOutputAdvancedLog);
				}

				AssetDatabase.Refresh();
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
			}
		}

		//ローカルのリソースをバージョンアップ
		void VersionUpLocalFiles(AdvFileListConverter converter)
		{
			if (IsConvertResources) VersionUpResources(converter);
		}

		//サーバーのリソースやシナリオをバージョンアップ
		void VersionUpServerFiles(AdvFileListConverter converter)
		{
			if (IsConvertResources) VersionUpResources(converter);
			if (IsConvertScenario) VersionUpScenrio(converter);
		}

		//特殊設定のリソースやシナリオをバージョンアップ
		void VersionUpAdvanceFiles(AdvFileListConverter converter)
		{
			if (IsConvertResources) VersionUpResources(converter);
			if (IsConvertScenario) VersionUpScenrio(converter);
		}

		//リソースのバージョンアップ
		void VersionUpResources(AdvFileListConverter converter)
		{
			converter.VersionUpResouces(KeyResouces, ResourcesDirectory, IsOnlyNew);
		}

		//シナリオのバージョンアップ
		void VersionUpScenrio(AdvFileListConverter converter)
		{
			converter.ConvertFileList.EditorVersionUp(KeyScenario, VersionUpScenrioSub(converter));
		}

		List<ConvertFileList.CusomFileVersionUpInfo> VersionUpScenrioSub(AdvFileListConverter converter)
		{
			List<ConvertFileList.CusomFileVersionUpInfo> list = new List<ConvertFileList.CusomFileVersionUpInfo>();

			AdvExcelCsvConverter excelConverter = new AdvExcelCsvConverter();
			List<AdvExcelCsvConverter.CsvInfo> csvInfoList = new List<AdvExcelCsvConverter.CsvInfo>();
			foreach (AdvScenarioDataProject.ChapterData item in ProjectSetting.ChapterDataList)
			{
				if (!excelConverter.TryConvertToCsvList(item.ExcelPathList, item.chapterName, 0, ref csvInfoList))
				{
					throw new System.Exception("Convert is failed");
				}
			}

			int count = 0;
			foreach (AdvExcelCsvConverter.CsvInfo csvInfo in csvInfoList)
			{
				ConvertFileList.CusomFileVersionUpInfo versionUpInfo;
				if (converter.TryVersionUpFileFromMem(csvInfo.Path + ExtensionUtil.TSV, System.Text.Encoding.UTF8.GetBytes(csvInfo.Grid.ToText()), out versionUpInfo))
				{
					++count;
				}
				list.Add(versionUpInfo);
			}
			Debug.Log(string.Format("Scenairo {0}/{1} files updateted", count, csvInfoList.Count));
			return list;
		}

		//アセットバンドルのビルド
		void BuildAssetBundles(string outPutDirectoryPath, FileIOManager fileIOManager, AssetFileManagerSettings settings, bool isOutputLog)
		{
			if (assetBundleBuildMode == AssetBundleBuildMode.None) return;

			//アセットバンドルをプラットフォーム別にビルド
			List<BuildTarget> buildTargets = new List<BuildTarget>();
			switch (assetBundleBuildMode)
			{
				case AssetBundleBuildMode.OnlyEditor://エディタ上のみ
					buildTargets.Add(EditorUserBuildSettings.activeBuildTarget);
					break;
				case AssetBundleBuildMode.AllPlatform://全プラットフォーム
					{
						buildTargets = AssetBundleHelper.BuildTargetFlagsToBuildTargetList(buildTargetFlags);
					}
					break;
				default:
					break;
			}

			List<MainAssetInfo> assets = GetAssetBudleList(settings);
			RenameAssetBundles(assets);
			AssetBundleBuild[] builds = ToAssetBundleBuilds(assets);
			if (builds.Length <= 0) return;


			foreach (BuildTarget buildTarget in buildTargets)
			{
				string outputPath = FilePathUtil.Combine(outPutDirectoryPath, AssetBundleHelper.BuildTargetToBuildTargetFlag(buildTarget).ToString());
				//出力先のディレクトリを作成
				if (!Directory.Exists(outputPath))
				{
					Directory.CreateDirectory(outputPath);
				}
				//アセットバンドルを作成
				AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, builds, BuildAssetBundleOptions.None, buildTarget);

				//アセットバンドルの情報をバージョンアップ
				AdvFileListConverter converter = new AdvFileListConverter(outputPath, fileIOManager, settings);
				converter.VersionUp(
					(x) =>
					{
						int count = x.ConvertFileList.EditorVersionUpAssetBundle(manifest, buildTarget);
						Debug.Log("" + count + " AsseBundle version up to target " + buildTarget.ToString());
					});
				if (isOutputLog) converter.WriteLog(true);
			}
		}

		//アセットバンドルのリストを取得
		List<MainAssetInfo> GetAssetBudleList(AssetFileManagerSettings settings)
		{
			List<MainAssetInfo> assets = new List<MainAssetInfo>();

			//指定ディレクトリ以下のアセットを全て取得
			MainAssetInfo inputDirAsset = new MainAssetInfo(AssetBundleDirectory);
			foreach (MainAssetInfo asset in inputDirAsset.GetAllChidren())
			{
				if (asset.IsDirectory) continue;
				if (IsIgnoreAssetBundle(asset)) continue;
				AssetFileSetting assetFileSetting = settings.FindSettingFromPath(asset.AssetPath);
				AssetFileEncodeType encodeType = assetFileSetting.EncodeType;
				switch (encodeType)
				{
					case AssetFileEncodeType.AssetBundle:
						assets.Add(asset);
						break;
					default:
						break;
				}
			}

			return assets;
		}

		//アセットバンドル化しないアセットを取得
		bool IsIgnoreAssetBundle(MainAssetInfo asset)
		{
			string path = asset.AssetPath;
			if (path.EndsWith("keep.keep"))
			{
				return true;
			}

			return false;
		}


		//アセットバンドル名のリネーム
		void RenameAssetBundles(List<MainAssetInfo> assets)
		{
			if (assetBundleRenameType == AssetBundleRenameType.None) return;

			foreach (MainAssetInfo asset in assets)
			{
				AssetImporter importer = asset.AssetImporter;
				if (importer == null)
				{
					Debug.LogError("Not Find Importer");
					continue;
				}

				if (assetBundleRenameType == AssetBundleRenameType.OnlyNotNamed
					&& !string.IsNullOrEmpty(importer.assetBundleName))
				{
					//まだ名前がついていないときにのみネーミング
					continue;
				}

				string assetBundleName = Path.GetFileNameWithoutExtension(asset.AssetPath);
				//強制的にリネーム
				if (importer.assetBundleName != assetBundleName)
				{
					importer.assetBundleName = assetBundleName;
					importer.SaveAndReimport();
				}
			}
		}

		//アセットバンドルリストを取得
		AssetBundleBuild[] ToAssetBundleBuilds(List<MainAssetInfo> assets)
		{
			List<AssetBundleBuild> list = new List<AssetBundleBuild>();
			foreach (MainAssetInfo asset in assets)
			{
				AssetImporter importer = asset.AssetImporter;
				if (importer == null)
				{
					Debug.LogError("Not Find Importer");
					continue;
				}
				AssetBundleBuild build = new AssetBundleBuild();
				build.assetBundleName = importer.assetBundleName;
				build.assetBundleVariant = importer.assetBundleVariant;
				build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(importer.assetBundleName);
				list.Add(build);
			}
			return list.ToArray();
		}

	}
}