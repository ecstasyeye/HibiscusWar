//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Utage
{

	/// <summary>
	/// フェード切り替え機能つきのスプライト表示
	/// </summary>
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/2DSprite")]
	public class AdvGraphicObject2DSprite : AdvGraphicObject
	{
		/// <summary>
		/// 現在のスプライト
		/// </summary>
		public Sprite2D CurrentSprite { get; protected set; }

		public Sprite2D FadeOutSprite { get; protected set; }

		/// <summary>
		/// 現在のテクスチャ名
		/// </summary>
		public string CurrentTextureName{
			get
			{
				if (CurrentSprite != null && CurrentSprite.File != null)
				{
					return CurrentSprite.File.FileName;
				}
				else
				{
					return "";
				}
			}
		}

		//現在のメインのオブジェクト（フェードなどのエフェクトなどの抜きにしたもの）
		public override GameObject CurrentObject { get { return CurrentSprite ? CurrentSprite.gameObject : null; } }

		//フェードアウト処理
		internal override void OnFadeOut(float fadeTime)
		{
			CurrentSprite.FadeOut(fadeTime, true);
			if (FadeOutSprite)
			{
				FadeOutSprite.FadeOut(0, true);
				FadeOutSprite = null;
			}
		}

		internal override void OnEffectColorsChange(EffectColors colors)
		{
			if (CurrentSprite != null) CurrentSprite.EffectColor = colors.MulColor;
			if (FadeOutSprite != null) FadeOutSprite.EffectColor = colors.MulColor;
		}

		internal override void OnDraw(GraphicInfoList graphic, float fadeTime)
		{
			//テクスチャが同じなら、変化なし
			if (this.CurrentSprite != null && this.CurrentSprite.GraphicInfo == graphic.Main) return;

			//フェードアウト中のスプライトは消す
			if (FadeOutSprite != null)
			{
				FadeOutSprite.FadeOut(0, true);
				FadeOutSprite = null;
			}

			if (CurrentSprite != null)
			{
				//既にスプライトがあるならフェードアウトさせる
				FadeOutSprite = CurrentSprite;
				///表示順は手前にする
				FadeOutSprite.LocalOrderInLayer = FadeOutSprite.LocalOrderInLayer + 1;
				FadeOutSprite.FadeOut(fadeTime, true);

				//テクスチャからスプライト作成
				CurrentSprite = CreateSprite(graphic);
			}
			else
			{
				//新規スプライトがあるならフェードインさせる
				//テクスチャからスプライト作成
				CurrentSprite = CreateSprite(graphic);
				CurrentSprite.FadeIn(fadeTime);
			}
		}

		Sprite2D CreateSprite(GraphicInfoList graphic)
		{
			GraphicInfo texture = graphic.Main;
			Sprite2D sprite = UtageToolKit.AddChildGameObjectComponent<Sprite2D>(this.transform, FilePathUtil.GetFileNameWithoutExtension(texture.File.FileName));
			sprite.SetTextureFile(texture, PixelsToUnits);
			sprite.LocalOrderInLayer = Layer.SettingData.Order;
			return sprite;
		}

		//古いセーブデータを読み込み
		internal void ReadOld(BinaryReader reader)
		{
			UtageToolKit.ReadLocalTransform(this.transform, reader);
			EffectColors.SetColor(Utage.EffectColors.Index.TweenColor, UtageToolKit.ReadColor(reader));

			//Tweenがある場合は、Tween情報を読み込む
			int tweenCount = reader.ReadInt32();
			for (int i = 0; i < tweenCount; ++i)
			{
				AdvTweenPlayer tween = this.gameObject.AddComponent<AdvTweenPlayer>();
				tween.Read(reader, PixelsToUnits);
			}

			GraphicInfoList  graphic = AdvGraphicInfoParser.FindGraphicInfoFromTexturePath(Engine, reader.ReadString());
			OnDraw(graphic, 0);
			Graphic = graphic;
			IsLoading = false;
		}
	}
}
