using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utage;

public class FusoManager : MonoBehaviour
{
    public UtageUguiTitle m_title = null;
    public UtageUguiMainGame m_MainGame;
    public GSaveManager m_GSaveData = null;

    public UtageUguiGallery m_UtageGallery = null;

    public DelayHide m_DelayHide = null;
    public Transform m_MainMenuCanvas = null;
    public Transform m_MainMenu = null;
    public Transform m_WarMenu = null;
    public Transform m_NoUseNode = null;
    public Transform m_NodeSetting = null;

    public Transform m_BtnStart = null;
    //public Transform m_BtnGallery = null;
    //public Transform m_BtnSetting = null;

    public Image m_MainTexture = null;

    public Text m_TextMoney = null;
    public Text m_TextRound = null;
    public bool m_IsGameStart = false;
    private bool m_IsDelayStart = true;

	public WarNodeManager m_WarNode = null;

	private void Awake()
	{
		GameGlobalValue.m_MainCanvasMenu = m_MainMenuCanvas;
	}

	public void Update()
    {
        if (m_IsDelayStart)
        { 
            float math_f = 0.35f * Time.deltaTime;
            m_MainTexture.color += new Color(math_f, math_f, math_f);
        }

        m_TextMoney.text = GameGlobalValue.m_Money.ToString();
        m_TextRound.text = GameGlobalValue.m_Rounds.ToString();
        if(m_IsGameStart==false && Time.time>3)
        {
            LoadTitleOver();
        }
		else if (m_IsGameStart == false &&Time.time > 1.95f)
		{
			m_BtnStart.gameObject.SetActive(true);
		}
		//if (Input.GetKeyDown(KeyCode.A))
		//{
		//    m_title.Close();
		//    mainGame.OpenStartLabel("Text2");
		//}
		//if(Input.GetKeyDown(KeyCode.A))
		//{
		//	RoundManager.Main.NextRound();
		//}
	}

    private void LoadTitleOver()
    {
        m_IsDelayStart = false;
        m_IsGameStart = true;
        
        //m_BtnGallery.gameObject.SetActive(true);
        //m_BtnSetting.gameObject.SetActive(true);

    }

    public void OnClickStart()
    {
        if(m_IsGameStart==false)
        {
            return;
        }
		GameGlobalValue.Init();

		m_MainMenu.gameObject.SetActive(false);
        m_WarMenu.gameObject.SetActive(true);
        //m_title.Close();
        //mainGame.OpenStartLabel("Start");
    }

    public void OnClickSetting()
    {
        if (m_NodeSetting.gameObject.activeSelf == false)
        {
            m_NodeSetting.gameObject.SetActive(true);
        }
        else
        {
            m_NodeSetting.gameObject.SetActive(false);
        }
    }

    public void OnClickOther()
    {
        m_DelayHide.ShowMenu();
        m_NoUseNode.gameObject.SetActive(true);
    }

    public void OnClickOver()
    {
        m_MainMenuCanvas.gameObject.SetActive(false);
        //m_GSaveData.m_Rounds += 1;
        m_title.Close();
        m_MainGame.OpenStartLabel("Start");
    }

    public void OnClickMapBack()
    {
        m_MainMenu.gameObject.SetActive(true);
        m_WarMenu.gameObject.SetActive(false);
    }

    public void OnClickGallery()
    {
        m_MainMenu.gameObject.SetActive(false);
        m_UtageGallery.gameObject.SetActive(true);
    }

    public void OnClickGalleryBack()
    {
        m_MainMenu.gameObject.SetActive(true);
        m_UtageGallery.gameObject.SetActive(false);
    }

	public void OnClickLoad()
	{
		m_MainGame.gameObject.SetActive(true);
		m_MainMenuCanvas.gameObject.SetActive(false);
		//m_WarNode.gameObject.SetActive(false);
		m_title.Close();
		GameGlobalValue.m_IsTitle = true;
		//m_MainGame.OpenStartLabel("SaveLoad");
		m_MainGame.OnTapLoad();

	}

	public void OnClickEnd()
	{
		Application.Quit();
	}

	public void OnStart()
	{
		m_MainMenu.gameObject.SetActive(false);
		m_WarMenu.gameObject.SetActive(true);
	}


	public void ShowMenu(AdvCommandSendMessageByName command)
    {
        m_MainMenuCanvas.gameObject.SetActive(true);
		m_WarNode.RefreshData();

	}
}
