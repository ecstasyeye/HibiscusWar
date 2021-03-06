//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Utage
{

	/// <summary>
	/// グラフィック描画のための引数としてのデータ
	/// </summary>
	public class AdvGraphicOperaitonArg
	{
		public float FadeTime { get; private set; }
		public bool IsNoPtattern { get; private set; }
		public StringGridRow RowData { get; private set; }
		public GraphicInfoList Graphic { get; private set; }
		public string Arg2 { get; private set; }
		public string Arg3 { get; private set; }
		public string Arg4 { get; private set; }
		public string Arg5 { get; private set; }

		public float? X { get; private set; }
		public float? Y { get; private set; }

		public bool IsPostionArgs { get { return (X != null) || (Y != null); } }
		public MotionPlayType PlayType { get; private set; }

		internal AdvGraphicOperaitonArg(AdvCommand command, GraphicInfoList graphic, float fadeTime, bool isNoPtattern = false)
		{
			this.RowData = command.RowData;
			this.Graphic = graphic;
			this.FadeTime = fadeTime;
			this.IsNoPtattern = isNoPtattern;

			this.Arg2 = command.ParseCellOptional<string>(AdvColumnName.Arg2, "");
			this.Arg3 = command.ParseCellOptional<string>(AdvColumnName.Arg3, "");
			this.Arg4 = command.ParseCellOptional<string>(AdvColumnName.Arg4, "");
			this.Arg5 = command.ParseCellOptional<string>(AdvColumnName.Arg5, "");
			float x;
			if (float.TryParse(Arg4, out x))
			{
				this.X = x;
			}
			float y;
			if (float.TryParse(Arg5, out y))
			{
				this.Y = y;
			}
			else
			{
				MotionPlayType type;
				if (UtageToolKit.TryParaseEnum<MotionPlayType>(Arg5, out type))
				{
					PlayType = type;
				}
			}
		}
	}
}
