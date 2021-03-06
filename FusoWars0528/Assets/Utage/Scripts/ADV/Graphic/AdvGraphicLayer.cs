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
	/// グラフィックのレイヤー管理
	/// </summary>
	[AddComponentMenu("Utage/ADV/Internal/GraphicLayer")]
	public class AdvGraphicLayer : MonoBehaviour
	{
		public AdvGraphicManager Manager { get { return manager; } }
		AdvGraphicManager manager;		

		public AdvLayerSettingData SettingData { get { return settingData; } }
		AdvLayerSettingData settingData;

		public AdvGraphicObject DefaultObject { get { return defaultObject; } }
		AdvGraphicObject defaultObject;
		Dictionary<string, AdvGraphicObject> currentGraphics = new Dictionary<string, AdvGraphicObject>();


		//初期化
		public void Init(AdvGraphicManager manager, AdvLayerSettingData settingData)
		{
			this.manager = manager;
			this.settingData = settingData;
			this.transform.localPosition = new Vector3(SettingData.Center.x / manager.PixelsToUnits, SettingData.Center.y / manager.PixelsToUnits, SettingData.GetZ( manager.SortOderToZUnits ) );
			if (!string.IsNullOrEmpty(SettingData.LayerMask))
			{
				gameObject.layer = LayerMask.NameToLayer(SettingData.LayerMask);
			}
		}

		//オブジェクトを描画する
		internal AdvGraphicObject Draw(string name, AdvGraphicOperaitonArg arg )
		{
			AdvGraphicObject obj = GetObjectCreateIfMissing(name, arg);
			obj.Draw(arg);
			return obj;
		}

		//デフォルトオブジェクトとして描画する
		internal AdvGraphicObject DrawToDefault(string name, AdvGraphicOperaitonArg arg)
		{
			//デフォルトオブジェクトの名前が違うなら、そのオブジェクトを消す
			if (DefaultObject != null && DefaultObject.name != name)
			{
				//フェードアウトする
				FadeOut(DefaultObject.name, arg.FadeTime);
			}
			defaultObject = GetObjectCreateIfMissing(name,arg);
			defaultObject.Draw(arg);
			return defaultObject;
		}

		//指定の名前のオブジェクトを取得、なければ作成
		internal AdvGraphicObject GetObjectCreateIfMissing(string name, AdvGraphicOperaitonArg arg)
		{
			AdvGraphicObject obj;
			if (currentGraphics.TryGetValue(name, out obj))
			{
				if ( obj.Graphic.FileType != arg.Graphic.FileType)
				{
					//違うファイルタイプなので、前のはいったん消す
					FadeOut( name, arg.FadeTime);
					//改めて作成
					obj = AddObject(name, arg.Graphic, false);
				}
			}
			else
			{
				//まだ作成されてないから作る
				obj = AddObject(name, arg.Graphic, false);
			}
			return obj;
		}

		//指定の名前のオブジェクトを取得、なければ作成
		internal AdvGraphicObject AddObject(string name, GraphicInfoList grapic, bool isDefault )
		{
			Type type = AdvGraphicObjectFactory.Create(grapic.FileType);
			GameObject go = UtageToolKit.AddChildGameObject(this.transform, name);
			AdvGraphicObject obj = go.AddComponent(type) as AdvGraphicObject;
			obj.Init(this);
			currentGraphics.Add(name, obj);
			if(isDefault) defaultObject = obj;
			return obj;
		}

		//フェードアウト
		internal void FadeOut(string name, float fadeTime)
		{
			AdvGraphicObject obj;
			if (currentGraphics.TryGetValue(name, out obj))
			{
				obj.FadeOut(fadeTime);
				currentGraphics.Remove(name);
				if (defaultObject == obj)
				{
					defaultObject = null;
				}
			}
		}

		internal void FadeOutAll(float fadeTime)
		{
			foreach (var obj in currentGraphics.Values)
			{
				obj.FadeOut(fadeTime);
			}
			currentGraphics.Clear();
			defaultObject = null;
		}

		//クリア処理
		internal void Clear()
		{
			foreach (var obj in currentGraphics.Values)
			{
				obj.Clear();
			}
			currentGraphics.Clear();
			defaultObject = null;
		}

		//デフォルトグラフィックオブジェクトの名前が指定名と同じかチェック
		internal bool IsEqualDefaultGraphicName(string name)
		{
			if (DefaultObject!=null)
			{
				return DefaultObject.name == name;
			}
			return false;
		}

		//指定名のオブジェクトがあるか
		internal bool Contains(string name)
		{
			return currentGraphics.ContainsKey(name);
		}

		//指定名のオブジェクトがあれば返す
		internal AdvGraphicObject Find(string name)
		{
			AdvGraphicObject obj;
			if(currentGraphics.TryGetValue(name,out obj))
			{
				return obj;
			}
			return null;
		}


		internal void AddAllGraphics(List<AdvGraphicObject> graphics)
		{
			graphics.AddRange(currentGraphics.Values);
		}

		//（古いバージョンのデータを読み込むときに使う）
		internal void SetDefault(string defualtSpriteName)
		{
			AdvGraphicObject obj = Find(defualtSpriteName);
			if(obj!=null)
			{
				defaultObject = obj;
			}
		}

		//旧バージョン（LayerManagerだったころ）のレイヤーデータ読み込み
		internal void ReadOld(BinaryReader reader)
		{
			//Transofom,Colorを空読み込み
			Vector3 pos = new Vector3();
			Vector3 euler = new Vector3();
			Vector3 scale = new Vector3();
			UtageToolKit.ReadLocalTransform(reader, out pos, out euler, out scale);
			UtageToolKit.ReadColor(reader);

			//Tween情報を空読み込み
			int tweenCount = reader.ReadInt32();
			for (int i = 0; i < tweenCount; ++i)
			{
				AdvTweenPlayer tween = this.gameObject.AddComponent<AdvTweenPlayer>() as AdvTweenPlayer;
				tween.Read(reader, Manager.PixelsToUnits);
				Destroy(tween);
			}

			//各スプライトの読み込み
			int count = reader.ReadInt32();
			for (int i = 0; i < count; ++i)
			{
				string name = reader.ReadString();
				GameObject go = UtageToolKit.AddChildGameObject(this.transform, name);
				AdvGraphicObject2DSprite obj = go.AddComponent<AdvGraphicObject2DSprite>();
				obj.Init(this);
				currentGraphics.Add(name, obj);
				obj.ReadOld(reader);
			}
			//デフォルトオブジェクトを設定
			defaultObject = Find(reader.ReadString());
		}

		//ロード中かチェック
		internal bool IsLoading
		{
			get
			{
				foreach (AdvGraphicObject obj in currentGraphics.Values)
				{
					if (obj.IsLoading) return true;
				}
				return false;
			}
		}

	}
}
