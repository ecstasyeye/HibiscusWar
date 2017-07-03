using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadConfirm : MonoBehaviour {

    public Transform m_YesBtn = null;
    public Transform m_NoBtn = null;
    public Text m_Text = null;
    public UtageUguiSaveLoad m_SaveLoadData = null;
    private UtageUguiSaveLoadItem m_TempItem = null;

    public void SetSaveData( UtageUguiSaveLoadItem item,bool isSave)
    {
        m_TempItem = item;
        gameObject.SetActive(true);
        if(isSave )
        {
            m_Text.text = "是否要存檔";
        }
        else
        {
            m_Text.text = "是否要讀檔";
        }
    }



    public void OnClickConfirm()
    {
        if (m_TempItem != null)
        {
            m_SaveLoadData.OnRealTap(m_TempItem);
            m_TempItem = null;
            gameObject.SetActive(false);
        }
    }

    public void OnClickNo()
    {
        m_TempItem = null;
        gameObject.SetActive(false);
    }

}
