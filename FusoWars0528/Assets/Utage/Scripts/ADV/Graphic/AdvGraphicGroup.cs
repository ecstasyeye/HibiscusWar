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
	/// グラフィックオブジェクトを、キャラクターやBGなどのグループ単位で管理のためのスーパークラス
	/// </summary>
	public class AdvGraphicGroup
	{
		protected AdvLayerSettingData.LayerType type;
		protected AdvGraphicLayer defaultLayer;

		List<AdvGraphicLayer> layers = new List<AdvGraphicLayer>();

		//起動時の初期化
		internal AdvGraphicGroup( AdvLayerSettingData.LayerType type, AdvLayerSetting setting, AdvGraphicManager manager )
		{
			this.type = type;
			foreach (var item in setting.List)
			{
				if (item.Type == type)
				{
					AdvGraphicLayer layer = UtageToolKit.AddChildGameObjectComponent<AdvGraphicLayer>(manager.transform, item.Name);
					layer.Init(manager,item);
					layers.Add(layer);
					if (item.IsDefault) defaultLayer = layer;
				}
			}
		}

		//クリア
		internal virtual void Clear()
		{
			foreach (AdvGraphicLayer layer in layers)
			{
				layer.Clear();
			}
		}

		internal void DestroyAll()
		{
			foreach (AdvGraphicLayer layer in layers)
			{
				layer.Clear();
				UtageToolKit.SafeDestroy(layer.gameObject);
			}
			layers.Clear();
			defaultLayer = null;
		}

		//表示する
		internal AdvGraphicObject Draw(string layerName, string name, AdvGraphicOperaitonArg arg)
		{
			return FindLayerOrDefault(layerName).Draw(name, arg);
		}

		//デフォルトレイヤーのデフォルトオブジェクトとして表示する
		internal AdvGraphicObject DrawToDefault(string name, AdvGraphicOperaitonArg arg)
		{
			return defaultLayer.DrawToDefault(name, arg);
		}

		//キャラクターオブジェクトとして、特殊な表示をする
		internal AdvGraphicObject DrawCharacter(string layerName, string name, AdvGraphicOperaitonArg arg)
		{
			//既に同名のグラフィックがあるなら、そのレイヤーを取得
			AdvGraphicLayer oldLayer = layers.Find((item) => (item.IsEqualDefaultGraphicName(name)));

			//レイヤー名の指定がある場合、そのレイヤーを探す
			AdvGraphicLayer layer = layers.Find((item) => (item.SettingData.Name == layerName));
			if (layer == null)
			{
				//レイヤーがない場合は、旧レイヤーかデフォルトレイヤーを使う
				layer = (oldLayer == null) ? defaultLayer : oldLayer;
			}

			//レイヤーが変わる場合は、昔のほうを消す
			if (oldLayer != layer && oldLayer != null)
			{
				oldLayer.FadeOut(name, arg.FadeTime );
			}

			//レイヤー上にデフォルトオブジェクトとして表示
			return layer.DrawToDefault(name, arg);
		}


		//指定名のオブジェクトを非表示（フェードアウト）する
		internal virtual void FadeOut(string name, float fadeTime)
		{
			AdvGraphicLayer layer = FindLayerFromObjectName(name);
			if (layer != null) layer.FadeOut(name, fadeTime);
		}

		//全オブジェクトを非表示（フェードアウト）する
		internal virtual void FadeOutAll(float fadeTime)
		{
			foreach (AdvGraphicLayer layer in layers)
			{
				layer.FadeOutAll(fadeTime);
			}
		}

		//指定名グラフィックオブジェクトをデフォルトに持つレイヤーがあるかどうか
		internal bool IsContiansDefalutGraphic(string name)
		{
			return layers.Exists((item) => (item.IsEqualDefaultGraphicName(name)));
		}

		//指定の名前のグラフィックオブジェクトを持つレイヤーを探す
		internal AdvGraphicLayer FindLayerFromObjectName(string name)
		{
			foreach( AdvGraphicLayer layer in layers )
			{
				if( layer.Contains(name) ) return layer;
			}
			return null;
		}

		//指定の名前のレイヤーを探す
		internal AdvGraphicLayer FindLayer(string name)
		{
			return layers.Find((item) => (item.SettingData.Name == name));
		}

		//指定の名前のレイヤーを探す（見つからなかったらデフォルト）
		internal AdvGraphicLayer FindLayerOrDefault(string name)
		{
			return layers.Find((item) => (item.SettingData.Name == name)) ?? defaultLayer;
		}

		//指定の名前のグラフィックオブジェクトをを探す
		internal AdvGraphicObject FindObject(string name)
		{
			foreach (AdvGraphicLayer layer in layers)
			{
				 AdvGraphicObject obj = layer.Find(name);
				 if (obj!=null) return obj;
			}
			return null;
		}

		//全てのグラフィックオブジェクトを取得
		internal List<AdvGraphicObject> AllGraphics()
		{
			List<AdvGraphicObject> allGraphics = new List<AdvGraphicObject>();
			foreach (AdvGraphicLayer layer in layers)
			{
				layer.AddAllGraphics(allGraphics);
			}
			return allGraphics;
		}

		internal void AddAllGraphics(List<AdvGraphicObject> graphics)
		{
			foreach (AdvGraphicLayer layer in layers)
			{
				layer.AddAllGraphics(graphics);
			}
		}

		//ロード中かチェック
		internal bool IsLoading
		{
			get
			{
				foreach (AdvGraphicLayer layer in layers)
				{
					if (layer.IsLoading) return true;
				}
				return false;
			}
		}
	}
}