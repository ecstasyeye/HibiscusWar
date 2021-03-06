//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// ADVデータ解析
	/// </summary>
	public class AdvParser
	{
		public static string Localize(AdvColumnName name)
		{
			//多言語化をしてみたけど、複雑になってかえって使いづらそうなのでやめた
			return name.ToString();
		}

		//指定の名前のセルを、型Tとして解析・取得（データがなかったらエラーメッセージを出す）
		public static T ParseCell<T>(StringGridRow row, AdvColumnName name)
		{
			return row.ParseCell<T>(Localize(name));
		}

		//指定の名前のセルを、型Tとして解析・取得（データがなかったらデフォルト値を返す）
		public static T ParseCellOptional<T>(StringGridRow row, AdvColumnName name, T defaultVal)
		{
			return row.ParseCellOptional<T>(Localize(name), defaultVal);
		}

		//指定の名前のセルを、型Tとして解析・取得（データがなかったらfalse）
		public static bool TryParseCell<T>(StringGridRow row, AdvColumnName name, out T val)
		{
			return row.TryParseCell<T>(Localize(name), out val);
		}

		//セルが空かどうか
		public static bool IsEmptyCell(StringGridRow row, AdvColumnName name)
		{
			return row.IsEmptyCell(Localize(name));
		}

		//マクロやエンティティによる変換を無視する行
		internal static List<int> CreateMacroOrEntityIgnoreIndexArray(StringGrid grid)
		{
			List<int> list = new List<int>();
			/*			//ウィンドウタイプには無効
						int indexWindowType;
						if (!grid.TryGetColumnIndex(AdvColumnName.WindowType.ToString(), out indexWindowType))
						{
							indexWindowType = -1;
						}
						list.Add(indexWindowType);

						int indexPageCtrl;
						if (!grid.TryGetColumnIndex(AdvColumnName.PageCtrl.ToString(), out indexPageCtrl))
						{
							indexPageCtrl = -1;
						}
						list.Add(indexPageCtrl);*/

			return list;
		}
	}
}
