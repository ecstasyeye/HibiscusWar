using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelayHide : MonoBehaviour {

    public Image m_Image = null;
    public Text m_Text = null;

    private void Update()
    {
        if(m_Image!=null)
        {
            m_Image.color -= new Color(0, 0, 0, 0.5f*Time.deltaTime);
        }
        if (m_Text != null)
        {
            m_Text.color -= new Color(0, 0, 0, 0.5f * Time.deltaTime);
        }
    }

    public void ShowMenu()
    {
        if (m_Image != null)
        {
            m_Image.color = Color.white;
        }
        if (m_Text != null)
        {
            m_Text.color = Color.white;
        }
    }
}
