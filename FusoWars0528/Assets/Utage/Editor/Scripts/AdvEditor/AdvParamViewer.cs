//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Utage
{
	//宴のビューワー表示ウィンドウ
	public class AdvParamViewer : CustomEditorWindow
	{
		void OnEnable()
		{
			//シーン変更で描画をアップデートする
			this.autoRepaintOnSceneChange = true;
			//スクロールを有効にする
			this.isEnableScroll = true;
		}

		protected override void OnGUISub()
		{
			AdvEngine engine = UtageEditorToolKit.FindComponentAllInTheScene<AdvEngine>();
			if (engine == null)
			{
				UtageEditorToolKit.BoldLabel("Not found AdvEngine");
			}
			else
			{
				OnGuiParam(engine);
			}
		}

		//パラメーターの表示
		void OnGuiParam(AdvEngine engine)
		{
			if (!engine.Param.IsInit) return;

			OnGuiParamDefault( engine.Param.GetDefault () );
			OnGuiStructArray(engine);
		}

		const int CellHeight = 16;
		const int LineHeight = CellHeight+2;
		GUILayoutOption paramHeight = GUILayout.Height(CellHeight);
		GUILayoutOption paramWidth = GUILayout.Width(96);

		//高速表示可能なスクロールビュー
		Dictionary<string, OptimizedScrollView> scrollViewDic = new Dictionary<string,OptimizedScrollView>();

		OptimizedScrollView GetScrollViewCreateIfMissing(string name)
		{
			OptimizedScrollView  scrollView;
			if( !scrollViewDic.TryGetValue(name, out scrollView ) )
			{
				scrollView = new OptimizedScrollView(name,this, LineHeight, CellHeight);
				scrollViewDic.Add(name, scrollView);
			}
			return scrollView;
		}


		//通常パラメーターの表示
		void OnGuiParamDefault(AdvParamStruct paramDefault)
		{
			{
				List<AdvParamData> list = paramDefault.GetFileTypeList(AdvParamData.FileType.Default);
				GetScrollViewCreateIfMissing("Default").OnGui(list.Count, (x) => OnGuiParamData(list[x]));
			}
			{
				List<AdvParamData> list = paramDefault.GetFileTypeList(AdvParamData.FileType.System);
				GetScrollViewCreateIfMissing("System").OnGui(list.Count, (x) => OnGuiParamData(list[x]));
			}
		}

		//通常パラメーターの表示
		void OnGuiParamData(AdvParamData data)
		{
			switch (data.Type)
			{
				case AdvParamData.ParamType.Float:
					float f = EditorGUILayout.FloatField(data.Key, (float)data.Parameter, paramHeight);
					if (f != (float)data.Parameter)
					{
						data.Parameter = f;
					}
					break;
				case AdvParamData.ParamType.Int:
					int i = EditorGUILayout.IntField(data.Key, (int)data.Parameter, paramHeight); ;
					if (i != (int)data.Parameter)
					{
						data.Parameter = i;
					}
					break;
				case AdvParamData.ParamType.Bool:
					bool b = EditorGUILayout.Toggle(data.Key, (bool)data.Parameter, paramHeight);
					if (b != (bool)data.Parameter)
					{
						data.Parameter = b;
					}
					break;
				case AdvParamData.ParamType.String:
					string s = EditorGUILayout.TextField(data.Key, (string)data.Parameter, paramHeight);
					if (s != (string)data.Parameter)
					{
						data.Parameter = s;
					}
					break;
			}
		}

		void OnGuiStructArray(AdvEngine engine)
		{
			foreach( var item in engine.Param.StructTbl )
			{
				string key = item.Key;
				if (AdvParamManager.SheetNameParam == key) continue;

				OnGuiStructArray(key, item.Value.Tbl);
			}
		}

		//配列パラメーターの表示
		void OnGuiStructArray(string name, Dictionary<string, AdvParamStruct> tbl)
		{
			if(tbl.Count == 0 ) return;

			string[] keys = new string[tbl.Count];
			tbl.Keys.CopyTo( keys,0);

			GetScrollViewCreateIfMissing(name).OnGui(
				tbl.Count,
				()=>DrawStructArrayHeader( tbl[ keys[0]]),
				(x) => OnGuiStructArrayValues(keys[x], tbl[keys[x]]
					));
			;
		}

		//構造体配列のヘッダ部分表示
		void DrawStructArrayHeader(AdvParamStruct advParamStruct)
		{
			GUILayout.BeginVertical();

			//名前
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Label", paramHeight, paramWidth);
			foreach (var item in advParamStruct.Tbl)
			{
				AdvParamData data = item.Value;
				EditorGUILayout.LabelField(data.Key, paramHeight, paramWidth);
			}
			GUILayout.EndHorizontal();

			//型
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Type", paramHeight, paramWidth);
			foreach (var item in advParamStruct.Tbl)
			{
				AdvParamData data = item.Value;
				EditorGUILayout.LabelField(data.Type.ToString(), paramHeight, paramWidth);
			}
			GUILayout.EndHorizontal();


			//ファイルタイプ
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("FileType", paramHeight, paramWidth);
			foreach (var item in advParamStruct.Tbl)
			{
				AdvParamData data = item.Value;
				EditorGUILayout.LabelField(data.SaveFileType.ToString(), paramHeight, paramWidth);
			}
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();

		}

		//構造体配列の値を表示
		void OnGuiStructArrayValues(string key, AdvParamStruct param)
		{
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(key, paramHeight, paramWidth);
			foreach (var item in param.Tbl)
			{
				AdvParamData data = item.Value;
				switch (data.Type)
				{
					case AdvParamData.ParamType.Float:
						float f = EditorGUILayout.FloatField((float)data.Parameter, paramHeight, paramWidth);
						if (f != (float)data.Parameter)
						{
							data.Parameter = f;
						}
						break;
					case AdvParamData.ParamType.Int:
						int i = EditorGUILayout.IntField((int)data.Parameter, paramHeight, paramWidth);
						if (i != (int)data.Parameter)
						{
							data.Parameter = i;
						}
						break;
					case AdvParamData.ParamType.Bool:
						bool b = EditorGUILayout.Toggle((bool)data.Parameter, paramHeight, paramWidth);
						if (b != (bool)data.Parameter)
						{
							data.Parameter = b;
						}
						break;
					case AdvParamData.ParamType.String:
						string s = EditorGUILayout.TextField((string)data.Parameter, paramHeight, paramWidth);
						if (s != (string)data.Parameter)
						{
							data.Parameter = s;
						}
						break;
				}
			}
			GUILayout.EndHorizontal();
		}
	}
}
