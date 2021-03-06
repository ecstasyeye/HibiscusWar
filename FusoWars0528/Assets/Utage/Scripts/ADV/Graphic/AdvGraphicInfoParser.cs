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
	/// グラフィック情報クラス
	/// </summary>
	public static class AdvGraphicInfoParser
	{
		public const string TypeCharacter = "Character";
		public const string TypeTexture = "Texture";

		//指定のタイプ、キーのGraphicを取得
		internal static GraphicInfoList FindGraphicInfo(AdvEngine engine, string dataType, string key)
		{
			switch (dataType)
			{
				case GraphicInfoList.TypeFilePath:
					return new GraphicInfoList(key);
				case TypeCharacter:
					return engine.DataManager.SettingDataManager.CharacterSetting.KeyToGraphicInfo(key);
				case TypeTexture:
				default:
					return engine.DataManager.SettingDataManager.TextureSetting.LabelToGraphic(key);
			}
		}

		//テクスチャパスからグラフィック情報を取得（古いバージョンのセーブデータを読むのに使う）
		internal static GraphicInfoList FindGraphicInfoFromTexturePath(AdvEngine engine, string texturePath)
		{
			GraphicInfoList graphic = engine.DataManager.SettingDataManager.CharacterSetting.FindFromPath(texturePath);
			if (graphic != null) return graphic;

			graphic = engine.DataManager.SettingDataManager.TextureSetting.FindFromPath(texturePath);
			if (graphic != null) return graphic;

			return new GraphicInfoList(texturePath);
		}
	}
}
