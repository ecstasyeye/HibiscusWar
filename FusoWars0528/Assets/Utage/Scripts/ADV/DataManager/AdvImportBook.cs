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
	/// インポートしたBook（エクセルファイル）のデータ
	/// </summary>
	public class AdvImportBook : ScriptableObject
	{
		const int Version = 0;
		
		[SerializeField]
		int importVersion = 0;
		public bool CheckVersion()
		{
			return importVersion == Version;
		}

		public List<StringGrid> GridList { get { return gridList; } }
		[SerializeField]
		List<StringGrid> gridList = new List<StringGrid>();

		public void Clear()
		{
			gridList.Clear();
		}

		public void AddData(StringGrid grid)
		{
			gridList.Add(grid);
		}
	}
}