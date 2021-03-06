//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Utage
{
	//喋っていないキャラクターをグレーアウトする処理
	//AdvEngineのOnPageTextChangeから呼び出す、このコンポーネントの同名メソッドを登録すると使えるようになる
	[AddComponentMenu("Utage/ADV/Extra/CharacterGrayOutContoller")]
	public class AdvCharacterGrayOutContoller : MonoBehaviour
	{
		/// <summary>ADVエンジン</summary>
		public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>()); } }
		[SerializeField]
		protected AdvEngine engine;

		//ライティング（グレーアウトしない）するフラグ
		[System.Flags]
		public enum LightingMask
		{
			Talking = 0x1,					//しゃべっているキャラは
			NewCharacerInPage = 0x1 << 1,	//ページ内の新しいキャラクター
			NoChanageIfTextOnly = 0x1 << 2,	//テキストのみ表示のときは、変化しない
		}

		[SerializeField, EnumFlags]
		LightingMask mask = LightingMask.Talking;
		public LightingMask Mask
		{
			get { return mask; }
			set { mask = value; }
		}

		//グレーアウトいないほうの色。白以外の色も任意に設定可能
		[SerializeField]
		Color mainColor = Color.white;
		public Color MainColor
		{
			get { return mainColor; }
			set { mainColor = value; }
		}

		//グレーアウトするほうの色　グレー以外の色も任意に設定可能
		[SerializeField]
		Color subColor = Color.gray;
		public Color SubColor
		{
			get { return subColor; }
			set { subColor = value; }
		}

		//フェード時間
		[SerializeField]
		float fadeTime = 0.2f;
		public float FadeTime
		{
			get { return fadeTime; }
			set { fadeTime = value; }
		}

		//テキストに変更があった場合
		void Awake()
		{
			if (Engine != null)
			{
				Engine.Page.OnBeginPage.AddListener(OnBeginPage);
				Engine.Page.OnChangeText.AddListener(OnChangeText);
			}
		}

		bool isChanged = false;
		List<AdvGraphicObject> pageBeinGraphics;

		//ページの冒頭
		void OnBeginPage(AdvPage page)
		{
			this.pageBeinGraphics = page.Engine.GraphicManager.CharacterManager.AllGraphics();
			if (this.mask == 0)
			{
				//表示なしなのでリセット
				if (isChanged)
				{
					foreach (AdvGraphicObject obj in pageBeinGraphics)
					{
						ChangeColor(obj, MainColor);
					}
					isChanged = false;
				}
			}
		}

		//テキストに変更があった場合
		void OnChangeText(AdvPage page)
		{
			if (this.mask == 0) return;
			isChanged = true;
			AdvEngine engine = page.Engine;

			//テキストのみ表示で、前のキャラをそのまま表示
			if (string.IsNullOrEmpty(page.CharacterLabel) && (Mask & LightingMask.NoChanageIfTextOnly) == LightingMask.NoChanageIfTextOnly)
			{
				return;
			}

			List<AdvGraphicObject> graphics = engine.GraphicManager.CharacterManager.AllGraphics();
			foreach (AdvGraphicObject obj in graphics)
			{
				ChangeColor(obj, IsLightingCharacter(page, obj) ? MainColor : SubColor);
			}
		}
		
		//強調表示（グレーアウト無視）するか
		bool IsLightingCharacter(AdvPage page, AdvGraphicObject obj)
		{
			//しゃべっているキャラ
			if( (Mask & LightingMask.Talking) == LightingMask.Talking)
			{
				if (obj.name == page.CharacterLabel) return true;
			}

			//ページ内の新規キャラ
			if ((Mask & LightingMask.NewCharacerInPage) == LightingMask.NewCharacerInPage)
			{
				if (pageBeinGraphics.Find(x => (x !=null ) && (x.name == obj.name) ) == null) return true;
			}
			return false;
		}

		//カラーを取得
		void ChangeColor(AdvGraphicObject obj, Color color)
		{
			if (FadeTime > 0)
			{
				Color from = obj.EffectColors.GetColor(EffectColors.Index.Color1);
				StartCoroutine(FadeColor(obj, from, color));
			}
			else
			{
				obj.EffectColors.SetColor(EffectColors.Index.Color1, color);
			}
		}

		IEnumerator FadeColor(AdvGraphicObject obj, Color from, Color to)
		{
			float elapsed = 0f;
			while(true)
			{
				yield return new WaitForEndOfFrame();
				elapsed += Time.deltaTime;
				if (elapsed >= fadeTime)
				{
					elapsed = fadeTime;
				}
				obj.EffectColors.SetColor(EffectColors.Index.Color1, Color.Lerp(from, to, elapsed / FadeTime));
				if (elapsed >= fadeTime) yield break;
			}
		}		
	}
}

