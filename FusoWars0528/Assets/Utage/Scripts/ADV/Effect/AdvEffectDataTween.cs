//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// Tweenアニメーションのデータ
	/// </summary>
	public class AdvEffectDataTween : AdvEffectData
	{
		protected iTweenData tweenData;

		protected AdvEffectDataTween() { }

		public AdvEffectDataTween(AdvCommand command)
		{
			this.targetName = command.ParseCell<string>(AdvColumnName.Arg1);
			this.targetType = TargetType.Default;

			string type = command.ParseCell<string>(AdvColumnName.Arg2);
			string arg = command.ParseCellOptional<string>(AdvColumnName.Arg3, "");
			string easeType = command.ParseCellOptional<string>(AdvColumnName.Arg4, "");
			string loopType = command.ParseCellOptional<string>(AdvColumnName.Arg5, "");
			this.waitType = command.ParseCellOptional<WaitType>(AdvColumnName.Arg6, WaitType.Wait);

			this.tweenData = new iTweenData(type, arg, easeType, loopType);
			if (this.tweenData.Type == iTweenType.Stop)
			{
				waitType = WaitType.Add;
			}

			if (!string.IsNullOrEmpty(tweenData.ErrorMsg))
			{
				Debug.LogError(command.ToErrorString(tweenData.ErrorMsg));
			}
		}

		public override void OnStart(AdvEffectManager manager)
		{
			AdvEngine engine = manager.Engine;
			if (!string.IsNullOrEmpty(tweenData.ErrorMsg))
			{
				Debug.LogError(tweenData.ErrorMsg);
				OnComplete();
			}
			else
			{
				GameObject target = manager.FindTarget(this);
				if (target!=null)
				{
					isPlaying = true;
					AdvTweenPlayer player = target.AddComponent<AdvTweenPlayer>();
					float skipSpeed = engine.Page.CheckSkip() ? engine.Config.SkipSpped : 0;
					player.Init(tweenData, engine.GraphicManager.PixelsToUnits, skipSpeed, OnComplete);
					player.Play();
					if (player.IsEndlessLoop) waitType = WaitType.Add;
				}
				else
				{
					//記述ミス
					Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.NotFoundTweenGameObject, this.targetName ));
					OnComplete();
				}
			}
		}

		void OnComplete(AdvTweenPlayer player)
		{
			OnComplete();
		}
	}
}
