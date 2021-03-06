//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Utage
{
	/// <summary>
	/// 指定された名前のenumだけ表示する[LimitEnum]のためのプロパティ拡張
	/// </summary>
	[CustomPropertyDrawer(typeof(LimitEnumAttribute))]
	public class LimitEnumDrawer : PropertyDrawer
	{
		LimitEnumAttribute Attribute { get { return (this.attribute as LimitEnumAttribute); } }

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			string lastName = property.enumNames[property.intValue];
			int lastIndex = ArrayUtility.FindIndex<string>(Attribute.Args, (x) => (x == lastName));
			int index = EditorGUI.Popup(position, label.text, lastIndex, Attribute.Args);
			if (lastIndex != index)
			{
				property.intValue = ArrayUtility.FindIndex<string>(property.enumNames, (x) => (x == Attribute.Args[index]));
			}
		}
	}
}
