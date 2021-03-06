//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;
using System.Collections;

namespace Utage
{

/// <summary>
/// コマンド：Tweenアニメーションをする
/// </summary>
	public class AdvCommandTween : AdvCommand
	{
		AdvEffectDataTween effectData;
		public AdvCommandTween(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			this.effectData = new AdvEffectDataTween(this);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.EffectManager.Play(effectData);
		}

		//コマンド終了待ち
		public override bool Wait(AdvEngine engine)
		{
			return engine.EffectManager.IsCommandWaiting(effectData);
		}
	}
}
