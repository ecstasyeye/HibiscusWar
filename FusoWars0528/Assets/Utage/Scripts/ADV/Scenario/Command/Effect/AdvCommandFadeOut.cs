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
	internal class AdvCommandFadeOut : AdvCommand
	{

		public AdvCommandFadeOut(StringGridRow row)
			: base(row)
		{
			this.color = ParseCellOptional<Color>(AdvColumnName.Arg1, Color.white);
			this.time = ParseCellOptional<float>(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			float fadetime = engine.Page.CheckSkip() ? time / engine.Config.SkipSpped : time;
/*			if (engine.UiTransitionManager != null)
			{
				engine.UiTransitionManager.FadeOut(fadetime, color);
			}
			else*/
			{
				engine.TransitionManager.FadeOut(fadetime, color);
			}
		}

		public override bool Wait(AdvEngine engine)
		{
/*			if (engine.UiTransitionManager != null)
			{
				return engine.UiTransitionManager.IsPlaying;
			}
			else*/
			{
				return engine.TransitionManager.IsWait;
			}
		}

		float time;
		Color color;
	}
}