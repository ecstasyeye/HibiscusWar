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
	public interface IAdvSettingData
	{
		List<StringGrid> GridList { get; }
		void ParseGrid(StringGrid grid, AdvBootSetting bootSetting);

		/// <summary>
		/// 全てのリソースをダウンロード
		/// </summary>
		void DownloadAll();
	};
}
