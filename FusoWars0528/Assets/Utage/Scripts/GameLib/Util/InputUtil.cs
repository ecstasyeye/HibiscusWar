//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;

namespace Utage
{

	/// <summary>
	/// 入力処理
	/// </summary>
	public static class InputUtil
	{
		public static bool IsMousceRightButtonDown()
		{
			if( UtageToolKit.IsPlatformStandAloneOrEditor() )
			{ 
				return Input.GetMouseButtonDown(1);
			}
			else
			{
				return false;
			}
		}

		public static bool IsInputControl()
		{
			if( UtageToolKit.IsPlatformStandAloneOrEditor() )
			{ 
				return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
			}
			else
			{
				return false;
			}
		}

		static float wheelSensitive = 0.1f;
		public static bool IsInputScrollWheelUp()
		{
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis >= wheelSensitive )
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool IsInputScrollWheelDown()
		{
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis <= -wheelSensitive )
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool IsInputKeyboadReturnDown()
		{
			if (UtageToolKit.IsPlatformStandAloneOrEditor())
			{
				return Input.GetKeyDown(KeyCode.Return);
			}
			else
			{
				return false;
			}
		}
	}

}