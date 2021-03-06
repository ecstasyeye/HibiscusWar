//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：BGM再生
	/// </summary>
	internal class AdvCommandBgm : AdvCommand
	{
		public AdvCommandBgm(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			string label = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.SoundSetting.Contains(label, SoundType.Bgm))
			{
				Debug.LogError(ToErrorString(label + " is not contained in file setting"));
			}

			this.file = AddLoadFile(dataManager.SoundSetting.LabelToFilePath(label, SoundType.Bgm), dataManager.SoundSetting.FindRowData(label));

			this.fadeOutTime = ParseCellOptional<float>(AdvColumnName.Arg5,0.2f);
			this.fadeInTime = ParseCellOptional<float>(AdvColumnName.Arg6,0);
		}
		public override void DoCommand(AdvEngine engine)
		{
			engine.SoundManager.PlayBgm(file, fadeInTime, fadeOutTime);
		}
		AssetFile file;
		float fadeInTime;
		float fadeOutTime;
	}
}