using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GEditorManager : MonoBehaviour {

    static string s_SaveString = "SaveData01";

    [MenuItem("GaryTool/CreateSaveData")]
    static void CreateCreateSaveData()
    {
        GCustomSaveData saveData = ScriptableObject.CreateInstance<GCustomSaveData>();
        AssetDatabase.DeleteAsset("Assets/ScriptableObject/" + s_SaveString + ".asset");
        AssetDatabase.CreateAsset(saveData, AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObject/" + s_SaveString + ".asset"));
        AssetDatabase.SaveAssets();
    }
}
