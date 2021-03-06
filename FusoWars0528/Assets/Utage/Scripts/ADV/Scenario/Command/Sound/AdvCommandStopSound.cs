//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------


namespace Utage
{

	/// <summary>
	/// コマンド：サウンド停止
	/// </summary>
	internal class AdvCommandStopSound : AdvCommand
	{
		public AdvCommandStopSound(StringGridRow row)
			:base(row)
		{
			this.fadeTime = ParseCellOptional<float>(AdvColumnName.Arg6, fadeTime);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.SoundManager.StopBgm(fadeTime);
			engine.SoundManager.StopAmbience(fadeTime);
		}

		float fadeTime = 0.15f;
	}
}