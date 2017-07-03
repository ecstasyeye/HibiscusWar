using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSettingBtn : MonoBehaviour {

    private bool m_IsOpenMenu = false;
    public Transform m_SaveLoad = null;
    public UtageUguiMainGame m_MainGame = null;
    public UtageUguiTitle m_title = null;
    public Transform m_WarNode = null;
    public Transform m_MainMenuCanvas = null;//Canvas-Menu(最上層)
    public Transform m_CharNode = null;
	public Transform m_NodeSetting = null;

	public void OnClickSetting()
    {
        if(m_IsOpenMenu == false)
        {
            m_SaveLoad.gameObject.SetActive(true);
            m_IsOpenMenu = true;
        }
        else
        {
            OpenSetting();
            m_IsOpenMenu = true;
        }
    }

    public void OnClickSave()
    {
       
        m_MainGame.gameObject.SetActive(true);
        m_MainMenuCanvas.gameObject.SetActive(false);
        //m_WarNode.gameObject.SetActive(false);
        m_title.Close();
        m_MainGame.OpenStartLabel("SaveLoad");
        m_MainGame.OnTapSave();

    }

    public void OnClickLoad()
    {
        m_MainGame.gameObject.SetActive(true);
        m_MainMenuCanvas.gameObject.SetActive(false);
        //m_WarNode.gameObject.SetActive(false);
        m_title.Close();
        //m_MainGame.OpenStartLabel("SaveLoad");
        m_MainGame.OnTapLoad();

    }

    public void OpenSetting()
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

    public void OnClickChar()
    {
        m_CharNode.gameObject.SetActive(true);
    }


    public void CloseSetting()
    {
        m_SaveLoad.gameObject.SetActive(false);
        m_IsOpenMenu = false;
		m_NodeSetting.gameObject.SetActive(false);
	}

	public void CloseCharNode()
	{
		m_CharNode.gameObject.SetActive(false);
	}
}
