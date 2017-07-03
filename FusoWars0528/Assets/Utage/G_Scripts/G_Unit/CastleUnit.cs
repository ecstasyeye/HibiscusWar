using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CastleUnit : MonoBehaviour {

    public int m_Index = 0;
    public string m_CastleName = "";
    public CastleNode m_CastleNode = null;
    public StrongHold[] m_StrongHold1;
    public CastleData m_CastleData = null;
	private ButtonAni m_ButtonAni = null;
    private void Awake()
    {
        Button.ButtonClickedEvent ev = new Button.ButtonClickedEvent();
        UnityAction ac = new UnityAction(OnClickCastleUnit);
        ev.AddListener(ac);
		m_ButtonAni = gameObject.GetComponent<ButtonAni>();
		gameObject.GetComponent<Button>().onClick = ev;
    }

    public void OnClickCastleUnit()
    {
		if(m_ButtonAni.m_Need==false)
		{
			return;
		}
        m_CastleNode.InitData(m_CastleData);
        m_CastleNode.gameObject.SetActive(true);
        //Debug.Log("m_Index" + m_Index);
        //Debug.Log("m_CastleName" + m_CastleName);
    }

    public void SetInit(CastleData data, CastleNode node)
    {
        m_CastleData = data;
        m_Index = data.m_index;
        m_CastleName = data.m_name;
        m_CastleNode = node;
		m_ButtonAni.m_Need = false;
	}

	public void SetCastleType(bool type)
	{
		m_ButtonAni.m_Need = type;
	}
}
