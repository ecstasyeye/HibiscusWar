//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：フェードアウト処理
	/// </summary>
	internal class AdvCommandImageEffectStart : AdvCommand
	{

		public AdvCommandImageEffectStart(StringGridRow row)
			: base(row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
/*			if (engine.UiTransitionManager != null)
			{
				engine.UiTransitionManager.StartImageEffect();
			}
*/		}
/*
		public override bool Wait(AdvEngine engine)
		{
			if (engine.UiTransitionManager != null)
			{
				return engine.UiTransitionManager.IsPlaying;
			}
			return false;
		}
*/	}
}