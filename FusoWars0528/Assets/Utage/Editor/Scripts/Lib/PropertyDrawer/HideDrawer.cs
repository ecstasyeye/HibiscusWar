//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Utage
{
	/// <summary>
	/// [Hide]を表示するためのプロパティ拡張
	/// 条件によって非表示にする
	/// </summary>
	[CustomPropertyDrawer(typeof(HideAttribute))]
	public class HideDrawer : PropertyDrawer
	{
		HideAttribute Attribute { get { return (this.attribute as HideAttribute); } }

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!IsHide(property))
			{
				EditorGUI.PropertyField(position, property, label);
			}
		}

		bool IsHide(SerializedProperty property)
		{
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
					return enalePropery.boolValue ^ Attribute.IsEnableProperty;
				}
				else
				{
					Debug.LogError("Not found " + path);
				}
			}
			return true;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (IsHide(property))
			{
				return 0;
			}
			else
			{
				return base.GetPropertyHeight(property, label);
			}
		}
	}
}
