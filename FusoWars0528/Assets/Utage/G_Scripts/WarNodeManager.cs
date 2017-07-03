using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarNodeManager : MonoBehaviour {

	public CastleManager m_CastleManager = null;

	public void RefreshData()
	{
		m_CastleManager.RefreshData();
	}

}
