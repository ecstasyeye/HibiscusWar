//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;

namespace Utage
{
	/// <summary>
	/// [EnumFlags]アトリビュート
	/// [Flags]用の表示
	/// </summary>
	public class EnumFlagsAttribute : PropertyAttribute	{}

	/// <summary>
	/// [LimitEnum]アトリビュート
	/// enumの一部だけ表示
	/// </summary>
	public class LimitEnumAttribute : PropertyAttribute
	{
		public string[] Args { get; private set; }
		public LimitEnumAttribute(params string[] args)
		{
			Args = args;
		}
	}

	/// <summary>
	/// [PathDialog]アトリビュート
	/// パス用の文字列を、選択ダイアログ用のボタンつきで表示する
	/// </summary>
	public class PathDialogAttribute : PropertyAttribute
	{
		public enum DialogType
		{
			Directory,
			File,
		};

		public DialogType Type { get; private set; }
		public string Extention { get; private set; }

		public PathDialogAttribute(DialogType type)
		{
			this.Type = type;
			this.Extention = "";
		}
		public PathDialogAttribute(DialogType type, string extention)
		{
			this.Type = type;
			this.Extention = extention;
		}
	}

	/// <summary>
	/// [NotEditable]アトリビュート
	/// 表示のみで編集を不可能にする
	/// </summary>
	public class NotEditableAttribute : PropertyAttribute
	{
		public string EnablePropertyPath { get; private set; }
		public bool IsEnableProperty { get; private set; }

		public NotEditableAttribute() { }
		public NotEditableAttribute(string enablePropertyPath, bool isEnableProperty = true)
		{
			this.EnablePropertyPath = enablePropertyPath;
			this.IsEnableProperty = isEnableProperty;
		}
	}

	/// <summary>
	/// [Hide]アトリビュート
	/// 条件によって非表示にする
	/// </summary>
	public class HideAttribute : PropertyAttribute
	{
		public string EnablePropertyPath { get; private set; }
		public bool IsEnableProperty { get; private set; }

		public HideAttribute() { }
		public HideAttribute(string enablePropertyPath, bool isEnableProperty = true)
		{
			this.EnablePropertyPath = enablePropertyPath;
			this.IsEnableProperty = isEnableProperty;
		}
	}
}
