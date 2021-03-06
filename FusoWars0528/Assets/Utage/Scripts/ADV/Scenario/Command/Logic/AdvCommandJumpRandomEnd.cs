//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：ランダムジャンプ終了
	/// </summary>
	internal class AdvCommandJumpRandomEnd : AdvCommand
	{
		public AdvCommandJumpRandomEnd(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			AdvCommandJumpRandom command = engine.ScenarioPlayer.JumpManager.GetRandomJumpCommand() as AdvCommandJumpRandom;
			if(command!=null )
			{
				command.DoRandomEnd(engine);
			}
		}
	}
}