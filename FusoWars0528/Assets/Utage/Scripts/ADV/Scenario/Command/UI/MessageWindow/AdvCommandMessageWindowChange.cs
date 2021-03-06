//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;


namespace Utage
{

	/// <summary>
	/// コマンド：MessageWindow操作　ChangeCurrent
	/// </summary>
	internal class AdvCommandMessageWindowChangeCurrent : AdvCommand
	{
		public AdvCommandMessageWindowChangeCurrent(StringGridRow row)
			: base(row)
		{
			this.name = ParseCell<string>(AdvColumnName.Arg1);
		}

		//ページ用のデータを作成
		public override void MakePageData(AdvScenarioPageData pageData)
		{
			pageData.InitMessageWindowName(this, name);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.MessageWindowManager.ChangeCurrentWindow(name);
		}

		string name;
	}
}
