//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{
	//宴のエディター設定ウィンドウ
	public class AdvEditorSettingWindow : CustomEditorWindow
	{
		public static AdvEditorSettingWindow GetInstance()
		{
			AdvEditorSettingWindow instance = EditorWindow.GetWindow(typeof(AdvEditorSettingWindow)) as AdvEditorSettingWindow;
			instance.Load();
			instance.Close();
			return instance;
		}

		//EditorPrefsにセーブする
		protected override SaveType EditorSaveType
		{
			get { return SaveType.EditorPrefs; }
		}

		//********シナリオインポート時のチェック********//

		//宴のシーンが切り替わったら、自動で宴プロジェクトを切り替える
		public bool AutoChangeProject { get { return autoChangeProject; } }
		[SerializeField]
		bool autoChangeProject = true;
/*
		//Unityプロジェクト内の全ての宴プロジェクトを対象にインポートをする
		public bool ImportAllUtageProject { get { return importAllUtageProject; } }
		[SerializeField]
		bool importAllUtageProject = false;

		//全てのエクセルファイルのインポート時に、宴プロジェクトのチェックをする
		public bool CheckAllExcelFiles { get { return checkAllExcelFiles; } }
		[SerializeField]
		bool checkAllExcelFiles = false;

		//テキストあふれのチェックをするか
		public bool checkTexOver = true;

		//文字数をカウントするか
		public bool countChacacters = true;
*/
		//********シーン変更時のチェック********//

		//宴のシーンが切り替わったら、自動でシーンのチェックをするか
		public bool AutoCheckScene { get { return autoCheckScene; } }
		[SerializeField]
		bool autoCheckScene = true;

		//Unityのバージョンアップによる致命的な不具合をチェックする
		public bool AutoCheckUnityVersionUp { get { return autoCheckUnityVersionUp; } }
		[SerializeField]
		bool autoCheckUnityVersionUp = true;

		//インポート時に空白をチェックする
		public bool CheckWhiteSpaceOnImport { get { return checkWhiteSpaceOnImport; } }
		[SerializeField]
		bool checkWhiteSpaceOnImport = true;

		//パスワードをチェックする
//		public bool autoCheckPassWord = true;
	}
}
