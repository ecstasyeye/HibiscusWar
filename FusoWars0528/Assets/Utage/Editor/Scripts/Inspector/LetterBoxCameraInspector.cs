//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UguiLetterBoxCamera))]
	public class LetterBoxCameraInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			DrawProperties();
			serializedObject.ApplyModifiedProperties();
		}

		void DrawProperties()
		{
			UguiLetterBoxCamera obj = target as UguiLetterBoxCamera;

			UtageEditorToolKit.PropertyField(serializedObject, "pixelsToUnits", "Pixels To Units");
			if (obj.PixelsToUnits < 1) obj.PixelsToUnits = 1;

			//基本画面サイズ
			UtageEditorToolKit.PropertyField(serializedObject, "width", "Game Screen Width" );
			if (obj.Width < 1) obj.Width = 1;
			UtageEditorToolKit.PropertyField(serializedObject, "height", "Game Screen Height" );
			if (obj.Height < 1) obj.Height = 1;

			//基本画面サイズ
			UtageEditorToolKit.BeginGroup ("Flexible",serializedObject, "isFlexible");
			if(obj.IsFlexible)
			{
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Wide  ", GUILayout.Width(50));
				UtageEditorToolKit.PropertyField(serializedObject, "maxWidth", "", GUILayout.Width(50));
				if (obj.MaxWidth < obj.Width) obj.MaxWidth = obj.Width;
				EditorGUILayout.LabelField(" x " + obj.Height, GUILayout.Width(50));
//				UtageEditorToolKit.PropertyField(serializedObject, "minHeight", "Height");
//				obj.MinHeight = Mathf.Clamp(obj.MinHeight, 1, obj.Height);
				GUILayout.EndHorizontal();

				GUILayout.Space(4f);
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Nallow  ", GUILayout.Width(50));
//				UtageEditorToolKit.PropertyField(serializedObject, "minWidth", "Width");
//				obj.MinWidth = Mathf.Clamp(obj.MinWidth, 1, obj.Width);
				EditorGUILayout.LabelField("" + obj.Width + " x ", GUILayout.Width(50));

				UtageEditorToolKit.PropertyField(serializedObject, "maxHeight", "", GUILayout.Width(50));
				if (obj.MaxHeight < obj.Height) obj.MaxHeight = obj.Height;
				GUILayout.EndHorizontal();
			}
			UtageEditorToolKit.EndGroup();

			EditorGUILayout.LabelField("Current Size = " +  obj.CurrentWidth +" x " + obj.CurrentHeight);

			UtageEditorToolKit.PropertyField(serializedObject, "anchor", "Anchor");
		}
	}
}

 