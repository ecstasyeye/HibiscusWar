//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Utage
{

	/// <summary>
	/// キャラクタの表示情報
	/// </summary>
	public class AdvCharacterInfo
	{
		public AdvCharacterInfo( string label, bool isHide, bool isNonePattern )
		{
			this.label = label;
			this.isHide = isHide;
			this.isNonePattern = isNonePattern;
		}

		public string Label
		{
			get { return label; }
		}
		string label;

		public string NameText{ get; set; }

		public GraphicInfoList Graphic
		{
			get { return graphic; }
			set { graphic = value; }
		}
		GraphicInfoList graphic;
/*
		public GraphicInfoList IconGraphic
		{
			get { return iconGraphic; }
			set { iconGraphic = value; }
		}
		GraphicInfoList iconGraphic;
*/		
		public string ErrorMsg
		{
			get { return errorMsg; }
			set { errorMsg = value; }
		}
		string errorMsg;


		public bool IsHide
		{
			get { return isHide; }
		}
		bool isHide;

		public bool IsNonePattern
		{
			get { return isNonePattern; }
		}
		bool isNonePattern;
	}
}
