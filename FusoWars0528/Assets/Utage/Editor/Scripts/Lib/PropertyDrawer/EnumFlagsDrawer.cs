//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Utage
{
	/// <summary>
	/// [EnumFlags]を表示するためのプロパティ拡張
	/// </summary>
	[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
	public class EnumFlagsDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.intValue = EditorGUI.MaskField( position, label, property.intValue, property.enumNames );
		}
	}
}
