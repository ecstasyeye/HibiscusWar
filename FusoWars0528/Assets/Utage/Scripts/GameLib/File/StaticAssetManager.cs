//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	/// <summary>
	/// 動的にロードしないで、常に保持しつづけるアセットの管理
	/// 3Dモデルや、BGM（DLするとストリーム再生できない）など
	/// アセットバンドル化したくないオブジェクトを中心に
	/// </summary>
	[AddComponentMenu("Utage/Lib/File/StaticAssetManager")]
	public class StaticAssetManager : MonoBehaviour
	{
		[SerializeField]
		List<StaticAsset> assets = new List<StaticAsset>();
		List<StaticAsset> Assets { get { return assets; } }

		public AssetFile FindAssetFile(string filePath, AssetFileManagerSettings settings, StringGridRow rowData)
		{
			if (Assets == null) return null;
			string assetName = FilePathUtil.GetFileNameWithoutExtension(filePath);
			StaticAsset asset = Assets.Find((x) => (x.Asset.name == assetName));
			if (asset == null) return null;

			return new StaticAssetFile(asset, filePath, settings.FindSettingFromPath(filePath), rowData);
		}

		public bool Contains(Object asset)
		{
			foreach( StaticAsset item in Assets )
			{
				if( item.Asset == asset ) return true;
			}
			return false;
		}
	}

	//動的にロードしないアセットの情報
	[System.Serializable]
	public class StaticAsset
	{
		[SerializeField]
		Object asset;
		public Object Asset
		{
			get { return asset; }
		}
	}

	//動的にロードしないアセットをロードファイルのように扱うためのクラス
	public class StaticAssetFile : AssetFileBase
	{
		public StaticAssetFile(StaticAsset asset, string filePath, AssetFileSetting setting, StringGridRow rowData)
			: base(filePath, setting, rowData)
		{
			this.Asset = asset;
			TextAsset textAsset = Asset.Asset as TextAsset;
			if (null != textAsset)
			{
				Text = textAsset.text;
				Bytes = textAsset.bytes;
				bool isTsv = (LoadFlags & AssetFileLoadFlags.Tsv) != AssetFileLoadFlags.None;
				Csv = new StringGrid(FileName, isTsv ? CsvType.Tsv : CsvType.Csv, textAsset.text);
			}
			this.Sprite = Asset.Asset as Sprite;
			this.Texture = Asset.Asset as Texture2D;
			this.Sound = Asset.Asset as AudioClip;
			this.UnityObject = Asset.Asset;
			this.IsLoadEndOriginal = true;
		}

		public override IEnumerator CoLoadAsync(float timeOutDownload)
		{
			yield break;
		}
		public override void Unload() { }

		public StaticAsset Asset { get; protected set; }
	}
}
