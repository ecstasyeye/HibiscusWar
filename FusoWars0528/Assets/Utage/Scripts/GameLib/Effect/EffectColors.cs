//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using Utage;
using System.Collections;
using System;


namespace Utage
{
	//エフェクト用のカラーを配列で持つクラス
	public class EffectColors
	{
		public enum Index
		{
			TweenColor,	//Tweenに使うカラー
			Color1,
			Color2,
			Color3,
		};


		public delegate void onValueChanged(EffectColors colors);
		public onValueChanged OnValueChanged;

		//全てのカラーを乗算したカラー値
		public Color MulColor { get {return mulColor;} }
		Color mulColor = Color.white;

		//カラー配列
		Color[] colors = new Color[4] { Color.white, Color.white, Color.white, Color.white };

		//エフェクトによるカラー値を取得する
		public Color GetColor(Index index)
		{
			return colors[(int)index];
		}

		//エフェクトによるカラー値を設定する
		public void SetColor(Index index, Color color)
		{
			colors[(int)index] = color;
			mulColor = Color.white;
			foreach (Color c in colors)
			{
				mulColor *= c;
			}
			if (OnValueChanged != null) OnValueChanged(this);
		}

		const int Version = 0;
		internal void Write(System.IO.BinaryWriter writer)
		{
			writer.Write(Version);
			writer.Write(colors.Length);
			foreach (Color color in colors)
			{
				UtageToolKit.WriteColor(color,writer);
			}
			UtageToolKit.WriteColor(mulColor, writer);
		}

		internal void Read(System.IO.BinaryReader reader)
		{
			int version = reader.ReadInt32();
			if (version == Version)
			{
				int count = reader.ReadInt32();
				for (int i = 0; i < count; ++i)
				{
					colors[i] = UtageToolKit.ReadColor(reader);
				}
				mulColor =  UtageToolKit.ReadColor(reader);
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, version));
			}
		}
	};	


}