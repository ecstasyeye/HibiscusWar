using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class DataManager : MonoBehaviour {

    private void Awake()
    {
        SetData();
    }

    private void SetData()
    {
        //TextAsset al = (TextAsset)Resources.Load("ItemData");
        //AssetBundle al = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "GameData\\CastleData"));
        //string[] s_queue_array = al.ToString().Split('\n');
        //string[] s_queue_split = null;
        //for (int i = 2; i < s_queue_array.Length; i++)
        //{
        //    string[] queue = s_queue_array[i].Split(',');
        //    queue[queue.Length - 1] = queue[queue.Length - 1].TrimEnd('\r');
        //    s_queue_split = queue;
        //    SetCastleDataToDicData(s_queue_split);
        //}
    }
    
}
