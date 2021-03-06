//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// シェイクアニメーションのデータ
	/// </summary>
	public class AdvEffectDataShake : AdvEffectDataTween
	{
		public AdvEffectDataShake(AdvCommand command)
		{
			this.targetName = command.ParseCellOptional<string>(AdvColumnName.Arg1, "");
			if (!UtageToolKit.TryParaseEnum<TargetType>(this.targetName, out this.targetType))
			{
				this.targetType = TargetType.Default;
			}
			this.targetName = command.ParseCellOptional<string>(AdvColumnName.Arg2, this.targetName);
			string defaultStr = " x=30 y=30";
			string arg = command.ParseCellOptional<string>(AdvColumnName.Arg3, defaultStr);
			if (!arg.Contains("x=") && !arg.Contains("y="))
			{
				arg += defaultStr;
			}
			string easeType = command.ParseCellOptional<string>(AdvColumnName.Arg4, "");
			string loopType = command.ParseCellOptional<string>(AdvColumnName.Arg5, "");
			this.tweenData = new iTweenData(iTweenType.ShakePosition.ToString(), arg, easeType, loopType);

			this.waitType = command.ParseCellOptional<WaitType>(AdvColumnName.Arg6, WaitType.Wait);
			
			if (!string.IsNullOrEmpty(tweenData.ErrorMsg))
			{
				Debug.LogError(command.ToErrorString(tweenData.ErrorMsg));
			}
		}
	}
}
