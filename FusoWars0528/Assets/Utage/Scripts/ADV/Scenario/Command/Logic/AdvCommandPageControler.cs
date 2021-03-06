//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：ページ制御
	/// </summary>
	internal class AdvCommandPageControler : AdvCommand
	{
		public AdvCommandPageControler(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
		}

		//ページ用のデータを作成
		public override void MakePageData(AdvScenarioPageData pageData)
		{
			InitTextDataInPage(pageData.AddTextDataInPage(this));
		}
		
		public override void DoCommand(AdvEngine engine)
		{
			engine.Page.UpdatePageTextData (TextDataInPage);
		}
		
		public override bool Wait(AdvEngine engine)
		{
			return engine.Page.IsWaitTextCommand;
		}
		
		//ページ区切りのコマンドか
		public override bool IsTypePageEnd() { return TextDataInPage.IsPageEnd; }
	}
}