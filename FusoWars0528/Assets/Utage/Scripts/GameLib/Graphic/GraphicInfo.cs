//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Utage
{

	/// <summary>
	/// グラフィック情報クラス
	/// </summary>
	public class GraphicInfo
	{
		/// <summary>
		/// 文字列の条件式を計算するコールバック
		/// </summary> 
		public static Func<string, bool> CallbackExpression;

		public StringGridRow RowData { get; protected set; }

		string fileName;

		AssetFile file;
		public AssetFile File
		{
			get { return file; }
			set { file = value; }
		}

		Vector2 pivot = new Vector2(0.5f, 0.5f);
		public Vector2 Pivot
		{
			get { return pivot; }
			set { pivot = value; }
		}

		Vector2 scale = Vector2.one;
		public Vector2 Scale
		{
			get { return scale; }
			set { scale = value; }
		}

		//条件式の判定
		public bool CheckCondionalExpression
		{
			get
			{
				if (null == CallbackExpression)
				{
					Debug.LogError("GraphicInfo CallbackExpression is nul");
					return false;
				}
				else
				{
					return CallbackExpression(CondionalExpression);
				}
			}
		}

		//条件式
		public string CondionalExpression { get; private set; }

		/// <summary>バージョン</summary>
		public int Version { get { return this.version; } }
		int version;

		public GraphicInfo(string filePath)
		{
			Debug.Log("Warning");
			File = AssetFileManager.GetFileCreateIfMissing(filePath);
		}

		public GraphicInfo(StringGridRow row)
		{
			this.RowData = row;
			this.fileName = AdvParser.ParseCell<string>(row, AdvColumnName.FileName);
			try
			{
				this.pivot = ParserUtil.ParsePivotOptional(AdvParser.ParseCellOptional<string>(row, AdvColumnName.Pivot, ""), pivot);
			}
			catch (System.Exception e)
			{
				Debug.LogError(row.ToErrorString(e.Message));
			}

			try
			{
				this.scale = ParserUtil.ParseScale2DOptional(AdvParser.ParseCellOptional<string>(row, AdvColumnName.Scale, ""), this.scale);
			}
			catch (System.Exception e)
			{
				Debug.LogError(row.ToErrorString(e.Message));
			}
			this.CondionalExpression = AdvParser.ParseCellOptional<string>(row, AdvColumnName.Conditional, "");
			this.version = AdvParser.ParseCellOptional<int>(row, AdvColumnName.Version, 0);
//			this.IgnoreLoad = AdvParser.ParseCellOptional<bool>(row, AdvColumnName.IgnoreLoad, false);
		}

		//起動時の初期化
		public void BootInit(System.Func<string, string> FileNameToPath)
		{
			File = AssetFileManager.GetFileCreateIfMissing(FileNameToPath(fileName), RowData);
			if (File!=null)
			{
				File.Version = Version;
			}
		}
	}
}
