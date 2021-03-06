//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// コマンド：サブルーチンにジャンプ
	/// </summary>
	internal class AdvCommandJumpSubroutine : AdvCommand
	{
		public AdvCommandJumpSubroutine(StringGridRow row, AdvSettingDataManager dataManager)
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
			this.returnLabel = IsEmptyCell(AdvColumnName.Arg3) ? ""  : ParseScenarioLabel(AdvColumnName.Arg3);
		}

		//ページ用のデータからコマンドに必要な情報を初期化
		public override void InitFromPageData(AdvScenarioPageData pageData)
		{
			this.scenaraioLabel = pageData.ScenarioLabelData.ScenaioLabel;
			this.subroutineCommandIndex = pageData.ScenarioLabelData.CountSubroutineCommandIndex(this);
		}

		public override string[] GetJumpLabels()
		{
			if (string.IsNullOrEmpty(returnLabel))
			{
				return new string[] { jumpLabel };
			}
			else
			{
				return new string[] { jumpLabel, returnLabel };
			}
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (IsEnable(engine.Param))
			{
				SubRoutineInfo info = new SubRoutineInfo( engine, this.returnLabel, this.scenaraioLabel, this.subroutineCommandIndex);
				engine.ScenarioPlayer.JumpManager.RegistoreSubroutine(jumpLabel, info);
			}
		}

		//ページ区切りのコマンドか
		public override bool IsTypePageEnd() { return true; }


		bool IsEnable( AdvParamManager param )
		{
			return (exp == null || param.CalcExpressionBoolean( exp ) );
		}

		string scenaraioLabel;
		int subroutineCommandIndex;
		string jumpLabel;
		string returnLabel;
		ExpressionParser exp;	//ジャンプ条件式
	}
}