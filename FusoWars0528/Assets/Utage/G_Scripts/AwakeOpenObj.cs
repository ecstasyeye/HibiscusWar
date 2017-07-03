using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwakeOpenObj : MonoBehaviour {

    public GameObject[] m_AllOpenObj = null;


    private void Awake()
    {
        if(m_AllOpenObj!=null && m_AllOpenObj.Length >0)
        {
            for(int i = 0; i < m_AllOpenObj.Length;i++)
            {
                m_AllOpenObj[i].SetActive(true);
            }
        }
    }
}
