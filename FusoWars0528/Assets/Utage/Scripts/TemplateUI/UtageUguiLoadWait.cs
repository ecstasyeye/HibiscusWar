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
/// ロード待ち画面のサンプル
/// </summary>
[AddComponentMenu("Utage/TemplateUI/LoadWait")]
public class UtageUguiLoadWait : UguiView
{
    /// <summary>ADVエンジン</summary>
    public AdvEngine Engine { get { return this.engine ?? (this.engine = FindObjectOfType<AdvEngine>() as AdvEngine); } }
    [SerializeField]
    AdvEngine engine;

    /// <summary>スターター</summary>
    public AdvEngineStarter Starter { get { return this.starter ?? (this.starter = FindObjectOfType<AdvEngineStarter>()); } }
    [SerializeField]
    AdvEngineStarter starter;

    public bool isAutoCacheFileLoad;

    public UtageUguiTitle title;

    public string bootSceneName;

    public GameObject buttonSkip;
    public GameObject buttonBack;
    public GameObject buttonDownload;
    public GameObject loadingBarRoot;
    public Image loadingBar;
    public Text textMain;
    public Text textCount;

    /// <summary>
    /// ダイアログ呼び出し
    /// </summary>
    public OpenDialogEvent OnOpenDialog
    {
        set { this.onOpenDialog = value; }
        get
        {
            //ダイアログイベントに登録がないなら、SystemUiのダイアログを使う
            if (this.onOpenDialog.GetPersistentEventCount() == 0)
            {
                if (SystemUi.GetInstance() != null)
                {
                    onOpenDialog.AddListener(SystemUi.GetInstance().OpenDialog);
                }
            }
            return onOpenDialog;
        }
    }
    [SerializeField]
    OpenDialogEvent onOpenDialog;

    enum State
    {
        Start,
        Downloding,
        DownlodFinished,
    };
    State CurrentState { get; set; }

	enum Type
	{
		Default,
		Boot,
		ChapterDownload,
	};
	Type DownloadType { get; set; }


    //起動時に開く
    public void OpenOnBoot()
    {
		DownloadType = Type.Boot;
        this.Open();
    }
    void OnClose()
    {
		DownloadType = Type.Default;
	}

    void OnOpen()
    {
		switch (DownloadType)
		{
			case Type.Boot:
				if (this.buttonBack) this.buttonBack.SetActive(false);
				if (this.buttonSkip) this.buttonSkip.SetActive(true);
				if (this.buttonDownload) this.buttonDownload.SetActive(true);
				break;
			case Type.Default:
				if (this.buttonBack) this.buttonBack.SetActive(true);
				if (this.buttonSkip) this.buttonSkip.SetActive(false);
				if (this.buttonDownload) this.buttonDownload.SetActive(true);
				break;
			case Type.ChapterDownload:
				if (this.buttonBack) this.buttonBack.SetActive(false);
				if (this.buttonSkip) this.buttonSkip.SetActive(false);
				if (this.buttonDownload) this.buttonDownload.SetActive(false);
				break;
		}

        if (!Engine.IsStarted)
        {
            ChangeState(State.Start);
        }
        else
        {
            ChangeState(State.Downloding);
        }
    }

    void ChangeState(State state)
    {
        this.CurrentState = state;
        switch (state)
        {
            case State.Start:
                buttonDownload.SetActive(true);
                loadingBarRoot.SetActive(false);
                textMain.text = "";
                textCount.text = "";
                StartLoadEngine();
                break;
            case State.Downloding:
                buttonDownload.SetActive(false);
                StartCoroutine(CoUpdateLoading());
                break;
            case State.DownlodFinished:
				OnFished();
                break;
        }
    }

	void OnFished()
	{
		switch (DownloadType)
		{
			case Type.Boot:
				this.Close();
				title.Open();
				break;
			case Type.Default:
				buttonDownload.SetActive(false);
				loadingBarRoot.SetActive(false);
				textMain.text = LanguageSystemText.LocalizeText(SystemText.DownloadFinished);
				textCount.text = "";
				break;
			case Type.ChapterDownload:
				this.Close();
				break;
		}
	}

