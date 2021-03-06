//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// レイヤー設定のデータ
	/// </summary>	
	public class AdvLayerSettingData : AdvSettingDataDictinoayItemBase
	{
		/// <summary>
		/// レイヤー名
		/// </summary>
		public string Name { get { return this.Key; } }

		/// <summary>
		/// レイヤーのタイプ
		/// </summary>
		public enum LayerType
		{
			/// <summary>背景</summary>
			Bg,
			/// <summary>キャラクター</summary>
			Character,
			/// <summary>その他スプライト</summary>
			Sprite,
			/// <summary>タイプ数</summary>
			Max,
		};
		/// <summary>
		/// レイヤーのタイプ
		/// </summary>
		public LayerType Type { get { return this.type; } }
		LayerType type;

		/// <summary>
		/// 中心座標
		/// </summary>
		public Vector2 Center { get { return this.center; } }
		Vector2 center;

		/// <summary>
		/// 描画順
		/// </summary>
		public int Order { get { return this.order; } }
		int order;
		//	public int SpriteSortingOrderOffset {get {return Depth*1000;}}

		/// <summary>
		/// レイヤーマスク（Unityのレイヤー名）
		/// </summary>
		public string LayerMask { get { return this.layerMask; } }
		string layerMask;

		/// <summary>
		/// z値(未定義の場合は、float.MinValue)
		/// </summary>
		public float GetZ( float sortOderToZUnits)
		{
			return Mathf.Approximately(z, float.MinValue) ?  -1.0f*Order / sortOderToZUnits : this.z;
		}
		float z;

		/// <summary>
		/// デフォルトデータ
		/// </summary>
		public bool IsDefault { get { return this.isDefault; } set { this.isDefault = value; } }
		bool isDefault;

		public StringGridRow RowData { get; protected set; }

		/// <summary>
		/// StringGridの一行からデータ初期化
		/// </summary>
		/// <param name="row">初期化するためのデータ</param>
		/// <returns>成否</returns>
		public override bool InitFromStringGridRow(StringGridRow row, AdvBootSetting bootSetting)
		{
			RowData = row;
			string key = AdvParser.ParseCell<string>(row, AdvColumnName.LayerName);
			if (string.IsNullOrEmpty(key))
			{
				return false;
			}
			else
			{
				InitKey(key);
				this.type = AdvParser.ParseCell<LayerType>(row, AdvColumnName.Type);
				this.center.Set(AdvParser.ParseCellOptional<float>(row, AdvColumnName.X,0), AdvParser.ParseCellOptional<float>(row, AdvColumnName.Y,0));
				this.order = AdvParser.ParseCell<int>(row, AdvColumnName.Order);
				this.layerMask = AdvParser.ParseCellOptional<string>(row, AdvColumnName.LayerMask,"");
				this.z = AdvParser.ParseCellOptional<float>(row, AdvColumnName.Z, float.MinValue);
				return true;
			}
		}

		/// <summary>
		/// デフォルトレイヤー用の初期化
		/// </summary>
		/// <param name="name">名前</param>
		/// <param name="type">タイプ</param>
		/// <param name="order">描画順</param>
		public void InitDefault( string name, LayerType type, int order )
		{
			InitKey(name);
			this.type = type;
			this.center = Vector2.zero;
			this.order = order;
			this.layerMask = "";
			this.z = -0.001f * order;
		}
	}

	/// <summary>
	/// レイヤー設定
	/// </summary>
	public class AdvLayerSetting : AdvSettingDataDictinoayBase<AdvLayerSettingData>
	{
		public override void ParseGrid(StringGrid grid, AdvBootSetting bootSetting)
		{
			base.ParseGrid(grid, bootSetting);
			InitDefault(AdvLayerSettingData.LayerType.Bg,0);
			InitDefault(AdvLayerSettingData.LayerType.Character, 100);
			InitDefault(AdvLayerSettingData.LayerType.Sprite, 200);
		}

		void InitDefault( AdvLayerSettingData.LayerType type, int defaultOrder )
		{
			AdvLayerSettingData defaultLayer = List.Find((item) => item.Type == type);
			if (defaultLayer == null)
			{
				defaultLayer = new AdvLayerSettingData();
				defaultLayer.InitDefault( type.ToString()  +" Default", type, defaultOrder);
				AddData( defaultLayer);
			}
			defaultLayer.IsDefault = true;
		}

		public bool Contains(string layerName, AdvLayerSettingData.LayerType type )
		{
			AdvLayerSettingData data;
			if( Dictionary.TryGetValue(layerName, out data ) )
			{
				return data.Type == type;
			}
			return false;
		}
	}
}