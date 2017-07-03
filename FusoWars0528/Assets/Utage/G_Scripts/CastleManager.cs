using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleManager : MonoBehaviour {

    public CastleDataTotal m_TotalCastleObject = null;
	public CastleConnectionTotal m_TotalCastleConnect = null;
	public CastleNode m_CastleNode = null;
    public CastleUnit[] m_TotalCastle = null;

	private Dictionary<int, int> m_CastleType = new Dictionary<int, int>();//目前城池的狀態

    private void Awake()
    {
        m_TotalCastle = transform.GetComponentsInChildren<CastleUnit>();
    }

    private void Start()
    {
		RefreshData();
    }

	public void RefreshData()
	{
		for (int i = 0; i < m_TotalCastle.Length; i++)
		{
			m_TotalCastle[i].SetInit(m_TotalCastleObject.m_TotalData[i], m_CastleNode);
			CastleData data = m_TotalCastleObject.m_TotalData[i];
			m_CastleType[data.m_index] = data.m_GetType;
			if(data.m_GetType == 1)
			{
				List<int> list = m_TotalCastleConnect.m_TotalData[i].m_Connection;
				for (int j = 0;j< list.Count; j++)
				{
					if(list[j]==0)
					{
						continue;
					}
					//編號轉換
					m_TotalCastle[(list[j]-1)].SetCastleType(true);
				}
				m_TotalCastle[i].SetCastleType(true);
			}
		}
	}

}
