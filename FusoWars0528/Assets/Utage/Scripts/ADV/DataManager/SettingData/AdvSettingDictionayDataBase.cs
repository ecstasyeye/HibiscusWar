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
	/// StringGridから作成するKeyValueデータ
	/// </summary>
	public abstract class AdvSettingDataDictinoayItemBase
	{
		/// <summary>
		/// キー
		/// </summary>
		public string Key { get { return key; } }
		string key;

		/// <summary>
		/// キーの初期化
		/// </summary>
		/// <param name="key"></param>
		internal void InitKey(string key) { this.key = key; }

		/// <summary>
		/// 文字列グリッドの行データから、データを初期化
		/// </summary>
		/// <param name="row">初期化するための文字列グリッドの行データ</param>
		/// <returns>成否。空のデータの場合などはfalseが帰る</returns>
		internal bool InitFromStringGridRowMain(StringGridRow row, AdvBootSetting bootSetting)
		{
			this.row = row;
			return InitFromStringGridRow(row, bootSetting);
		}

		/// <summary>
		/// 文字列グリッドの行データから、データを初期化
		/// </summary>
		/// <param name="row">初期化するための文字列グリッドの行データ</param>
		/// <returns>成否。空のデータの場合などはfalseが帰る</returns>
		public abstract bool InitFromStringGridRow(StringGridRow row, AdvBootSetting bootSetting);

		//元となる行データ
		public StringGridRow Row { get { return row; } }
		StringGridRow row;
	}

	/// <summary>
	/// 設定データの基本クラス
	/// </summary>
	public abstract class AdvSettingDataDictinoayBase<T> : AdvSettingDataBase
				where T : AdvSettingDataDictinoayItemBase, new()
	{
		public List<T> List { get; private set; }
		public Dictionary<string, T> Dictionary { get; private set; }
		public AdvSettingDataDictinoayBase()
		{
			Dictionary = new Dictionary<string, T>();
			List = new List<T>();
		}

		/// <summary>
		/// 文字列グリッドから、データ解析
		/// </summary>
		/// <param name="grid"></param>
		protected override void OnParseGrid(StringGrid grid, AdvBootSetting bootSetting)
		{
			T last = null;
			foreach (StringGridRow row in grid.Rows)
			{
				if (row.RowIndex < grid.DataTopRow) continue;			//データの行じゃない
				if (row.IsEmptyOrCommantOut) continue;								//データがない

				if (!TryParseContiunes(last, row, bootSetting))
				{
					T data = ParseFromStringGridRow(last, row, bootSetting);
					if (data != null) last = data;
				}
			}
		}

		//連続するデータとして追加できる場合はする。基本はしない
		protected virtual bool TryParseContiunes(T last, StringGridRow row, AdvBootSetting bootSetting)
		{
			if (last == null) return false;
			return false;
		}

		//連続するデータとして追加できる場合はする。基本はしない
		protected virtual T ParseFromStringGridRow(T last, StringGridRow row, AdvBootSetting bootSetting)
		{
			T data = new T();
			if (data.InitFromStringGridRowMain(row, bootSetting))
			{
				if (!Dictionary.ContainsKey(data.Key))
				{
					AddData(data);
					return data;
				}
				else
				{
					string errorMsg = "";
					errorMsg += row.ToErrorString(ColorUtil.AddColorTag(data.Key, Color.red) + "  is already contains");
					Debug.LogError(errorMsg);
				}
			}
			return null;
		}

		protected void AddData(T data)
		{
			List.Add(data);
			Dictionary.Add(data.Key, data);
		}
	}
}
