//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：環境音再生
	/// </summary>
	internal class AdvCommandAmbience : AdvCommand
	{
		public AdvCommandAmbience( StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			string label = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.SoundSetting.Contains(label, SoundType.Ambience))
			{
				Debug.LogError(ToErrorString(label + " is not contained in file setting"));
			}

			this.file = AddLoadFile( dataManager.SoundSetting.LabelToFilePath(label, SoundType.Ambience),  dataManager.SoundSetting.FindRowData(label) );
			this.isLoop = ParseCellOptional<bool>(AdvColumnName.Arg2, false);
			this.fadeOutTime = ParseCellOptional<float>(AdvColumnName.Arg5,0.2f);
			this.fadeInTime = ParseCellOptional<float>(AdvColumnName.Arg6,0);
		}
		public override void DoCommand(AdvEngine engine)
		{
			engine.SoundManager.PlayAmbience(file, isLoop,fadeInTime, fadeOutTime);
		}
		AssetFile file;
		bool isLoop;
		float fadeInTime;
		float fadeOutTime;
	}
}