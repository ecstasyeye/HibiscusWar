//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.Events;
using Utage;
using System.Collections;
using System;


namespace Utage
{
	public class FadeInOutEvent : UnityEvent<float>{}

	//フェードイン・アウトを行う
	[AddComponentMenu("Utage/Lib/Effect/FadeInOut")]
	public class FadeInOut : MonoBehaviour
	{
		public float Alpha
		{
			get { return alpha; }
			set { alpha = value; OnValueChanged.Invoke(Alpha); }
		}
		[SerializeField]
		float alpha = 1;

		public FadeInOutEvent OnValueChanged = new FadeInOutEvent();

		LinearValue fadeValue = new LinearValue();

		/// <summary>
		/// フェードイン開始
		/// </summary>
		/// <param name="fadeTime">フェードする時間</param>
		public void FadeIn(float fadeTime)
		{
			fadeValue.Init(fadeTime, 0, 1);
			StopCoroutine("CoFade");
			StartCoroutine("CoFade", false);
		}

		/// <summary>
		/// フェードアウト開始
		/// </summary>
		/// <param name="fadeTime">フェードする時間</param>
		/// <param name="autiomaticDestoroy">フェード終了後、自動的に自分自身のGameObjectをDestoryする</param>
		public void FadeOut(float fadeTime)
		{
			FadeOut(fadeTime, false);
		}

		/// <summary>
		/// フェードアウト開始
		/// </summary>
		/// <param name="fadeTime">フェードする時間</param>
		/// <param name="autiomaticDestoroy">フェード終了後、自動的に自分自身のGameObjectをDestoryする</param>
		public void FadeOut(float fadeTime, bool autiomaticDestoroy)
		{
			fadeValue.Init(fadeTime, alpha, 0);
			StopCoroutine("CoFade");
			StartCoroutine("CoFade", autiomaticDestoroy);
		}

		IEnumerator CoFade(bool autiomaticDestoroy)
		{
			while (!fadeValue.IsEnd())
			{
				fadeValue.IncTime();
				Alpha = fadeValue.GetValue();
				yield return 0;
			}
			if (autiomaticDestoroy)
			{
				GameObject.Destroy(this.gameObject);
			}
		}
	};	

}