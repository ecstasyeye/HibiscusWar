//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------


using UnityEngine;
using System.Collections.Generic;
using System;

namespace Utage
{
	/// <summary>
	/// 非アクティブなコンポーネントでも、強制的に初期化するための処理
	/// </summary>
	public interface IInitialzieInactive
	{
		void Initialzie();
	};
}
