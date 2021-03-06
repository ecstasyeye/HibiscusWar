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
	/// グラフィック情報のリスト
	/// </summary>
	public class GraphicInfoList
	{
		//独自にカスタムしたいファイルタイプの、LoadCompleteを指定
		public delegate void ParseCustomFileTypeLoadComplete(string fileType, ref AssetFileEvent onLoadCmplete);
		public static ParseCustomFileTypeLoadComplete CallbackParseCustomFileTypeLoadComplete;

		//独自にカスタムしたいファイルタイプの、LoadSubfilesを指定
		public delegate void ParseCustomFileTypeLoadSubfiles(string fileType, ref AssetFileEvent onLoadSubfiles);
		public static ParseCustomFileTypeLoadSubfiles CallbackParseCustomFileTypeLoadSubfiles;

		public const string TypeFilePath = "FilePath";
		public string DataType { get; protected set; }
		public string Key { get; protected set; }

		/// <summary>ファイルタイプ</summary>
		public string FileType { get { return this.fileType; } }
		string fileType;

		public List<GraphicInfo> InfoList { get { return infoList; } }
		List<GraphicInfo> infoList = new List<GraphicInfo>();

		public GraphicInfo Main {
			get
			{
				if(InfoList.Count == 0 ) return null;

				//複数持っている場合を考慮して条件判定を行う
				GraphicInfo main = null;
				foreach (GraphicInfo graphic in InfoList)
				{
					if (string.IsNullOrEmpty(graphic.CondionalExpression))
					{
						if (main == null)
						{
							main = graphic;
						}
					}
					else if (graphic.CheckCondionalExpression)
					{
						return graphic;
					}
				}
				return main;
			}		
		}

		public GraphicInfoList( string dataType, string key, string fileType )
		{
			this.DataType = dataType;
			this.Key = key;
			this.fileType = fileType;
//			this.fileType = AdvParser.ParseCellOptional<string>(row, AdvColumnName.FileType, "");
		}

		//ラベルをそのままファイル名扱いにしたグラフィック情報を作成
		public GraphicInfoList(string filePath)
		{
			this.DataType = TypeFilePath;
			this.Key = filePath;
			Add(new GraphicInfo(filePath));
		}

		internal void Add(StringGridRow row)
		{
			Add(new GraphicInfo(row));
		}

		internal void Add(GraphicInfo graphic)
		{
			infoList.Add(graphic);
		}

		internal void BootInit(System.Func<string, string> FileNameToPath)
		{
			foreach (var item in infoList)
			{
				item.BootInit(FileNameToPath);
			}

			//特定のファイルタイプなら、ロード終了時の処理をする
			if (CallbackParseCustomFileTypeLoadComplete != null && !AssetFileManager.IsEditorErrorCheck)
			{
				AssetFileEvent onLoadComplete = null;
				CallbackParseCustomFileTypeLoadComplete(this.FileType, ref onLoadComplete);
				if (onLoadComplete != null)
				{
					foreach (GraphicInfo info in InfoList)
					{
						info.File.OnLoadComplete += onLoadComplete;
					}
				}
			}
			//特定のファイルタイプなら、サブファイルロードの処理をする
			if (CallbackParseCustomFileTypeLoadSubfiles != null && !AssetFileManager.IsEditorErrorCheck)
			{
				AssetFileEvent onLoadSubfiles = null;
				CallbackParseCustomFileTypeLoadSubfiles(this.FileType, ref onLoadSubfiles);
				if (onLoadSubfiles != null)
				{
					foreach (GraphicInfo info in InfoList)
					{
						info.File.OnLoadSubFiles += onLoadSubfiles;
					}
				}
			}
		}

		internal void DownloadAll()
		{
			foreach( var item in infoList )
			{
				AssetFileManager.Download(item.File);
			}
		}

		//古いセーブデータを読むのに使う
		internal GraphicInfo FindFromPath(string texturePath)
		{
			foreach (var item in infoList)
			{
				if (item.File.FileName == texturePath) return item;
			}
			return null;
		}

		public bool IsLoadEnd
		{
			get
			{
				foreach (var item in infoList)
				{
					if (!item.File.IsLoadEnd) return false;
				}
				return true;
			}
		}

		public bool IsDefaultFileType
		{
			get
			{
				switch (this.FileType)
				{
					case "":
						return true;
					default:
						return false;
				}
			}
		}
	}
}
