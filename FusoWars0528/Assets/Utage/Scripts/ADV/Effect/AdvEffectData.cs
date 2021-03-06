//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System;

namespace Utage
{

	/// <summary>
	/// トランジションの管理
	/// </summary>
	[AddComponentMenu("Utage/ADV/EffectManager")]
	public abstract class AdvEffectData
	{
		public enum TargetType
		{
			Default,
			Camera,
			Graphics,
			MessageWindow,
		};

		public enum WaitType
		{
			Wait,
			PageWait,
			Add,
//			Delay,
			NoWait,
		};

		public TargetType Target { get { return targetType; } }
		protected TargetType targetType;

		public string TargetName { get { return targetName ; } }
		protected string targetName ;

		public WaitType Wait { get { return waitType; } }
		protected WaitType waitType;

		protected bool isPlaying = false;
		public bool IsPlaying { get { return isPlaying; } }
		Action<AdvEffectData> onComplete;

		protected AdvEffectManager manager;

		public virtual void Play(AdvEffectManager manager, Action<AdvEffectData> onComplete)
		{
			this.manager = manager;
			this.onComplete = onComplete;
			OnStart(manager);
		}

		public abstract void OnStart(AdvEffectManager manager);

		//コマンドウェイト中か
		public virtual bool IsCommandWaiting
		{
			get
			{
				if (!IsPlaying) return false;
				switch (waitType)
				{
					case WaitType.Wait:
					case WaitType.Add:
						return true;
					default:
						return false;
				}
			}
		}

		//ページウェイト中か
		public virtual bool IsPageWaiting
		{
			get
			{
				if (!IsPlaying) return false;
				switch (waitType)
				{
					case WaitType.PageWait:
					case WaitType.Add:
						return true;
					default:
						return false;
				}
			}
		}

		protected virtual void OnComplete()
		{
			if(onComplete!=null)
			{
				onComplete(this);
			}
			isPlaying  = false;
		}
	}
}
