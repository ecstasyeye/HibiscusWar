//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Utage
{
	[System.Serializable]
	public class AdvScenarioPlayerEvent : UnityEvent<AdvScenarioPlayer> { }
	[System.Serializable]
	public class AdvCommandEvent : UnityEvent<AdvCommand> { }

	/// <summary>
	/// シナリオを進めていくプレイヤー
	/// </summary>
	[AddComponentMenu("Utage/ADV/Internal/ScenarioPlayer")]
	public class AdvScenarioPlayer : MonoBehaviour
	{
		/// <summary>
		/// 「SendMessage」コマンドが実行されたときにSendMessageを受け取るGameObject
		/// </summary>
		public GameObject SendMessageTarget { get { return sendMessageTarget; } }
		[SerializeField]
		GameObject sendMessageTarget;

		[System.Flags]
		enum DebugOutPut
		{
			Log = 0x01,
			Waiting = 0x02,
			CommandEnd = 0x04,
		};
		[SerializeField]
		[EnumFlags]
		DebugOutPut debugOutPut = 0;

		[SerializeField]
		int maxFilePreload = 20;	///事前にロードするファイルの最大数
		HashSet<AssetFile> preloadFileSet = new HashSet<AssetFile>();

		/// <summary>
		///　シナリオ終了時に呼ばれる
		/// </summary>
		public AdvScenarioPlayerEvent OnEndScenario { get { return this.onEndScenario; } }
		[SerializeField]
		public AdvScenarioPlayerEvent onEndScenario = new AdvScenarioPlayerEvent();

		/// <summary>
		///　シナリオポーズ時に呼ばれる
		/// </summary>
		public AdvScenarioPlayerEvent OnPauseScenario { get { return this.onPauseScenario; } }
		[SerializeField]
		public AdvScenarioPlayerEvent onPauseScenario = new AdvScenarioPlayerEvent();

		/// <summary>
		///　シナリオ終了かポーズ時に呼ばれる
		/// </summary>
		public AdvScenarioPlayerEvent OnEndOrPauseScenario { get { return this.onEndOrPauseScenario; } }
		[SerializeField]
		public AdvScenarioPlayerEvent onEndOrPauseScenario = new AdvScenarioPlayerEvent();

		/// <summary>
		///　コマンド開始時に呼ばれる
		/// </summary>
		public AdvCommandEvent OnBeginCommand { get { return this.onBeginCommand; } }
		[SerializeField]
		public AdvCommandEvent onBeginCommand = new AdvCommandEvent();

		/// <summary>
		///　コマンド待機中の前に呼ばれる
		/// </summary>
		public AdvCommandEvent OnUpdatePreWaitingCommand { get { return this.onUpdatePreWaitingCommand; } }
		[SerializeField]
		public AdvCommandEvent onUpdatePreWaitingCommand = new AdvCommandEvent();		

		/// <summary>
		///　コマンド待機中に呼ばれる
		/// </summary>
		public AdvCommandEvent OnUpdateWaitingCommand { get { return this.onUpdateWaitingCommand; } }
		[SerializeField]
		public AdvCommandEvent onUpdateWaitingCommand = new AdvCommandEvent();

		/// <summary>
		///　コマンド終了時に呼ばれる
		/// </summary>
		public AdvCommandEvent OnEndCommand { get { return this.onEndCommand; } }
		[SerializeField]
		public AdvCommandEvent onEndCommand = new AdvCommandEvent();

		/// <summary>
		/// 現在の、シーン回想用のシーンラベル
		/// </summary>
		public string CurrentGallerySceneLabel { get { return this.currentGallerySceneLabel;}  set{ this.currentGallerySceneLabel = value;} }
		string currentGallerySceneLabel = "";


		/// <summary>
		/// ロード中か
		/// </summary>
		public bool IsWaitLoading { get { return this.isWaitLoading ; } }
		bool isWaitLoading = false;

		/// <summary>
		/// シナリオ終了したか
		/// </summary>
		public bool IsEndScenario { get { return this.isEndScenario; } }
		bool isEndScenario = false;

		public bool IsPausing { get; private set; }

		public void Pause()
		{
			IsPausing = true;
			this.OnPauseScenario.Invoke(this);
			this.OnEndOrPauseScenario.Invoke(this);
		}
		public void Resume()
		{
			IsPausing = false;
		}


		/// <summary>
		/// シナリオ実行中か
		/// </summary>
		public bool IsPlayingScenario { get; private set; }

		//If文制御のマネージャー
		internal AdvIfManager IfManager { get { return this.ifManager; } }
		AdvIfManager ifManager = new AdvIfManager();

		//ジャンプのマネージャー
		internal AdvJumpManager JumpManager { get { return this.jumpManager; } }
		AdvJumpManager jumpManager = new AdvJumpManager();

		AdvEngine Engine { get { return this.engine ?? (this.engine = GetComponent<AdvEngine>()); } }
		AdvEngine engine;

		/// <summary>
		/// 古いバージョンのセーブデータか
		/// </summary>
		bool IsOldVersion{ get; set;}

		bool IsBreakCommand
		{
			get { return JumpManager.IsReserved || IsReservedEndScenario;}
		}

		//
		bool IsReservedEndScenario {
			get { return isReservedEndScenario;}
		}
		bool isReservedEndScenario;
		public void ReserveEndScenario()
		{
			isReservedEndScenario = true;
		}

		/// <summary>
		/// ジャンプ時に最初の状態に
		/// </summary>
		void ResetOnJump()
		{
			isReservedEndScenario = false;
			isWaitLoading = false;
			ifManager.Clear();
			jumpManager.ClearOnJump();
			ClearPreload();
		}

		/// <summary>
		/// シナリオの実行開始
		/// </summary>
		/// <param name="engine">ADVエンジン</param>
		/// <param name="scenarioLabel">ジャンプ先のシナリオラベル</param>
		/// <param name="page">シナリオラベルからのページ数</param>
		/// <param name="scenarioLabel">ジャンプ先のシーン回想用シナリオラベル</param>
		public void StartScenario(string label, int page)
		{
			Clear();
			this.IsPausing = false;
			this.isEndScenario = false;

			StartCoroutine( CoStartScenario(label, page, null));
		}


		internal void StartSaveData(AdvSaveData saveData)
		{
			Clear();
			this.isEndScenario = false;
			//現在のシーン回想登録用のラベルを記録
			this.currentGallerySceneLabel = saveData.CurrentGallerySceneLabel;
			//古いセーブデータかを設定しておく
			this.IsOldVersion = (saveData.FileVersion <= AdvSaveData.Version2);
			//ジャンプマネージャーを初期化
			BinaryUtil.BinaryRead(saveData.JumpMangerBuffer, (x) => this.JumpManager.Read(Engine, x));
			//バージョンアップ用のセーブデータ
			saveData.VersionUpBuffer.ReadCustomSaveData(Engine.SaveManager.GetVersionUpSaveIoListCreateIfMissing(Engine));
			//カスタムセーブデータ
			saveData.CustomBuffer.ReadCustomSaveData(Engine.SaveManager.CustomSaveDataIOList);
			//シナリオ開始
			StartCoroutine(CoStartScenario(saveData.CurrentSenarioLabel, saveData.CurrentPage, null));
		}

		
		/// <summary>
		/// シナリオ終了
		/// </summary>
		public void EndScenario()
		{
			this.OnEndScenario.Invoke(this);
			this.OnEndOrPauseScenario.Invoke(this);
			Engine.ClearOnEnd();
			IsPlayingScenario = false;
			isEndScenario = true;
		}

		/// <summary>
		/// クリア処理
		/// </summary>
		public void Clear()
		{
			IsPlayingScenario = false;
			ResetOnJump();
			currentGallerySceneLabel = "";
			jumpManager.Clear();
			StopAllCoroutines();
		}

		//登録先にジャンプ
		void JumpToReserved()
		{
			//前回の実行がまだ回ってるかもしれないので止める
			StopAllCoroutines();
			if (JumpManager.SubRoutineReturnInfo!=null)
			{
				SubRoutineInfo info = JumpManager.SubRoutineReturnInfo;
				StartCoroutine(CoStartScenario(info.ReturnLabel, info.ReturnPageNo, info.ReturnCommand));
			}
			else
			{
				StartCoroutine(CoStartScenario(JumpManager.Label, 0, null));
			}
		}

		//指定のシナリオを再生
		IEnumerator CoStartScenario(string label, int page, AdvCommand returnToCommand)
		{
			IsPlayingScenario = true;
			//ジャンプ先のシナリオラベルのログを出力
			if ((debugOutPut & DebugOutPut.Log) == DebugOutPut.Log) Debug.Log("Jump : " + label + " :" + page);
			
			//起動時のロード待ち
			while (Engine.IsLoading)
			{
				yield return 0;
			}


			//シナリオロード待ち
			isWaitLoading = true;
			while (!Engine.DataManager.IsLoadEndScenarioLabel(label))
			{
				yield return 0;
			}
			isWaitLoading = false;

			//各データをリセット
			ResetOnJump();

			if (page < 0) page = 0;
			//ページ指定がある場合はif分岐の設定をしておく
			if (page != 0) ifManager.IsLoadInit = true;

			//ジャンプ先のシナリオデータを取得
			AdvScenarioLabelData currentLabelData = Engine.DataManager.FindScenarioLabelData(label);
			while (currentLabelData!=null)
			{
				UpdateSceneGallery(currentLabelData.ScenaioLabel, engine);
				AdvScenarioPageData cuurentPageData = currentLabelData.GetPageData(page);
				//ページデータを取得
				while (cuurentPageData != null)
				{
					//プリロードを更新
					UpdatePreLoadFiles(currentLabelData.ScenaioLabel, page);

					///ページ開始処理
					Engine.Page.BeginPage(cuurentPageData);

					//0フレーム即コルーチンが終わる場合を考えてこう書く
					var pageCoroutine = StartCoroutine(CoStartPage(currentLabelData, cuurentPageData, returnToCommand ));
					if (pageCoroutine != null)
					{
						yield return pageCoroutine;
					}
					currentCommand = null;
					returnToCommand = null;
					while(Engine.EffectManager.IsPageWaiting ) yield return 0;

					//古いバージョンのロード処理は終了
					IsOldVersion = false;

					///改ページ処理
					Engine.Page.EndPage();
					if(IsBreakCommand)
					{
						if( IsReservedEndScenario)
						{
							break;
						}
						if( JumpManager.IsReserved ) JumpToReserved();
						yield break;
					}

					cuurentPageData = currentLabelData.GetPageData(++page);
				}
				if (IsReservedEndScenario)
				{
					break;
				}
				//ロード直後処理終了
				IfManager.IsLoadInit = false;
				currentLabelData = Engine.DataManager.NextScenarioLabelData(currentLabelData.ScenaioLabel);
				page = 0;
			}
			EndScenario();
		}

		//一ページ内のコマンド処理
		IEnumerator CoStartPage( AdvScenarioLabelData labelData,  AdvScenarioPageData pageData, AdvCommand returnToCommand )
		{
			int index = 0;
			AdvCommand command = pageData.GetCommand(index);

			if (returnToCommand != null)
			{
				while (command!=returnToCommand)
				{
					command = pageData.GetCommand(++index);
				}
			}

			//復帰直後はIf内分岐は無効
			if( IfManager.IsLoadInit )
			{
				index = pageData.GetIfSkipCommandIndex(index);
				command = pageData.GetCommand(index);
			}

			while (command!=null)
			{
				if(command.IsEntityType)
				{
					command = command.CreateEntityCommand(Engine,pageData);
				}

				//古いセーブデータのロード中はページ末までスキップ
				if (IsOldVersion && !command.IsTypePageEnd())
				{
					command = pageData.GetCommand(++index);
					continue;
				}

				//ifスキップチェック
				if(IfManager.CheckSkip(command))
				{
					if ((debugOutPut & DebugOutPut.Log) == DebugOutPut.Log) Debug.Log("Command If Skip: " + command.GetType() + " " + labelData.ScenaioLabel + ":" + pageData.PageNo);
					command = pageData.GetCommand(++index);
					continue;
				}

				currentCommand = command;
				//ロード
				command.Load();

				//ロード待ち
				while (!command.IsLoadEnd())
				{
					isWaitLoading = true;
					yield return 0;
				}
				isWaitLoading = false;

				//コマンド実行
				if ((debugOutPut & DebugOutPut.Log) == DebugOutPut.Log) Debug.Log("Command : " + command.GetType() + " " + labelData.ScenaioLabel + ":" + pageData.PageNo);
				this.OnBeginCommand.Invoke(command);
				command.DoCommand(engine);
				///ページ末端・オートセーブデータを更新
//				if (command.IsTypePageEnd())
//				{
//					///ページ開始処理
//					engine.Page.BeginPage(currentScenarioLabel, currentPage);
//					engine.SaveManager.UpdateAutoSaveData(engine);
//				}

				//コマンド実行後にファイルをアンロード
				command.Unload();

				while (IsPausing )
				{
					yield return 0;
				}
				//コマンドの処理待ち
				while (true )
				{
					this.OnUpdatePreWaitingCommand.Invoke(command);
					if (!command.Wait(engine))
					{
						break;
					}
					if ((debugOutPut & DebugOutPut.Waiting) == DebugOutPut.Waiting) Debug.Log("Wait..." + command.GetType());
					this.OnUpdateWaitingCommand.Invoke(command);
					yield return 0;
				}
				if ((debugOutPut & DebugOutPut.CommandEnd) == DebugOutPut.CommandEnd) Debug.Log("End :" + command.GetType() + " " + labelData.ScenaioLabel + ":" + pageData.PageNo);
				this.OnEndCommand.Invoke(command);

				Engine.UiManager.IsInputTrig = false;
				Engine.UiManager.IsInputTrigCustom = false;

				if (IsBreakCommand)
				{
					yield break;
				}
				command = pageData.GetCommand(++index);
			}
		}

		//先読みファイルをクリア
		void ClearPreload()
		{
			//直前の先読みファイルは参照を減算しておく
			foreach (AssetFile file in preloadFileSet)
			{
				file.Unuse(this);
			}
			preloadFileSet.Clear();
		}

		//先読みかけておく
		void UpdatePreLoadFiles(string scenarioLabel, int page)
		{
			//直前までの先読みファイルリスト
			HashSet<AssetFile> lastPreloadFileSet = preloadFileSet;
			//今回の先読みファイルリスト
			preloadFileSet = Engine.DataManager.MakePreloadFileList(scenarioLabel, page, maxFilePreload);

			if (preloadFileSet == null) preloadFileSet = new HashSet<AssetFile>();

			//リストに従って先読み
			foreach (AssetFile file in preloadFileSet)
			{
				//先読み
				AssetFileManager.Preload(file, this);
			}

			//直前の先読みファイルのうち、今回の先読みファイルからなくなったものは使用状態を解除する
			foreach (AssetFile file in lastPreloadFileSet)
			{
				//もうプリロードされなくなったリストを作るために
				if (!(preloadFileSet.Contains(file)))
				{
					file.Unuse(this);
				}
			}
		}


		/// <summary>
		/// シーン回想のためにシーンラベルを更新
		/// </summary>
		/// <param name="label">シーンラベル</param>
		/// <param name="engine">ADVエンジン</param>
		void UpdateSceneGallery(string label, AdvEngine engine)
		{
			AdvSceneGallerySetting galleryData = engine.DataManager.SettingDataManager.SceneGallerySetting;
			if (galleryData.Contains(label))
			{
				if (currentGallerySceneLabel != label)
				{
					if (!string.IsNullOrEmpty(currentGallerySceneLabel))
					{
						//別のシーンが終わってないのに、新しいシーンに移っている
						Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.UpdateSceneLabel, currentGallerySceneLabel, label));
					}
					currentGallerySceneLabel = label;
				}
			}
		}

		/// <summary>
		/// シーン回想のためのシーンの終了処理
		/// </summary>
		/// <param name="engine">ADVエンジン</param>
		public void EndSceneGallery(AdvEngine engine)
		{
			if (string.IsNullOrEmpty(currentGallerySceneLabel))
			{
				//シーン回想に登録されていないのに、シーン回想終了がされています
				Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.EndSceneGallery));
			}
			else
			{
				engine.SystemSaveData.GalleryData.AddSceneLabel(currentGallerySceneLabel);
				currentGallerySceneLabel = "";
			}
		}

		AdvCommand currentCommand;
		public AdvCommand CurrentCommand{ get{ return currentCommand;} }
		public bool IsCurrentCommand(AdvCommand command)
		{
			return (command !=null) && (currentCommand == command);
		}
	}
}