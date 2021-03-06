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
	/// グラフィックの管理
	/// </summary>
	[AddComponentMenu("Utage/ADV/GraphicManager")]
	public class AdvGraphicManager : MonoBehaviour
	{
		public Camera Camera { get { return renderCamera ?? (renderCamera = FindObjectOfType<UguiLetterBoxCamera>().CachedCamera); } }
		[SerializeField]
		Camera renderCamera;

		/// <summary>
		/// スプライトを作成する際の、座標1.0単位辺りのピクセル数
		/// </summary>
		public float PixelsToUnits { get { return pixelsToUnits; } }
		[SerializeField]
		float pixelsToUnits = 100;

		/// <summary>
		/// Z座標1.0単位辺りのSortingOrderの数
		/// </summary>
		public float SortOderToZUnits { get { return sortOderToZUnits; } }
		[SerializeField]
		float sortOderToZUnits = 100;

		public string BgSpriteName
		{
			get { return bgSpriteName; }
		}
		[SerializeField]
		string bgSpriteName = "BG";

		/// <summary>
		/// イベントモード（キャラクター立ち絵非表示）
		/// </summary>
		public bool IsEventMode { get { return this.isEventMode; } set { isEventMode = value; } }
		bool isEventMode;

		/// <summary>
		/// キャラクター管理
		/// </summary>
		public AdvGraphicGroup CharacterManager { get { return this.allGruop[(int)(AdvLayerSettingData.LayerType.Character)]; } }

		/// <summary>
		/// スプライト管理
		/// </summary>
		public AdvGraphicGroup SpriteManager { get { return this.allGruop[(int)(AdvLayerSettingData.LayerType.Sprite)]; } }

		/// <summary>
		/// スプライト管理
		/// </summary>
		public AdvGraphicGroup BgManager { get { return this.allGruop[(int)(AdvLayerSettingData.LayerType.Bg)]; } }

		/// <summary>
		/// 全てのグループ
		/// </summary>
		List<AdvGraphicGroup> allGruop = new List<AdvGraphicGroup>();

		internal AdvEngine Engine { get { return engine; } }
		AdvEngine engine;

		/// <summary>
		/// 起動時初期化
		/// </summary>
		/// <param name="setting">レイヤー設定データ</param>
		public void BootInit(AdvEngine engine, AdvLayerSetting setting)
		{
			this.engine = engine;
			allGruop.Clear();
			foreach( AdvLayerSettingData.LayerType type in Enum.GetValues(typeof(AdvLayerSettingData.LayerType) ))
			{
				AdvGraphicGroup group = new AdvGraphicGroup(type, setting, this);
				allGruop.Add(group);
			}
		}

		/// <summary>
		/// 章追加時などリメイク
		/// </summary>
		public void Remake(AdvLayerSetting setting)
		{
			foreach (AdvGraphicGroup group in allGruop)
			{
				group.DestroyAll();
			}
			allGruop.Clear();
			foreach (AdvLayerSettingData.LayerType type in Enum.GetValues(typeof(AdvLayerSettingData.LayerType)))
			{
				AdvGraphicGroup group = new AdvGraphicGroup(type, setting, this);
				allGruop.Add(group);
			}
		}

		/// <summary>
		/// 全てクリア
		/// </summary>
		internal void Clear()
		{
			foreach (AdvGraphicGroup group in allGruop)
			{
				group.Clear();
			}
		}

		/// <summary>
		/// 指定のキーのレイヤーを探す
		/// </summary>
		internal AdvGraphicLayer FindLayer(string layerName)
		{
			foreach (AdvGraphicGroup group in allGruop)
			{
				AdvGraphicLayer layer = group.FindLayer(layerName);
				if (layer != null) return layer;
			}
			return null;
		}

		/// <summary>
		/// 指定の名前のグラフィックオブジェクトを検索
		/// </summary>
		internal AdvGraphicObject FindObject(string name)
		{
			foreach (AdvGraphicGroup group in allGruop)
			{
				AdvGraphicObject obj = group.FindObject(name);
				if (obj != null) return obj;
			}
			return null;
		}

		//全てのグラフィックオブジェクトを取得
		internal List<AdvGraphicObject> AllGraphics()
		{
			List<AdvGraphicObject> allGraphics = new List<AdvGraphicObject>();
			foreach (AdvGraphicGroup group in allGruop)
			{
				group.AddAllGraphics(allGraphics);
			}
			return allGraphics;
		}

		//ロード中かチェック
		internal bool IsLoading
		{
			get
			{
				foreach (AdvGraphicGroup group in allGruop)
				{
					if (group.IsLoading) return true;
				}
				return false;
			}
		}

		/// <summary>
		/// クリックイベントを追加
		/// </summary>
		internal void RemoveClickEvent(string name)
		{
			AdvGraphicObject obj = FindObject(name);
			if (obj != null)
			{
				obj.RemoveClickEvent();
			}
		}

		/// <summary>
		/// 指定の名前のスプライトにクリックイベントを設定
		/// </summary>
		/// <param name="name"></param>
		internal void AddClickEvent(string name, bool isPolygon, StringGridRow row, UnityAction<BaseEventData> action)
		{
			AdvGraphicObject obj = FindObject(name);
			if (obj != null)
			{
				obj.AddClickEvent(isPolygon, row, action);
			}
			else
			{
				Debug.LogError("can't find Graphic object" + name);
			}
		}

		/// <summary>
		/// セーブデータ用のバイナリを取得
		/// </summary>
		/// <returns>セーブデータのバイナリ</returns>
		public byte[] ToSaveDataBuffer()
		{
			using (MemoryStream stream = new MemoryStream())
			{
				//バイナリ化
				using (BinaryWriter writer = new BinaryWriter(stream))
				{
					WriteSaveData(writer);
				}
				return stream.ToArray();
			}
		}

		/// <summary>
		/// セーブデータを読みこみ
		/// </summary>
		/// <param name="buffer">セーブデータのバイナリ</param>
		public void ReadSaveDataBuffer(AdvEngine engine, byte[] buffer)
		{
			using (MemoryStream stream = new MemoryStream(buffer))
			{
				using (BinaryReader reader = new BinaryReader(stream))
				{
					ReadSaveData(reader);
				}
			}
		}

		/*
				/// <summary>
				/// セーブデータを読んだ後、指定のキーのレイヤーに、グラフィックオブジェクトを追加
				/// </summary>
				internal void LoadObject(string layerName, AdvGraphicObject advGraphicObject )
				{
					AdvGraphicLayer layer = FindLayer(layerName);
					layer.LoadObject(advGraphicObject);
				}
		*/		

		const int VERSION = 2;
		const int VERSION_1 = 1;
		const int VERSION_0 = 0;
		//セーブデータ用のバイナリ書き込み
		void WriteSaveData(BinaryWriter writer)
		{
			List<AdvGraphicObject> graphics = AllGraphics();
			writer.Write(VERSION);
			writer.Write(isEventMode);
			writer.Write(graphics.Count);
			foreach (AdvGraphicObject graphic in graphics)
			{
				writer.Write(graphic.name);
				writer.Write(graphic.Layer.SettingData.Name);
				writer.Write(graphic.Graphic.DataType);
				writer.Write(graphic.Graphic.Key);
				writer.Write(graphic.IsDefault);
				graphic.Write(writer);
			}
		}

		//セーブデータ用のバイナリ読み込み
		void ReadSaveData(BinaryReader reader)
		{
			int version = reader.ReadInt32();
			if (version <= VERSION_1)
			{
				ReadOldVersion(reader, version);
			}
			else if(version == VERSION)
			{
				this.isEventMode = reader.ReadBoolean();
				int count = reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					string name = reader.ReadString();
					string layerName = reader.ReadString();
					string graphicDataType = reader.ReadString();
					string graphicKey = reader.ReadString();
					bool isDefault = reader.ReadBoolean();
					GraphicInfoList graphicInfo = AdvGraphicInfoParser.FindGraphicInfo(engine, graphicDataType, graphicKey);
					AdvGraphicLayer layer = FindLayer(layerName);
					AdvGraphicObject graphic = layer.AddObject(name, graphicInfo, isDefault);
					graphic.Read(graphicInfo, reader);
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, version));
			}
		}

		//旧バージョン（LayerManagerだったころ）のデータ読み込み
		void ReadOldVersion(BinaryReader reader, int version)
		{
			if (version >= VERSION_1)
			{
				this.isEventMode = reader.ReadBoolean();
			}

			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				string key = reader.ReadString();
				AdvGraphicLayer layer = FindLayer(key);
				layer.ReadOld(reader);
			}
		}
	}
}