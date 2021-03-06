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
	/// 表示のみで編集を不可能にする
	/// </summary>
	[CustomPropertyDrawer(typeof(NotEditableAttribute))]
	public class NotEditableDrawer : PropertyDrawer
	{
		NotEditableAttribute Attribute { get { return (this.attribute as NotEditableAttribute); } }

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			bool isNotEditable = true;
			if (!string.IsNullOrEmpty(Attribute.EnablePropertyPath))
			{
				string path = "";
				//子のプロパティの場合ルートのパスが必要になる
				int lastIndex = property.propertyPath.LastIndexOf('.');
				if (lastIndex > 0)
				{
					path += property.propertyPath.Substring(0, lastIndex) + ".";
				}
				path += Attribute.EnablePropertyPath;

				SerializedProperty enalePropery = property.serializedObject.FindProperty(path);
				if (enalePropery != null)
				{
					isNotEditable = enalePropery.boolValue^Attribute.IsEnableProperty;
				}
				else
				{
					Debug.LogError("Not found " + path);
				}
			}
			EditorGUI.BeginDisabledGroup(isNotEditable);
			EditorGUI.PropertyField(position, property, label);
			EditorGUI.EndDisabledGroup();
		}
	}
}
