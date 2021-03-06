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
	[CustomEditor(typeof(Node2D))]
	public class Node2DInspector : Editor
	{


		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			DrawProperties();
			serializedObject.ApplyModifiedProperties();
		}

		public virtual void DrawProperties()
		{
			DrawNode2DProperties();
		}

		public void DrawNode2DProperties()
		{
			Node2D obj = target as Node2D;

			UtageEditorToolKit.BeginGroup("Node2D");

			//色
			UtageEditorToolKit.BeginGroup("Color");
			UtageEditorToolKit.PropertyField(serializedObject, "isLinkColor", "Link parent");
			UtageEditorToolKit.PropertyField(serializedObject, "localColor", "Color");
			UtageEditorToolKit.EndGroup();

			//ソートデータの設定
			UtageEditorToolKit.BeginGroup("Sort");
			UtageEditorToolKit.PropertyField(serializedObject, "isLinkSorting2D", "Link parent");
//			UtageEditorToolKit.PropertyField(serializedObject, "sortData", "Data prefab");
			//値キーのポップアップ表示
			Node2DSortData.DictionarySortData2D dic = Node2DSortData.Instance.Dictionary;
			List<string> items = new List<string>();
			items.Add(Node2DSortData.KeyNone);
			foreach (Node2DSortData.DictionaryKeyValueSortData2D keyValue in dic.List)
			{
				items.Add(keyValue.Key);
			}
			int currentIndex = items.FindIndex(item => (item == obj.SortKey));
			int index = EditorGUILayout.Popup(currentIndex, items.ToArray());
			if (index != currentIndex)
			{
				Undo.RecordObject(obj, "DefineZ Change");
				obj.SortKey = items[index];
				EditorUtility.SetDirty(target);
			}

			//描画レイヤー・描画順
			EditorGUI.BeginDisabledGroup(!obj.IsEmptySortData);
			UtageEditorToolKit.PropertyField(serializedObject, "localSortingLayer", "Sorting Layer");
			UtageEditorToolKit.PropertyField(serializedObject, "localOrderInLayer", "Order in Layer");
			EditorGUILayout.LabelField("Z", "" + obj.CachedTransform.localPosition.z);
			EditorGUI.EndDisabledGroup();

			UtageEditorToolKit.EndGroup();

			UtageEditorToolKit.EndGroup();
		}
	}
}