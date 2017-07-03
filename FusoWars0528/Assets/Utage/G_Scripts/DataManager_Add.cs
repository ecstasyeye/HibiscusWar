using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DataManager {




    private void SetCastleDataToDicData(string [] s_array)
    { 
        GameGlobalValue.m_DicCastleData.Clear();

        CastleData data = new CastleData();
        data.m_index = int.Parse(s_array[0]);
        data.m_name = s_array[1];
        data.m_Surr = s_array[2];
        data.m_Reso = s_array[3];
        GameGlobalValue.m_DicCastleData.Add(data.m_index, data);
    }
}
