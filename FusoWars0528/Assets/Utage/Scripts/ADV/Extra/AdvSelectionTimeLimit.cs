//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using Utage;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 時間制限のタイマー
/// </summary>
namespace Utage
{
	[AddComponentMenu("Utage/ADV/Extra/SelectionTimeLimit")]
	public class AdvSelectionTimeLimit : MonoBehaviour
	{
		//無効化フラグ
		[SerializeField]
		bool disable = false;
		public bool Disable
		{
			get { return disable; }
			set { disable = value; }
		}

		/// <summary>ADVエンジン</summary>
		public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>()); } }
		[SerializeField]
		protected AdvEngine engine;

		public float limitTime = 10.0f;
		public int timeLimitIndex = 0;
		float time;

		void Awake()
		{
			Engine.SelectionManager.OnBeginWaitInput.AddListener(OnBeginWaitInput);
			Engine.SelectionManager.OnUpdateWaitInput.AddListener(OnUpdateWaitInput);
		}

		void OnBeginWaitInput(AdvSelectionManager selection)
		{
			time = -Time.deltaTime;
		}

		void OnUpdateWaitInput(AdvSelectionManager selection)
		{
			time += Time.deltaTime;
			if (time >= limitTime)
			{
				selection.Select(timeLimitIndex);
			}
		}
	}
}
