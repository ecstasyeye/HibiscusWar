//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Utage
{

	[CustomEditor(typeof(Node2DSortData))]
	public class Node2DSortDataInspector : Editor
	{

		public override void OnInspectorGUI()
		{
			Node2DSortData obj = target as Node2DSortData;
			EditorGUILayout.BeginVertical();

			//Z値キーのポップアップ表示
			Node2DSortData.DictionarySortData2D dic = obj.Dictionary;
			//		List<string> items = new List<string>();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Key", GUILayout.MinWidth(100f));
			EditorGUILayout.LabelField("Sorting Layer", GUILayout.MinWidth(50f));
			EditorGUILayout.LabelField("Order", GUILayout.MinWidth(40f));
			EditorGUILayout.LabelField("Z", GUILayout.MinWidth(40f));
			EditorGUILayout.LabelField("", GUILayout.MinWidth(95f));
			EditorGUILayout.EndHorizontal();

			for (int i = 0; i < dic.List.Count; ++i)
			{
				Node2DSortData.DictionaryKeyValueSortData2D keyValue = dic.List[i];
				EditorGUILayout.BeginHorizontal();
				//キー
				string key = EditorGUILayout.TextField(keyValue.Key, GUILayout.MinWidth(100));
				if (key != keyValue.Key && key != Node2DSortData.KeyNone )
				{
					if (!dic.ContainsKey(key))
					{
						Undo.RecordObject(obj, "DefineZData Change Key");
						keyValue.InitKey(key);
						dic.RefreshDictionary();
						EditorUtility.SetDirty(target);
					}
					else
					{
						Debug.LogError(key + ": contains same key");
					}
				}
				Node2DSortData.SortData2D data = keyValue.value;

				//レイヤー名
				string sortingLayer = EditorGUILayout.TextField(data.sortingLayer, GUILayout.MinWidth(50f));
				if (sortingLayer != data.sortingLayer)
				{
					Undo.RecordObject(obj, "DefineZData Change Value");
					data.sortingLayer = sortingLayer;
					dic.RefreshDictionary();
					EditorUtility.SetDirty(target);
				}

				//順番
				int orderInLayer = EditorGUILayout.IntField(data.orderInLayer, GUILayout.MinWidth(40f));
				if (orderInLayer != data.orderInLayer)
				{
					Undo.RecordObject(obj, "DefineZData Change Value");
					data.orderInLayer = orderInLayer;
					dic.RefreshDictionary();
					EditorUtility.SetDirty(target);
				}

				//Z値
				float z = EditorGUILayout.FloatField(data.z, GUILayout.MinWidth(40f));
				if (z != data.z)
				{
					Undo.RecordObject(obj, "DefineZData Change Value");
					data.z = z;
					dic.RefreshDictionary();
					EditorUtility.SetDirty(target);
				}

				//一つ上へボタン
				if (GUILayout.Button("Up", GUILayout.Width(30f)))
				{
					Undo.RecordObject(obj, "DefineZData Up");
					dic.Swap(i, i - 1);
					EditorUtility.SetDirty(target);
					break;
				}

				//一つ上へボタン
				if (GUILayout.Button("Down", GUILayout.Width(45f)))
				{
					Undo.RecordObject(obj, "DefineZData Down");
					dic.Swap(i, i + 1);
					EditorUtility.SetDirty(target);
					break;
				}

				//削除ボタン
				if (GUILayout.Button("X", GUILayout.Width(20f)))
				{
					Undo.RecordObject(obj, "DefineZData Remove");
					dic.Remove(keyValue.Key);
					EditorUtility.SetDirty(target);
					break;
				}
				EditorGUILayout.EndHorizontal();
			}

			//追加ボタン
			if (GUILayout.Button("Add"))
			{
				Undo.RecordObject(obj, "DefineZData Add");

				int count = dic.Count - 1;
				string newKey;
				do
				{
					++count;
					newKey = "Key " + count;
				} while (dic.ContainsKey(newKey));

				Node2DSortData.DictionaryKeyValueSortData2D keyVal = new Node2DSortData.DictionaryKeyValueSortData2D();
				keyVal.Init(newKey, new Node2DSortData.SortData2D());
				dic.Add(keyVal);
				EditorUtility.SetDirty(target);
			}

			EditorGUILayout.EndVertical();

		}
	}
}