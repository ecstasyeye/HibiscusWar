//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEditor;
using UnityEngine;

namespace Utage
{

	public class CustomCreateAsset : EditorWindow
	{
		const string roort = "Assets/Create/Utage/";
		[MenuItem(roort + "CustomProjectSetting")]
		static public void CreateCustomProjectSetting()
		{
			UtageEditorToolKit.CreateNewUniqueAsset<CustomProjectSetting>();
		}

		[MenuItem(roort + "Node2DSordData")]
		static public void CreateNode2DSortData()
		{
			UtageEditorToolKit.CreateNewUniqueAsset<Node2DSortData>();
		}

		[MenuItem(roort + "LanguageManager")]
		static public void CreateLanguageManager()
		{
			UtageEditorToolKit.CreateNewUniqueAsset<LanguageManager>();
		}

#if LegacyUtageUi
		[MenuItem(roort + "Legacy/FontData")]
		static public void CreateFontData()
		{
			UtageEditorToolKit.CreateNewUniqueAsset<FontData>();
		}
#endif

		[MenuItem(roort + "TextSettings")]
		static public void CreateTextSettings()
		{
			UtageEditorToolKit.CreateNewUniqueAsset<UguiNovelTextSettings>();
		}
		
		[MenuItem(roort + "EmojiData")]
		static public void CreateEmojiData()
		{
			UtageEditorToolKit.CreateNewUniqueAsset<UguiNovelTextEmojiData>();
		}
	}
}