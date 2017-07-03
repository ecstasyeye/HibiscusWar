using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Utage;
using System.IO;

public partial class ExcelParserScriptableObject : MonoBehaviour {

	private static string[] s_TotalDataName = new string[]
	{
		"castle",
		"char",
		"castle_connect",
	};

	[MenuItem("G_Excel/AssetLoader", false, 0)]
	static void AssetLoader()
	{
		Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/Utage/G_Scripts/LoadAssetBundles.cs");
		Selection.activeObject = obj;
	}

	[MenuItem("G_Excel/Excel資料夾",false,0)]
	static void PingExcel()
	{
		string[] guids = AssetDatabase.FindAssets("G_Excel t:Script");
		string path = AssetDatabase.GUIDToAssetPath(guids[0]);
		Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
		Selection.activeObject = obj;
	}

	[MenuItem("G_Excel/劇情Excel", false, 0)]
	static void PingExcelHome()
	{
		Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/G_Excel2/G_Excel2.xls");
		Selection.activeObject = obj;
	}

	[MenuItem("G_Excel/SO", false, 0)]
	static void ScriptableO()
	{
		Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/G_ScriptableObject");
		Selection.activeObject = obj;
	}

	[MenuItem("G_Excel/ExcelP", false, 0)]
	static void ExcelP()
	{
		Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/Utage/Editor/ExcelParserScriptableObject.cs");
		Selection.activeObject = obj;
	}

	[MenuItem("G_Excel/G_Scripts", false, 0)]
	static void G_Scripts()
	{
		Object obj = AssetDatabase.LoadAssetAtPath<Object>("Assets/Utage/G_Scripts");
		Selection.activeObject = obj;
	}

	[MenuItem("G_Excel/製造城池資料", false, 101)]
    static void MakeCastleData()
    { 
        StringGridDictionary SGD = ExcelParser.Read(Application.dataPath + "//G_Excel//CastleData.xlsx");
        MakeData(SGD, 0);
		Object obj = AssetDatabase.LoadAssetAtPath<Object>(GetSO_Path(0));
		Selection.activeObject = obj;
	}

    [MenuItem("G_Excel/製造角色資料", false, 101)]
    static void MakeCharData()
    {
        StringGridDictionary SGD = ExcelParser.Read(Application.dataPath + "//G_Excel//CharData.xlsx");
        MakeData(SGD, 1);
		//Debug.LogError(SGD);
		Object obj = AssetDatabase.LoadAssetAtPath<Object>(GetSO_Path(1));
		Selection.activeObject = obj;
	}

	[MenuItem("G_Excel/製造城池連接資料", false, 101)]
	static void MakeCastleConnectionData()
	{
		StringGridDictionary SGD = ExcelParser.Read(Application.dataPath + "//G_Excel//CastleConnection.xlsx");
		MakeData(SGD, 2);
		Object obj = AssetDatabase.LoadAssetAtPath<Object>(GetSO_Path(2));
		Selection.activeObject = obj;
	}

	private static void MakeData(StringGridDictionary SGD,int index )
    {
        Dictionary<string, StringGridDictionaryKeyValue>.ValueCollection vc = SGD.Values;
        //Debug.LogError(vc);
        Dictionary<string, StringGridDictionaryKeyValue>.ValueCollection.Enumerator en = vc.GetEnumerator();
        while (en.MoveNext())
        {
            StringGridDictionaryKeyValue kv = en.Current;
            StringGrid sg = kv.Grid;
            List<StringGridRow> list = sg.Rows;
            switch (index)
            {
                case 0:
                    AddCastle(list);
                    break;
                case 1:
                    AddChar(list);
                    break;
				case 2:
					AddCastleConnection(list);
					break;
				default:
                    break;
            }
        }
    }

	private static string GetSO_Path(int index)
	{
		string path = "Assets/G_ScriptableObject/" + s_TotalDataName[index] + ".asset";
		return path;
	}
}
