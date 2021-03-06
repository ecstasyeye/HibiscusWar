//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Utage
{

	/// <summary>
	/// コマンド：バイブレーションを作動
	/// </summary>
	internal class AdvCommandVibrate : AdvCommand
	{
		public AdvCommandVibrate(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
#if UNITY_IPHONE || UNITY_ANDROID
			Handheld.Vibrate();
#endif
		}
	}
}
