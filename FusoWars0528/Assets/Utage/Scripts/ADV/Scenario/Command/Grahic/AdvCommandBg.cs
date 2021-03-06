//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：背景表示・切り替え
	/// </summary>
	internal class AdvCommandBg : AdvCommand
	{
		public AdvCommandBg(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			string label = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.TextureSetting.ContainsLabel(label))
			{
				Debug.LogError(ToErrorString(label + " is not contained in file setting"));
			}

			this.graphic = dataManager.TextureSetting.LabelToGraphic(label);
			AddLoadGraphic(graphic);

			//グラフィック表示処理を作成
			this.graphicOperaitonArg = new AdvGraphicOperaitonArg( this, graphic, ParseCellOptional<float>(AdvColumnName.Arg6, 0.2f));
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.GraphicManager.IsEventMode = false;
			//表示する
			engine.GraphicManager.BgManager.DrawToDefault(engine.GraphicManager.BgSpriteName, graphicOperaitonArg);
		}

		protected GraphicInfoList graphic;
		protected AdvGraphicOperaitonArg graphicOperaitonArg;
	}
}