//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Utage;
using System.Collections;
using System.Collections.Generic;



/// <summary>
/// コンフィグ画面のサンプル
/// </summary>
[AddComponentMenu("Utage/TemplateUI/Config")]
public class UtageUguiConfig : UguiView
{
	/// <summary>ADVエンジン</summary>
	public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>() as AdvEngine); } }
	[SerializeField]
	AdvEngine engine;

	//コンフィグデータへのインターフェース
	AdvConfig Config { get { return Engine.Config; } }

	/// <summary>タイトル画面</summary>
	[SerializeField]
	UtageUguiTitle title;

	/// <summary>「フルスクリーン表示」のチェックボックス</summary>
	[SerializeField]
	Toggle checkFullscreen;

	/// <summary>「マウスホイールでメッセージ送り」のチェックボックス</summary>
	[SerializeField]
	Toggle checkMouseWheel;

	/// <summary>「未読スキップ」のチェックボックス</summary>
	[SerializeField]
	Toggle checkSkipUnread;

	/// <summary>「選択肢でスキップを解除」チェックボックス</summary>
	[SerializeField]
	Toggle checkStopSkipInSelection;

	/// <summary>「メッセージ速度」のスライダー</summary>
	[SerializeField]
	UnityEngine.UI.Slider sliderMessageSpeed;

	/// <summary>「自動メッセージ速度」のスライダー</summary>
	[SerializeField]
	UnityEngine.UI.Slider sliderAutoBrPageSpeed;

	/// <summary>「ウインドウの透明度」のスライダー</summary>
	[SerializeField]
	UnityEngine.UI.Slider sliderMessageWindowTransparency;

	/// <summary>「サウンド」の音量スライダー</summary>
	[SerializeField]
	UnityEngine.UI.Slider sliderSoundMasterVolume;

	/// <summary>「BGM」の音量スライダー</summary>
	[SerializeField]
	UnityEngine.UI.Slider sliderBgmVolume;

	/// <summary>「SE」の音量スライダー</summary>
	[SerializeField]
	UnityEngine.UI.Slider sliderSeVolume;

	/// <summary>「環境音」の音量スライダー</summary>
	[SerializeField]
	UnityEngine.UI.Slider sliderAmbienceVolume;

	/// <summary>「ボイス」の音量スライダー</summary>
	[SerializeField]
	UnityEngine.UI.Slider sliderVoiceVolume;

	/// <summary>音声の再生タイプのラジオボタン</summary>
	[SerializeField]
	UguiToggleGroupIndexed radioButtonsVoiceStopType;

	//文字送り速度
	public float MessageSpeed { set { if (!IsInit) return; Config.MessageSpeed = value; } }
	
	//オート文字送り速度
	public float AutoBrPageSpeed { set { if (!IsInit) return; Config.AutoBrPageSpeed = value; } }

	//メッセージウィンドウの透過色（バー）
	public float MessageWindowTransparency { set { if (!IsInit) return; Config.MessageWindowTransparency = value; } }

	//音量設定 サウンド全体
	public float SoundMasterVolume { set { if (!IsInit) return; Config.SoundMasterVolume = value; } }

	//音量設定 BGM
	public float BgmVolume { set { if (!IsInit) return; Config.BgmVolume = value; } }

	//音量設定 SE
	public float SeVolume { set { if (!IsInit) return; Config.SeVolume = value; } }

	//音量設定 環境音
	public float AmbienceVolume { set { if (!IsInit) return; Config.AmbienceVolume = value; } }

	//音量設定 ボイス
	public float VoiceVolume { set { if (!IsInit) return; Config.VoiceVolume = value; } }

	//フルスクリーン切り替え
	public bool IsFullScreen { set { if (!IsInit) return; Config.IsFullScreen = value; } }

	//マウスホイールでメッセージ送り切り替え
	public bool IsMouseWheel { set { if (!IsInit) return; Config.IsMouseWheelSendMessage = value; } }

	//エフェクトON・OFF切り替え
	public bool IsEffect { set { if (!IsInit) return; Config.IsEffect = value; } }

	//未読スキップON・OFF切り替え
	public bool IsSkipUnread { set { if (!IsInit) return; Config.IsSkipUnread = value; } }

	//選択肢でスキップ解除ON・OFF切り替え
	public bool IsStopSkipInSelection { set { if (!IsInit) return; Config.IsStopSkipInSelection = value; } }

	public bool IsInit { get { return isInit; } set { isInit = value; } }
	bool isInit = false;

	/// <summary>
	/// オープンしたときに呼ばれる
	/// </summary>
	void OnOpen()
	{
		isInit = false;
		//スクショをクリア
		if (Engine.SaveManager.Type != AdvSaveManager.SaveType.SavePoint)
		{
			Engine.SaveManager.ClearCaptureTexture();
		}
		StartCoroutine(CoWaitOpen());
	}


	//起動待ちしてから開く
	IEnumerator CoWaitOpen()
	{
		while (Engine.IsWaitBootLoading) yield break;
		LoadValues();
	}

	/// <summary>
	/// 画面を閉じる処理
	/// </summary>
	public override void Close()
	{
		Engine.WriteSystemData();
		base.Close();
	}

	void Update()
	{
		//右クリックで戻る
		if (isInit && InputUtil.IsMousceRightButtonDown())
		{
			Back();
		}
	}

	//各UIに値を反映
	void LoadValues()
	{
		isInit = false;
		checkFullscreen.isOn = Config.IsFullScreen;
		checkMouseWheel.isOn = Config.IsMouseWheelSendMessage;
		checkSkipUnread.isOn = Config.IsSkipUnread;
		checkStopSkipInSelection.isOn = Config.IsStopSkipInSelection;

		sliderMessageSpeed.value = Config.MessageSpeed;
		sliderAutoBrPageSpeed.value = Config.AutoBrPageSpeed;
		sliderMessageWindowTransparency.value = Config.MessageWindowTransparency;
		sliderSoundMasterVolume.value = Config.SoundMasterVolume;
		sliderBgmVolume.value = Config.BgmVolume;
		sliderSeVolume.value = Config.SeVolume;
		sliderAmbienceVolume.value = Config.AmbienceVolume;
		sliderVoiceVolume.value = Config.VoiceVolume;

		radioButtonsVoiceStopType.CurrentIndex = (int)Config.VoiceStopType;

		//フルスクリーンやマウスホイールは、PC版のみの操作
		if (!UtageToolKit.IsPlatformStandAloneOrEditor())
		{
			checkFullscreen.gameObject.SetActive(false);
			checkMouseWheel.gameObject.SetActive(false);
		}
		isInit = true;
	}

	//タイトルに戻る
	public void OnTapBackTitle()
	{
		Engine.EndScenario();
		this.Close();
		title.Open();
	}

	//全てデフォルト値で初期化
	public void OnTapInitDefaultAll()
	{
		if (!IsInit) return;
		Config.InitDefaultAll();
		LoadValues();
	}

	//音声設定（クリックで停止、次の音声まで再生を続ける）
	public void OnTapRadioButtonVoiceStopType( int index )
	{
		if (!IsInit) return;
		Config.VoiceStopType = (VoiceStopType)index;
	}
}
