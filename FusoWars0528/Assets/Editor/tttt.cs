using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class tttt : MonoBehaviour {

    [MenuItem("G_Helper/CreateAssetBunldesMain")]
    static void CreateAssetBunldesMain()
    {
        Caching.CleanCache();
        Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        List<AssetBundleBuild> list = new List<AssetBundleBuild>();
        for (int i = 0; i < selection.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(selection[i]);
            AssetBundleBuild b = new AssetBundleBuild();
            b.assetBundleName = selection[i].name;
            b.assetNames = new string[] { path };
            list.Add(b);
        }
        string outpath = Application.dataPath + "/StreamingAssets";
		AssetDatabase.DeleteAsset(outpath);
		BuildPipeline.BuildAssetBundles(outpath, list.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
        // 打開保存面板，獲得用戶選擇的路徑
        //string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "assetbundle");
        //if (path.Length != 0)
        //{
        //    // 選擇的要保存的對象
        //    Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        //    Application.
        //    //打包
        //    List<AssetBundleBuild> list = new List<AssetBundleBuild>();
        //    AssetBundleBuild b = new AssetBundleBuild();
        //    b.assetBundleName = "aaaaa";
        //    b.assetNames = new string[] { };

        //}
    }

    [MenuItem("G_Helper/CreateAssetBunldesALL")]
    static void CreateAssetBunldesALLForAndroid()
    {
        Caching.CleanCache();
        string path =  Application.dataPath+ "/AssetBundles";
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
    }
}
