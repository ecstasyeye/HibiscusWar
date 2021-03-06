//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// テクスチャ設定（ラベルとファイルの対応）
	/// </summary>
	public class AdvTextureSettingData : AdvSettingDataDictinoayItemBase
	{
		//独自にカスタムしたいファイルタイプの、ルートディレクトリを指定
		public delegate void ParseCustomFileTypeRootDir(string fileType, ref string rootDir);
		public static ParseCustomFileTypeRootDir CallbackParseCustomFileTypeRootDir;

		/// <summary>
		/// ファイル名
		/// </summary>
		string fileName;

		/// <summary>
		/// ファイルパス
		/// </summary>
//		public string FilePath { get { return this.filePath; } }
		string filePath;

		/// <summary>
		/// テクスチャのタイプ
		/// </summary>
		public enum Type
		{
			/// <summary>背景</summary>
			Bg,
			/// <summary>イベントCG</summary>
			Event,
			/// <summary>スプライト</summary>
			Sprite,
		}

		/// <summary>テクスチャのタイプ</summary>
		public Type TextureType { get { return this.type; } }
		Type type;

		//グラフィックの情報
		public GraphicInfoList Graphic { get { return this.graphic; } }
		GraphicInfoList graphic;
		
		/// <summary>
		/// サムネイル用ファイル名
		/// </summary>
		string thumbnailName;

		/// <summary>
		/// サムネイル用ファイルパス
		/// </summary>
		public string ThumbnailPath { get { return this.thumbnailPath; } }
		string thumbnailPath;

		/// <summary>
		/// サムネイル用ファイルのバージョン
		/// </summary>
		public int ThumbnailVersion { get { return this.thumbnailVersion; } }
		int thumbnailVersion;

		/// <summary>
		/// CGギャラリーでのカテゴリ
		/// </summary>
		public string CgCategolly { get { return this.cgCategolly; } }
		string cgCategolly;

		public StringGridRow RowData { get; protected set; }

		/// <summary>
		/// StringGridの一行からデータ初期化
		/// </summary>
		/// <param name="row">初期化するためのデータ</param>
		/// <returns>成否</returns>
		public override bool InitFromStringGridRow(StringGridRow row, AdvBootSetting bootSetting)
		{
			this.RowData = row;
			string key = AdvParser.ParseCell<string>(row, AdvColumnName.Label);
			InitKey(key);
			this.type = AdvParser.ParseCell<Type>(row, AdvColumnName.Type);
			this.graphic = new GraphicInfoList(AdvGraphicInfoParser.TypeTexture, key, AdvParser.ParseCellOptional<string>(row, AdvColumnName.FileType, ""));
			this.thumbnailName = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Thumbnail, "");
			this.thumbnailVersion = AdvParser.ParseCellOptional<int>(row, AdvColumnName.ThumbnailVersion, 0);
			this.cgCategolly = AdvParser.ParseCellOptional<string>(row, AdvColumnName.CgCategolly, "");
			this.AddGraphicInfo(row);
			this.BootInit(bootSetting);
			return true;
		}

		/// <summary>
		/// 起動時の初期化
		/// </summary>
		/// <param name="settingData">設定データ</param>
		internal void BootInit(AdvBootSetting settingData)
		{
			Graphic.BootInit((fileName) => FileNameToPath(fileName, settingData));

			thumbnailPath = settingData.ThumbnailDirInfo.FileNameToPath(thumbnailName);
			if (!string.IsNullOrEmpty(ThumbnailPath))
			{
				AssetFile file = AssetFileManager.GetFileCreateIfMissing(ThumbnailPath);
				if (file!=null) file.Version = ThumbnailVersion;
			}
		}

		string FileNameToPath(string fileName, AdvBootSetting settingData)
		{
			string root = null;
			if (CallbackParseCustomFileTypeRootDir != null)
			{
				CallbackParseCustomFileTypeRootDir(this.graphic.FileType, ref root);
			}
			if (root != null)
			{
				return FilePathUtil.Combine(settingData.ResorceDir, root, fileName);
			}
			else
			{
				if (!Graphic.IsDefaultFileType)
				{
					return FilePathUtil.Combine(settingData.ResorceDir, fileName);
				}
				else
				{
					switch (type)
					{
						case AdvTextureSettingData.Type.Event:
							return settingData.EventDirInfo.FileNameToPath(fileName);
						case AdvTextureSettingData.Type.Sprite:
							return settingData.SpriteDirInfo.FileNameToPath(fileName);
						case AdvTextureSettingData.Type.Bg:
						default:
							return settingData.BgDirInfo.FileNameToPath(fileName);
					}
				}
			}
		}

		internal void AddGraphicInfo(StringGridRow row)
		{
			Graphic.Add(row);
		}
	}

	/// <summary>
	/// テクスチャ設定
	/// </summary>
	public class AdvTextureSetting : AdvSettingDataDictinoayBase<AdvTextureSettingData>
	{
		//連続するデータとして追加できる場合はする。
		protected override bool TryParseContiunes(AdvTextureSettingData last, StringGridRow row, AdvBootSetting bootSetting)
		{
			if (last == null) return false;

			string key = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Label,"");
			if (!string.IsNullOrEmpty(key)) return false;

			last.AddGraphicInfo(row);
			last.BootInit(bootSetting);
			return true;
		}

		/// <summary>
		/// 全てのリソースをダウンロード
		/// </summary>
		public override void DownloadAll()
		{
			//ファイルマネージャーにバージョンの登録
			foreach (AdvTextureSettingData data in List)
			{
				data.Graphic.DownloadAll();
				if (!string.IsNullOrEmpty(data.ThumbnailPath))
				{
					AssetFileManager.Download(data.ThumbnailPath);
				}
			}
		}

		/// <summary>
		/// ラベルからグラフィック情報を取得
		/// </summary>
		/// <param name="label">ラベル</param>
		/// <returns>グラフィック情報</returns>
		public GraphicInfoList LabelToGraphic(string label)
		{
			AdvTextureSettingData data = FindData(label);
			if (data != null)
			{
				return data.Graphic;
			}
			else
			{
				//ラベルをそのままファイル名扱いに
				return new GraphicInfoList(label);
			}
		}

		/// <summary>
		/// ラベルからファイルパスを取得
		/// </summary>
		/// <param name="label">ラベル</param>
		/// <returns>ファイルパス</returns>
		public bool ContainsLabel(string label)
		{
			//既に絶対URLならそのまま
			if (FilePathUtil.IsAbsoluteUri(label))
			{
				return true;
			}
			else
			{
				AdvTextureSettingData data = FindData(label);
				if (data == null)
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		//ラベルからファイル名を取得
		AdvTextureSettingData FindData(string label)
		{
			AdvTextureSettingData data;
			if (!Dictionary.TryGetValue(label, out data))
			{
				return null;
			}
			else
			{
				return data;
			}
		}

		/// <summary>
		/// CGギャラリー用のデータを取得
		/// </summary>
		/// <param name="saveData">セーブデータ</param>
		/// <param name="gallery">ギャリーのデータ</param>
		public List<AdvCgGalleryData> CreateCgGalleryList( AdvGallerySaveData saveData )
		{
			return CreateCgGalleryList(saveData, "");
		}

		/// <summary>
		/// CGギャラリー用のデータを取得
		/// </summary>
		/// <param name="saveData">セーブデータ</param>
		/// <param name="category">セーブデータ</param>
		/// <param name="gallery">ギャリーのデータ</param>
		public List<AdvCgGalleryData> CreateCgGalleryList(AdvGallerySaveData saveData, string category)
		{
			List<AdvCgGalleryData> list = new List<AdvCgGalleryData>();
			AdvCgGalleryData currentData = null;
			foreach (var item in List)
			{
				if (item.TextureType == AdvTextureSettingData.Type.Event)
				{
					if (string.IsNullOrEmpty(item.ThumbnailPath)) continue;
					if (!string.IsNullOrEmpty(category) && item.CgCategolly != category) continue;

					string path = item.ThumbnailPath;
					if (currentData == null)
					{
						currentData = new AdvCgGalleryData(path, saveData);
						list.Add(currentData);
					}
					else
					{
						if (path != currentData.ThumbnailPath)
						{
							currentData = new AdvCgGalleryData(path, saveData);
							list.Add(currentData);
						}
					}
					currentData.AddTextureData(item);
				}
			}
			return list;
		}

		/// <summary>
		/// CGギャラリー用のカテゴリのリストを取得
		/// </summary>
		public List<string> CreateCgGalleryCategoryList()
		{
			List<string> list = new List<string>();
			foreach (var item in List)
			{
				if (item.TextureType == AdvTextureSettingData.Type.Event)
				{
					if (string.IsNullOrEmpty(item.ThumbnailPath)) continue;
					if (string.IsNullOrEmpty(item.CgCategolly)) continue;
					if (!list.Contains(item.CgCategolly))
					{
						list.Add(item.CgCategolly);
					}
				}
			}
			return list;
		}

		//古いセーブデータを読むのに使う
		internal GraphicInfoList FindFromPath(string texturePath)
		{
			foreach (var item in List)
			{
				GraphicInfo graphic = item.Graphic.FindFromPath(texturePath);
				if (graphic != null) return item.Graphic;
			}
			return null;
		}
	}
}