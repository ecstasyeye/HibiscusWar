//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：選択肢追加終了
	/// </summary>
	internal class AdvCommandSelectionEnd : AdvCommand
	{
		public AdvCommandSelectionEnd(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.Config.StopSkipInSelection();
			engine.SelectionManager.Show();
		}
	}
}
