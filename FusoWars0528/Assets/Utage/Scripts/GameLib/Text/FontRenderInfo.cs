//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;

namespace Utage
{

	/// <summary>
	/// 1文字ごとのフォントの描画情報
	/// </summary>
	public class FontRenderInfo
	{
		/// <summary>
		/// 文字
		/// </summary>
		public char Char { get { return c; } }
		char c;

		/// <summary>
		/// 文字情報
		/// </summary>
		public CharacterInfo CharInfo { get { return charInfo; } }
		CharacterInfo charInfo;

		/// <summary>
		/// 描画スプライト
		/// </summary>
		public Sprite Sprite { get { return sprite; } }
		Sprite sprite;

		/// <summary>
		/// 描画オフセットY
		/// </summary>
		public float OffsetY { get { return offset.y; } }

		/// <summary>
		/// 描画オフセットXを取得
		/// </summary>
		/// <param name="isKerning">カーニングするか</param>
		/// <returns>描画オフセットX</returns>
		public float GetOffsetX(bool isKerning) { return isKerning ? kerningOffsetX : offset.x; }

		/// <summary>
		/// 描画幅を取得
		/// </summary>
		/// <param name="isKerning">カーニングするか</param>
		/// <returns>描画幅</returns>
		public float GetWidth(bool isKerning) { return isKerning ? kerningWidth : width; }

		Vector3 offset;

		float width;
		float kerningWidth;

		float kerningOffsetX;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="c">文字</param>
		/// <param name="info">文字情報</param>
		/// <param name="sprite">描画スプライト</param>
		/// <param name="offsetY">フォントに設定されているオフセット値Y</param>
		public FontRenderInfo(char c, CharacterInfo info, Sprite sprite, float offsetY, float fontSize)
		{
			this.c = c;
			this.charInfo = info;
			this.sprite = sprite;

			//中心を原点とした場合の、表示位置
			WrapperUnityVersion.SetFontRenderInfo( c, ref info, offsetY, fontSize, out offset, out width, out kerningWidth );
			if (kerningWidth == 0)
			{
				kerningWidth = width;
			}
			kerningOffsetX = kerningWidth / 2;
		}
	}
}