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
	/// 設定データの基本クラス
	/// </summary>
	public abstract class AdvSettingDataBase : IAdvSettingData
	{
		public List<StringGrid> GridList { get { return gridList; } }
		List<StringGrid> gridList = new List<StringGrid>();

		public virtual void ParseGrid(StringGrid grid, AdvBootSetting bootSetting)
		{
			GridList.Add(grid);
			grid.InitLink();
			OnParseGrid(grid, bootSetting);
		}

		/// <summary>
		/// 文字列グリッドから、データ解析
		/// </summary>
		/// <param name="grid"></param>
		protected abstract void OnParseGrid(StringGrid grid, AdvBootSetting bootSetting);

		/// <summary>
		/// 全てのリソースをダウンロード
		/// </summary>
		public virtual void DownloadAll()
		{
		}
	}
}
