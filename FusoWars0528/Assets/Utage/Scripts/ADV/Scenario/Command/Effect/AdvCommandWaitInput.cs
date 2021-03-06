//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：
	/// </summary>
	internal class AdvCommandWaitInput : AdvCommand
	{

		public AdvCommandWaitInput(StringGridRow row)
			: base(row)
		{
			this.time = this.ParseCellOptional<float>(AdvColumnName.Arg6,-1);
		}

		public override void DoCommand(AdvEngine engine)
		{
			waitEndTime = Time.time + (engine.Page.CheckSkip() ? time / engine.Config.SkipSpped : time);
		}

		public override bool Wait(AdvEngine engine)
		{
			if (engine.Page.CheckSkip ())
			{
				return false;
			}
			if (engine.UiManager.IsInputTrig )
			{
				//ボイスを止める
				if (engine.Config.VoiceStopType == VoiceStopType.OnClick)
				{
					engine.SoundManager.StopVoice();
				} 
				engine.UiManager.ClearPointerDown();
				return false;
			}
			if (this.time > 0)
			{
				return (Time.time < waitEndTime);
			}

			return true;
		}

		float time;
		float waitEndTime;
	}
}