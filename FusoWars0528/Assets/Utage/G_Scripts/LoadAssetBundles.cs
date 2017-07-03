using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadAssetBundles : MonoBehaviour {

    public CastleManager m_CM = null;

    private void Awake()
    { 
        AssetBundle ab = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "castle"));
        if(ab==null)
        {
            Debug.LogError("ab==null");
            return;
        }

        string[] totalName = ab.GetAllAssetNames();
        for (int i = 0; i< totalName.Length; i++)
        {
            //Debug.LogError(totalName[i]);
            CastleDataTotal data = (CastleDataTotal)ab.LoadAsset(totalName[i]);
            m_CM.m_TotalCastleObject = data;
			SetDataToStatic(data.m_TotalData);
			//for(int k = 0; k<cdt.m_totalCastleData.Count; k++)
			//{
			//    Debug.LogError(cdt.m_totalCastleData[k]);
			//}
		}
        LoadCharData();
		LoadCastleConnection();

	}

    private void LoadCharData()
    {
        AssetBundle ab = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "char"));
        if (ab == null)
        {
            Debug.LogError("ab==null");
            return;
        }

        string[] totalName = ab.GetAllAssetNames();
        for (int i = 0; i < totalName.Length; i++)
        {
            CharDataTotal cdt = (CharDataTotal)ab.LoadAsset(totalName[i]);
            List<CharData> l = cdt.m_totalCastleData;
            for (int j = 0; j< l.Count; j++)
            {
                CharData data = l[j];
                GameGlobalValue.m_DicCharData[data.m_index] = data;
                //Debug.Log(data.name);
            }
        }
    }

	private void LoadCastleConnection()
	{
		AssetBundle ab = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "castle_connect"));
		if (ab == null)
		{
			Debug.LogError("ab==null");
			return;
		}

		string[] totalName = ab.GetAllAssetNames();
		CastleConnectionTotal data = (CastleConnectionTotal)ab.LoadAsset(totalName[0]);
		m_CM.m_TotalCastleConnect = data;
		SetDataToStatic(data.m_TotalData);
	}

	private void SetDataToStatic(List<CastleData> data)
	{
		for (int i = 0; i < data.Count; i++)
		{
			GameGlobalValue.m_DicCastleData[data[i].m_index] = data[i];
		}
	}

	private void SetDataToStatic(List<CastleConnectionData> data)
	{
		for (int i = 0; i < data.Count; i++)
		{
			GameGlobalValue.m_DicCastleConnectionData[data[i].m_index] = data[i];
		}
	}
}
