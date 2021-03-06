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
	/// グラフィックオブジェクトのデータ
	/// </summary>
	public class AdvGraphicObjectFactory
	{
		//独自オブジェクトを作成するためのコールバック
		//独自にカスタムしたい、ファイルタイプのオブジェクトの型だけ作成すればいい
		public delegate void CreateCustom(string fileType, ref Type type);
		public static CreateCustom CallbackCreateCustom;
		
		internal static Type Create(string graphicFileType)
		{
			if (CallbackCreateCustom!=null)
			{
				Type type = null;
				CallbackCreateCustom(graphicFileType, ref type);
				if (type != null) return type;
			}

			switch(graphicFileType)
			{
				case "3D":
					return typeof(AdvGraphicObject3DModel);
				case "2D":
				default:
					return typeof(AdvGraphicObject2DSprite);
			}
		}
	}
}
