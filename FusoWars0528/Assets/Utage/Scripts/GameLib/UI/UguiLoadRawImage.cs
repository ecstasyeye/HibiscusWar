//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// 動的にロード可能なRawImage
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/UI/LoadRawImage")]
	[RequireComponent(typeof(RawImage))]
	public class UguiLoadRawImage : MonoBehaviour
	{
		[SerializeField]
		string path;

		public string Path
		{
			get { return path; }
			set
			{
				if (path != value)
				{
					LoadTextureFile(value);
				}
			}
		}


		/// <summary>
		/// スプライト
		/// </summary>
		public RawImage RawImage { get { return rawImage ?? (rawImage = GetComponent<RawImage>()); }}
		RawImage rawImage;

		/// <summary>
		/// テクスチャファイル
		/// </summary>
		public AssetFile TextureFile { get { return this.textureFile; } }
		AssetFile textureFile;

		//グラフィック情報
		public GraphicInfo GraphicInfo { get { return this.graphicInfo; } }
		GraphicInfo graphicInfo;

		//テクスチャの
		public enum SizeSetting
		{
			RectSize,		//RectTransformの矩形のサイズ
			TextureSize,	//テクスチャサイズに合わせる
			GraphicSize,	//グラフィックのサイズに合わせる
		};
		public SizeSetting TextureSizeSetting { get { return sizeSetting; } set { sizeSetting = value; } }
		[SerializeField]
		SizeSetting sizeSetting = SizeSetting.RectSize;

		public UnityEvent OnLoadEnd;

		/// <summary>
		/// テクスチャファイルを設定
		/// </summary>
		/// <param name="path">ファイルパス</param>
		public void LoadTextureFile(string path)
		{
			AssetFile file = AssetFileManager.Load(path, this);
			SetTextureFile(file);
			file.Unuse(this);
		}

		/// <summary>
		/// テクスチャファイルを設定
		/// </summary>
		/// <param name="graphic">グラフィック情報</param>
		public void LoadTextureFile(GraphicInfo graphic)
		{
			this.graphicInfo = graphic;
			AssetFileManager.Load(graphic.File, this);
			SetTextureFile(graphic.File);
			graphic.File.Unuse(this);
		}

		/// <summary>
		/// テクスチャファイルを設定
		/// </summary>
		/// <param name="file">テクスチャファイル</param>
		/// <param name="pixelsToUnits">スプライトを作成する際の、座標1.0単位辺りのピクセル数</param>
		public void SetTextureFile(AssetFile file)
		{
			//直前のファイルがあればそれを削除
			ClearTextureFile();
			this.textureFile = file;
			this.path = file.FileName;

			textureFile.AddReferenceComponet(this.gameObject);
			StartCoroutine(CoWaitTextureFileLoading());
		}

		IEnumerator CoWaitTextureFileLoading()
		{
			while (!textureFile.IsLoadEnd) yield return 0;
			SetTexture(textureFile.Texture);
			OnLoadEnd.Invoke();
		}

		void SetTexture(Texture texture)
		{
			RawImage.texture = textureFile.Texture;
			switch( TextureSizeSetting )
			{
				case SizeSetting.TextureSize:
					SetRawImageSize(texture.width, texture.height);
					break;
				case SizeSetting.GraphicSize:
					if (GraphicInfo == null)
					{
						Debug.LogError("graphic is null");
					}
					else
					{
						float w = texture.width * GraphicInfo.Scale.x;
						float h = texture.height * GraphicInfo.Scale.y;
						SetRawImageSize(w, h);
					}
					break;
				case SizeSetting.RectSize:
				default:
					break;
			}
		}

		void SetRawImageSize( float w, float h )
		{
			(RawImage.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
			(RawImage.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
		}

		/// <summary>
		/// テクスチャファイルをクリア
		/// </summary>
		public void ClearTextureFile()
		{
			RawImage.texture = null;
			AssetFileReference reference = this.GetComponent<AssetFileReference>();
			if (reference != null)
			{
				Destroy(reference);
			}
			this.textureFile = null;
		}
	}
}
