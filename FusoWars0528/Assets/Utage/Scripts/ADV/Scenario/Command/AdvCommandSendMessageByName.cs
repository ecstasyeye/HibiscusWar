//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------
using System;
using UnityEngine;

namespace Utage
{

	/// <summary>
	/// コマンド：ゲーム固有の独自処理のためにSendMessageをする
	/// </summary>
	public class AdvCommandSendMessageByName : AdvCommand
	{
		public AdvCommandSendMessageByName(StringGridRow row)
			: base(row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			Engine = engine;
			string name = ParseCell<string>(AdvColumnName.Arg1);
			string function = ParseCell<string>(AdvColumnName.Arg2);
			GameObject go = GameObject.Find(name);
			if(go == null)
			{
				Debug.LogError( name + " is not found in current scene" );
			}
			else
			{
				UtageToolKit.SafeSendMessage(this, go, function);
			}
		}

		public override bool Wait(AdvEngine engine)
		{
			return IsWait;
		}

		public bool IsWait { get; set; }
		public AdvEngine Engine { get; private set; }
	}
}