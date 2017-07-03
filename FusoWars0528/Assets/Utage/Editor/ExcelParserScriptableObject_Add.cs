using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utage;

public partial class ExcelParserScriptableObject : MonoBehaviour {

    private static void AddCastle(List<StringGridRow> list)
    {
		string assetName = s_TotalDataName[0];

		CastleDataTotal BData = ScriptableObject.CreateInstance<CastleDataTotal>();
        for (int i = 1; i < list.Count; i++)
        {
            string[] s_array = list[i].Strings;
            CastleData Data = ScriptableObject.CreateInstance<CastleData>();
			int index = 0;
			if (int.TryParse(s_array[0], out index) == false)
			{
				continue ;
			}
			Data.m_index = index;
			Data.m_name = s_array[1];
            Data.m_Surr = s_array[2];
			Data.m_Reso = s_array[3];
			Data.m_GetType = ReturnValue(s_array[4]);
			Data.m_Content = s_array[5];


			AssetDatabase.DeleteAsset("Assets/G_ScriptableUnit/" + assetName + i + ".asset");
            AssetDatabase.CreateAsset(Data, AssetDatabase.GenerateUniqueAssetPath("Assets/G_ScriptableUnit/" + assetName + i + ".asset"));

            BData.m_TotalData.Add(Data);
        }
        AssetDatabase.DeleteAsset("Assets/G_ScriptableObject/" + assetName + ".asset");
        AssetDatabase.CreateAsset(BData, AssetDatabase.GenerateUniqueAssetPath("Assets/G_ScriptableObject/" + assetName + ".asset"));
        AssetDatabase.SaveAssets();
    }

    private static void AddChar(List<StringGridRow> list)
    {
        string assetName = s_TotalDataName[1];
		CharDataTotal BData = ScriptableObject.CreateInstance<CharDataTotal>();
        for (int i = 1; i < list.Count; i++)
        {
            string[] s_array = list[i].Strings;
            CharData Data = ScriptableObject.CreateInstance<CharData>();

            Data.m_index = int.Parse(s_array[0]);
            Data.m_CharIcon = s_array[1];
            Data.m_CharName = s_array[2];
            Data.m_Content = s_array[3];
            Data.m_Skill = s_array[4];
            Data.m_Force = int.Parse(s_array[5]);
            Data.m_See = s_array[6];
            //Data.atk = s_array[7];
            Data.m_HP = int.Parse(s_array[8]);
            //Data.m_NowHP = int.Parse(s_array[9]);
            Data.m_Bow = s_array[10];
            Data.m_Knight = s_array[11];
            Data.m_Infantry = s_array[12];
            Data.m_Pos = s_array[13];

            AssetDatabase.DeleteAsset("Assets/G_ScriptableUnit/" + assetName + i + ".asset");
            AssetDatabase.CreateAsset(Data, AssetDatabase.GenerateUniqueAssetPath("Assets/G_ScriptableUnit/" + assetName + i + ".asset"));

            BData.m_totalCastleData.Add(Data);
        }
        AssetDatabase.DeleteAsset("Assets/G_ScriptableObject/" + assetName + ".asset");
        AssetDatabase.CreateAsset(BData, AssetDatabase.GenerateUniqueAssetPath("Assets/G_ScriptableObject/" + assetName + ".asset"));
        AssetDatabase.SaveAssets();
    }

	private static void AddCastleConnection(List<StringGridRow> list)
	{
		string assetName = s_TotalDataName[2];
		CastleConnectionTotal BData = ScriptableObject.CreateInstance<CastleConnectionTotal>();
		for (int i = 1; i < list.Count; i++)
		{
			string[] s_array = list[i].Strings;
			CastleConnectionData Data = ScriptableObject.CreateInstance<CastleConnectionData>();
			int index = 0;
			if (int.TryParse(s_array[0], out index) == false)
			{
				continue;
			}
			Data.m_index = index;
			Data.m_Connection.Add(ReturnValue(s_array[1]));
			Data.m_Connection.Add(ReturnValue(s_array[2]));
			Data.m_Connection.Add(ReturnValue(s_array[3]));
			Data.m_Connection.Add(ReturnValue(s_array[4]));


			AssetDatabase.DeleteAsset("Assets/G_ScriptableUnit/" + assetName + i + ".asset");
			AssetDatabase.CreateAsset(Data, AssetDatabase.GenerateUniqueAssetPath("Assets/G_ScriptableUnit/" + assetName + i + ".asset"));

			BData.m_TotalData.Add(Data);
		}
		AssetDatabase.DeleteAsset("Assets/G_ScriptableObject/" + assetName + ".asset");
		AssetDatabase.CreateAsset(BData, AssetDatabase.GenerateUniqueAssetPath("Assets/G_ScriptableObject/" + assetName + ".asset"));
		AssetDatabase.SaveAssets();
	}

	private static int ReturnValue(string data)
	{
		return int.Parse(data);
	}

	private static string ReturnValues(string data)
	{
		return data;
	}
}
