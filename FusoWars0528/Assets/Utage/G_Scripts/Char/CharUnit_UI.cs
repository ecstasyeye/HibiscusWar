using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharUnit_UI : MonoBehaviour {

    public RawImage m_ImageIcon = null;
    public Text m_TextName = null;
    public Text m_HP = null;
    public Text m_See = null;
    public Text m_Bow = null;
    public Text m_Knight = null;
    public Text m_Infantry = null;
    
    private void Start()
    {
        //CharData data = GameGlobalValue.m_DicCharData[1];
        //InitData(data);
    }

    public void InitData(CharData data)
    {
        if(data.m_CharIcon!="")
        {
            //Debug.Log(data.m_CharIcon);
            //Debug.Log(Resources.Load(GameGlobalValue.m_PathCharHeadIcon + "/" + data.m_CharIcon));
            m_ImageIcon.texture = (Texture)Resources.Load(GameGlobalValue.m_PathCharHeadIcon + "/" + data.m_CharIcon);
            m_TextName.text = data.m_CharName;
            m_HP.text = ""+data.m_HP;
            m_See.text = data.m_See;
            m_Bow.text = data.m_Bow;
            m_Knight.text = data.m_Knight;
            m_Infantry.text = data.m_Infantry;
        }
    }
}
