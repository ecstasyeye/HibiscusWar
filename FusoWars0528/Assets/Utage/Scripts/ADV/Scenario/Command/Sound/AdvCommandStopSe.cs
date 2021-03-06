//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;


namespace Utage
{

	/// <summary>
	/// コマンド：Se停止
	/// </summary>
	internal class AdvCommandStopSe : AdvCommand
	{
		public AdvCommandStopSe(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			this.label = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.SoundSetting.Contains(label, SoundType.Se))
			{
				Debug.LogError(ToErrorString(label + " is not contained in file setting"));
			}
			this.fadeTime = ParseCellOptional<float>(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.SoundManager.StopSe(label,fadeTime);
		}

		string label;
		float fadeTime;
	}
}