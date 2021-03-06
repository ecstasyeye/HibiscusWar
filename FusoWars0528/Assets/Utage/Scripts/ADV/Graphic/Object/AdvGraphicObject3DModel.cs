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
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/3DModel")]
	public class AdvGraphicObject3DModel : AdvGraphicObject
	{
		//現在のメインのオブジェクト（フェードなどのエフェクトなどの抜きにしたもの）
		public override GameObject CurrentObject { get { return currentObject; } }
		protected GameObject currentObject;

		protected GameObject childRoot;

		void Awake()
		{
			childRoot = UtageToolKit.AddChild(this.transform, new GameObject("root"));
			childRoot.transform.localEulerAngles = Vector3.up * 180;
		}

		//描画
		internal override void OnDraw(GraphicInfoList graphic, float fadeTime)
		{
			if (Graphic == graphic)
			{
			}
			else
			{
				GraphicInfo main = graphic.Main;
				currentObject = UtageToolKit.AddChildUnityObject(childRoot.transform, main.File.UnityObject);
				childRoot.transform.localScale = new Vector3(main.Scale.x, main.Scale.y, 1.0f);
//				childRoot.transform.localPosition = PivotUtil.GetPivotOffset(currentObject, graphic.Pivot);
			}
		}

		//描画
		internal override void OnDrawArgCustom(AdvGraphicOperaitonArg arg)
		{
			string stateName = arg.Arg2;
			if(!string.IsNullOrEmpty(stateName))
			{
				Animator animator = currentObject.GetComponentInChildren<Animator>();
				if (animator)
				{
					animator.CrossFade(stateName, arg.FadeTime);
				}
				else
				{
					Animation animation = currentObject.GetComponentInChildren<Animation>();
					animation.CrossFade(stateName, arg.FadeTime);
				}
			}
			if (!arg.IsPostionArgs)
			{

			}
		}

		//フェードアウト処理(3Dなのでフェードアウトは出来ない)
		internal override void OnFadeOut(float fadeTime)
		{
			//フェード時間を適用できないので、即座に消す
			GameObject.Destroy(this.gameObject);
		}
	}
}
