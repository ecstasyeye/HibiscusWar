//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
#if LegacyUtageUi

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
	[AddComponentMenu("Utage/ADV/Internal/FadeSprites")]
	public class AdvFadeSprites : Node2D
	{
		float pixelsToUnits;

		/// <summary>
		/// 現在のスプライト
		/// </summary>
		public Sprite2D CurrentSprite { get { return currentSprite; } }
		Sprite2D currentSprite;

		Sprite2D fadeOutSprite;

		/// <summary>
		/// 現在のグラフィック情報
		/// </summary>
		public GraphicInfo CurrentGraphicInfo { get { return CurrentSprite == null  ? null : CurrentSprite.GraphicInfo; } }

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


		/// <summary>
		/// 描画する
		/// </summary>
		public void OnDraw(AdvGraphicObject advGraphicObject, AdvGraphicOperaitonArg arg) { }

		/// <summary>
		/// デフォルト描画
		/// </summary>
		public void OnDrawDefault(AdvGraphicObject advGraphicObject, AdvGraphicOperaitonArg arg) { }

		/// <summary>
		/// ロードする
		/// </summary>
		public void OnLoadObject(AdvGraphicObject advGraphicObject) { }

		/// <summary>
		/// フェードアウトして消える
		/// </summary>
		/// <param name="fadeTime"></param>
		public void FadeOut(float fadeTime) { }

		/// <summary>
		/// 上書きでフェードアウトして消える
		/// </summary>
		/// <param name="fadeTime"></param>
		public void OverrideFadeOut(float fadeTime) { }

		/// <summary>
		/// クリア
		/// </summary>
		public void Clear() { }

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="pixelsToUnits"></param>
		public void Init(float pixelsToUnits)
		{
			this.pixelsToUnits = pixelsToUnits;
		}

		/// <summary>
		/// テクスチャからスプライト作成
		/// 前に別のテクスチャが表示されていれば、それをフェードアウトさせる
		/// </summary>
		/// <param name="graphic">テクスチャ</param>
		/// <param name="fadeTime">フェード時間</param>
		public void SetTexture(GraphicInfo graphic, float fadeTime)
		{
			//テクスチャが同じなら、変化なし
			if (CurrentGraphicInfo == graphic) return;

			//フェードアウト中のスプライトは消す
			if (fadeOutSprite != null)	//destoryされたコンポーネントはnull判定になるはず･･･
			{
				fadeOutSprite.FadeOut(0, true);
				fadeOutSprite = null;
			}
			
			if (currentSprite != null)
			{
				//既にスプライトがあるならフェードアウトさせる
				fadeOutSprite = currentSprite;
				///表示順は手前にする
				fadeOutSprite.LocalOrderInLayer = fadeOutSprite.LocalOrderInLayer + 1;
				fadeOutSprite.FadeOut(fadeTime, true);

				//テクスチャからスプライト作成
				currentSprite = CreateSprite(graphic);
			}
			else
			{
				//新規スプライトがあるならフェードインさせる
				//テクスチャからスプライト作成
				currentSprite = CreateSprite(graphic);
				currentSprite.FadeIn(fadeTime);
			}
		}

		/// <summary>
		/// クリックイベントを設定
		/// </summary>
		public void AddClickEvent(bool isPolygon, UnityAction<BaseEventData> action)
		{
			StartCoroutine(CoAddClickEvent(isPolygon, action));
		}
		
		IEnumerator CoAddClickEvent(bool isPolygon, UnityAction<BaseEventData> action)
		{
			
			while( CurrentSprite.IsLoading ) yield return 0;
//			yield return new WaitForEndOfFrame();

			GameObject go = CurrentSprite.gameObject;

			//コライダーの追加
			if (isPolygon)
			{
				if (!go.GetComponent<PolygonCollider2D>())
				{
					go.AddComponent<PolygonCollider2D>();
				}
			}
			else
			{
				if (!go.GetComponent<Collider2D>())
				{
					go.AddComponent<Collider2D>();
				}
			}


			//イベントトリガーの追加
			EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
			if (!eventTrigger)
			{
				eventTrigger = go.AddComponent<EventTrigger>();
			}

			UtageToolKit.AddEventTriggerEntry(eventTrigger, action,  EventTriggerType.PointerClick );
		}

		/// <summary>
		/// クリックイベントを削除
		/// </summary>
		public void RemoveClickEvent()
		{
			GameObject go = CurrentSprite.gameObject;
			PolygonCollider2D polygonCollider2D = go.GetComponent<PolygonCollider2D>();
			if (polygonCollider2D)
			{
				Destroy(polygonCollider2D);
			}
			Collider2D collider2D = go.GetComponent<Collider2D>();
			if (collider2D)
			{
				Destroy(collider2D);
			}
			EventTrigger eventTrigger = go.GetComponent<EventTrigger>();
			if (eventTrigger)
			{
				Destroy(eventTrigger);
			}
		}
/*
		/// <summary>
		/// セーブデータ用のバイナリ書き込み
		/// </summary>
		/// <param name="writer">バイナリライター</param>
		public void WriteOld(BinaryWriter writer)
		{
			UtageToolKit.WriteLocalTransform(this.transform, writer);
			UtageToolKit.WriteColor(this.LocalColor, writer);

			//無限ループのTweenがある場合は、Tween情報を書き込む
			iTweenPlayer[] tweenArray = this.gameObject.GetComponents<iTweenPlayer>() as iTweenPlayer[];
			int tweenCount = 0;
			foreach (var tween in tweenArray)
			{
				if (tween.IsEndlessLoop) ++tweenCount;
			}
			writer.Write(tweenCount);
			foreach (var tween in tweenArray)
			{
				if (tween.IsEndlessLoop) tween.Write(writer);
			}

			writer.Write(CurrentTextureName);
		}
*/
		/// <summary>
		/// 昔のセーブデータ用のバイナリ読みこみ
		/// </summary>
		/// <param name="reader">バイナリリーダー</param>
		public void ReadOld(BinaryReader reader)
		{
			UtageToolKit.ReadLocalTransform(this.transform, reader);
//			EffectColors.SetColor( Utage.EffectColors.Index.TweenColor, UtageToolKit.ReadColor(reader) );

			//Tweenがある場合は、Tween情報を読み込む
			int tweenCount = reader.ReadInt32();
			for (int i = 0; i < tweenCount; ++i)
			{
				AdvTweenPlayer tween = this.gameObject.AddComponent<AdvTweenPlayer>() as AdvTweenPlayer;
				tween.Read(reader, pixelsToUnits);
			}
		}

		const int Version = 0;

		/// <summary>
		/// セーブデータ用のバイナリ書き込み
		/// </summary>
		/// <param name="writer">バイナリライター</param>
		public void Write(BinaryWriter writer)
		{
			writer.Write(Version);

			UtageToolKit.WriteLocalTransform(this.transform, writer);
			UtageToolKit.WriteColor(this.LocalColor, writer);
//			this.EffectColors.Write(writer);

			//無限ループのTweenがある場合は、Tween情報を書き込む
			AdvTweenPlayer[] tweenArray = this.gameObject.GetComponents<AdvTweenPlayer>();
			int tweenCount = 0;
			foreach (var tween in tweenArray)
			{
				if (tween.IsEndlessLoop) ++tweenCount;
			}
			writer.Write(tweenCount);
			foreach (var tween in tweenArray)
			{
				if (tween.IsEndlessLoop) tween.Write(writer);
			}
		}

		/// <summary>
		/// セーブデータ用のバイナリ読みこみ
		/// </summary>
		/// <param name="reader">バイナリリーダー</param>
		public void Read(BinaryReader reader)
		{
			int version = reader.ReadInt32();
			if (version >= Version)
			{
				UtageToolKit.ReadLocalTransform(this.transform, reader);
				this.LocalColor = UtageToolKit.ReadColor(reader);
//				this.EffectColors.Read(reader);

				//Tweenがある場合は、Tween情報を読み込む
				int tweenCount = reader.ReadInt32();
				for (int i = 0; i < tweenCount; ++i)
				{
					AdvTweenPlayer tween = this.gameObject.AddComponent<AdvTweenPlayer>();
					tween.Read(reader, pixelsToUnits);
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, version));
			}
		}
		
		Sprite2D CreateSprite(GraphicInfo graphic)
		{
			Sprite2D sprite = UtageToolKit.AddChildGameObjectComponent<Sprite2D>(this.transform, System.IO.Path.GetFileNameWithoutExtension(graphic.File.FileName));
			sprite.SetTextureFile(graphic, pixelsToUnits);
			return sprite;
		}
	}
}
#endif
