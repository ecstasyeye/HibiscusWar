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
	internal class AdvCommandBgEvent : AdvCommand
	{
		public AdvCommandBgEvent(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			this.label = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.TextureSetting.ContainsLabel(label))
			{
				Debug.LogError(ToErrorString(label + " is not contained in file setting"));
			}

			this.graphic = dataManager.TextureSetting.LabelToGraphic(label);
			AddLoadGraphic(graphic);
			this.graphicOperaitonArg = new AdvGraphicOperaitonArg(this, graphic, ParseCellOptional<float>(AdvColumnName.Arg6, 0.2f) );
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.SystemSaveData.GalleryData.AddCgLabel(label);
			//表示する
			engine.GraphicManager.IsEventMode = true;
			//キャラクターは非表示にする
			engine.GraphicManager.CharacterManager.FadeOutAll(graphicOperaitonArg.FadeTime);
			//表示する
			engine.GraphicManager.BgManager.DrawToDefault(engine.GraphicManager.BgSpriteName, graphicOperaitonArg);
		}

		string label;
		GraphicInfoList graphic;
		protected AdvGraphicOperaitonArg graphicOperaitonArg;
	}
}