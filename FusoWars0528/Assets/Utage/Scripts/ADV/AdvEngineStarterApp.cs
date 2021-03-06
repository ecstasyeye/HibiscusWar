//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using Utage;
using System.Collections;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// ゲーム起動処理のサンプル
	/// </summary>
	[AddComponentMenu("Utage/ADV/AdvEngineStarterApp")]
	public class AdvEngineStarterApp : MonoBehaviour
	{
		//エクセルのファイルパス
		[SerializeField]
		string excelPath = "";
		
		//Awake時にロードする
		[SerializeField]
		bool isLoadOnAwake = true;

		/// <summary>開始フレームで自動でADVエンジンを起動する</summary>
		[SerializeField]
		bool isAutomaticPlay = false;

		/// <summary>開始シナリオラベル</summary>
		[SerializeField]
		string startScenario = "";

		/// <summary>ADVエンジン</summary>
		public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>() as AdvEngine); } }
		[SerializeField]
		AdvEngine engine;

		/// <summary>リソースディレクトリのルートパス</summary>
		[SerializeField]
		string rootResourceDir = "Sample";
		string ResourceDir { get { return rootResourceDir; } }

		void Awake()
		{
			if (isLoadOnAwake)
			{
				LoadEngine();
			}
		}

		public void LoadEngine()
		{
			if (!string.IsNullOrEmpty (startScenario)) 
			{
				Engine.StartScenarioLabel = startScenario;
			}

			AssetFileManager.LoadInitFileList(new List<string>(), 0);

			//ADVエンジンの初期化を開始
			if (string.IsNullOrEmpty(ResourceDir)) { Debug.LogError("Not set ResourceData", this); return; }
			Engine.BootFromExcel(excelPath, ResourceDir);
			if (isAutomaticPlay)
			{
				StartCoroutine(CoPlayEngine());
			}
		}

		public void StartEngine()
		{
			StartCoroutine(CoPlayEngine());
		}

		IEnumerator CoPlayEngine()
		{
			while (Engine.IsWaitBootLoading) yield return 0;
			if (string.IsNullOrEmpty(startScenario))
			{
				Engine.StartGame();
			}
			else
			{
				Engine.StartGame(startScenario);
			}
		}
	}
}