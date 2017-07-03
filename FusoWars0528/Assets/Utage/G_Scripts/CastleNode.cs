using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastleNode : MonoBehaviour {

    public Text m_NameUI = null;
    public Text m_Surr = null;
    public Text m_Reso = null;
	public Text m_Content = null;

	public void InitData(CastleData data)
    {
        m_NameUI.text = data.m_name;
        m_Surr.text = data.m_Surr;
        m_Reso.text = data.m_Reso;
		m_Content.text = data.m_Content;

	}

    public void OnCliockReconna()
    {
        gameObject.SetActive(false);
    }

    public void OnCliockBack()
    {
        gameObject.SetActive(false);
    }

    public void OnCliockMove()
    {
        gameObject.SetActive(false);
    }
}
