using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SetObjectRe : MonoBehaviour {

    public static bool m_isTest = true;

    private void Awake()
    {
        Test2();
        SetSome();
    }
#if UNITY_EDITOR
    [MenuItem("test_Tool/OpenElse")]
    private static void OpenElse()
    {
        m_isTest = false;
    }

    [MenuItem("test_Tool/OpenElse2")]
    private static void OpenElse2()
    {
        SetOutSome();
    }
#endif
    private static void SetSome()
    {
#if UNITY_EDITOR
        if (m_isTest)
        {
            EditorApplication.update += Test;
            EditorApplication.hierarchyWindowItemOnGUI += SetTest;
        }
#endif
    }

    public static void SetOutSome()
    {
#if UNITY_EDITOR
        if (m_isTest)
        {
            EditorApplication.update -= Test;
            EditorApplication.hierarchyWindowItemOnGUI -= SetTest;
        }
#endif
    }

    private static void Test()
    {
    }
#if UNITY_EDITOR
    public static void SetTest( int id , Rect r)
    {
        GameObject go = (GameObject)UnityEditor.EditorUtility.InstanceIDToObject(id);
        if (go == null)
            return;
        go.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
    }
#endif

    private static void Test2()
    {
#if Gary
        m_isTest = false;
#endif
    }
}
