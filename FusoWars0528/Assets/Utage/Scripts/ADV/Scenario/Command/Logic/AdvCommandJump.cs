//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// コマンド：他他シナリオにジャンプ
	/// </summary>
	internal class AdvCommandJump : AdvCommand
	{
		public AdvCommandJump(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			this.jumpLabel = ParseScenarioLabel(AdvColumnName.Arg1);
			string expStr = ParseCellOptional<string>(AdvColumnName.Arg2, "");
			if (string.IsNullOrEmpty(expStr))
			{
				this.exp = null;
			}
			else
			{
				this.exp = dataManager.DefaultParam.CreateExpressionBoolean(expStr);
				if (this.exp.ErrorMsg != null)
				{
					Debug.LogError(ToErrorString(this.exp.ErrorMsg));
				}
			}
		}

		public override string[] GetJumpLabels() { return new string[] { jumpLabel }; }

		public override void DoCommand(AdvEngine engine)
		{
			if (IsEnable(engine.Param))
			{
				engine.ScenarioPlayer.JumpManager.RegistoreLabel(jumpLabel);
			}
		}

		bool IsEnable( AdvParamManager param )
		{
			return (exp == null || param.CalcExpressionBoolean( exp ) );
		}

		string jumpLabel;
		ExpressionParser exp;	//ジャンプ条件式
	}
}