    //スキップボタン
    public void OnTapSkip()
    {
        this.Close();
        title.Open();
    }

    //ｷｬｯｼｭｸﾘｱして最初のシーンを起動
    public void OnTapReDownload()
    {
        AssetFileManager.DeleteCacheFileAll();
        if (string.IsNullOrEmpty(bootSceneName))
        {
            WrapperUnityVersion.LoadScene(0);
        }
        else
        {
            WrapperUnityVersion.LoadScene(bootSceneName);
        }
    }

    //ローディング中の表示
    IEnumerator CoUpdateLoading()
    {
        int maxCountDownLoad = 0;
        int countDownLoading = 0;
        loadingBarRoot.SetActive(true);
        loadingBar.fillAmount = 0.0f;
        textMain.text = LanguageSystemText.LocalizeText(SystemText.Downloading);
		textCount.text = string.Format(LanguageSystemText.LocalizeText(SystemText.DownloadCount), 0, 1);
        while (Engine.IsWaitBootLoading) yield return 0;

        yield return 0;

        while (!AssetFileManager.IsDownloadEnd())
        {
            yield return 0;
            countDownLoading = AssetFileManager.CountDownloading();
            maxCountDownLoad = Mathf.Max(maxCountDownLoad, countDownLoading);
			int countDownLoaded = maxCountDownLoad - countDownLoading;
			textCount.text = string.Format(LanguageSystemText.LocalizeText(SystemText.DownloadCount), countDownLoaded, maxCountDownLoad);
            if (maxCountDownLoad > 0)
            {
                loadingBar.fillAmount = (1.0f * (maxCountDownLoad - countDownLoading) / maxCountDownLoad);
            }
        }
        loadingBarRoot.gameObject.SetActive(false);
        ChangeState(State.DownlodFinished);
    }

    //ロード開始
    void StartLoadEngine()
    {
        switch (Starter.ScenarioDataLoadType)
        {
            case AdvEngineStarter.LoadType.Local:
                Starter.LoadEngine();
                ChangeState(State.Downloding);
                break;
            case AdvEngineStarter.LoadType.Server:
                StartCoroutine(CoStartFromServer());
                break;
        }
    }


    //サーバーから起動する時にネットワークエラーをチェックする
    IEnumerator CoStartFromServer()
    {
        string url = Starter.UrlScenarioData;
        int scenarioVersion = Starter.ScenarioVersion;

        int version = scenarioVersion;

        bool isRetry = false;
        do
        {
            bool isWaiting = false;
            isRetry = false;
            version = scenarioVersion;
            //ネットワークのチェック(モバイルのみ)
            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:                      //ネットにつながらない
                    if (scenarioVersion < 0)
                    {
                        AssetFile file = AssetFileManager.GetFileCreateIfMissing(url);
                        if (file.CacheVersion >= 0)
                        {
                            version = 0;
                            if (!isAutoCacheFileLoad)
                            {
                                isWaiting = true;
                                string text = LanguageSystemText.LocalizeText(SystemText.WarningNotOnline);
                                List<ButtonEventInfo> buttons = new List<ButtonEventInfo>
                                        {
                                            new ButtonEventInfo(
                                                LanguageSystemText.LocalizeText(SystemText.Yes)
                                                , ()=>isWaiting=false
                                            ),
                                            new ButtonEventInfo(
                                                LanguageSystemText.LocalizeText(SystemText.Retry)
                                                , ()=>{isWaiting=false;isRetry=true;}
                                            ),
                                        };
                                OnOpenDialog.Invoke(text, buttons);
                            }
                        }
                    }
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:    //キャリア
                case NetworkReachability.ReachableViaLocalAreaNetwork:      //Wifi
                default:
                    break;
            }
            while (isWaiting)
            {
                yield return 0;
            }
        } while (isRetry);

        Starter.LoadEngine(version);
        ChangeState(State.Downloding);
    }


	internal void LoadCapter(string capterURL)
	{
		this.DownloadType = Type.ChapterDownload;
		this.Open();
		Engine.LoadChapter(capterURL,0);
	}
}
