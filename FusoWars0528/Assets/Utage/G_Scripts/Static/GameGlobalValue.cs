using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGlobalValue 
{
	public static Transform m_MainCanvasMenu = null;
    public static Dictionary<int, CastleData> m_DicCastleData = new Dictionary<int, CastleData>();//城池狀態
	public static Dictionary<int, CharData> m_DicCharData = new Dictionary<int, CharData>();//角色狀態
	public static Dictionary<int, CastleConnectionData> m_DicCastleConnectionData = new Dictionary<int, CastleConnectionData>();//城池狀態

	public static List<int> m_CharList = new List<int>(); // 全部角色
	public static List<int> m_CharPosList = new List<int>(); // 全部角色的位置
	public static List<int> m_CharHpList = new List<int>(); // 全部角色的當前血量
	public static int m_CharCount = 0; //角色數
	public static int m_Money = 0;//資源
	public static int m_Rounds = 0;//回合數
	public static int m_Troops = 0;//兵力
	public static bool m_IsTitle = false;
	public static string m_PathCharHeadIcon = "G_Excel2/Texture/Character/Head";

	public static void Init()
	{
		//m_DicCastleData.Clear();
		//m_DicCharData.Clear();
		//m_DicCastleConnectionData.Clear();
		Dictionary<int, CastleData>.Enumerator e = m_DicCastleData.GetEnumerator();
		while(e.MoveNext())
		{
			if (e.Current.Value.m_index == 15)
			{
				e.Current.Value.m_GetType = 1;
			}
			else
			{
				e.Current.Value.m_GetType = 0;
			}
		}

		Dictionary<int, CharData>.Enumerator e2 = m_DicCharData.GetEnumerator();
		while (e2.MoveNext())
		{

		}

		m_CharList.Clear();
		m_CharPosList.Clear();
		m_CharHpList.Clear();

		m_CharList.Add(2);
		m_CharPosList.Add(15);
		m_CharHpList.Add(100);
		m_CharCount = 1;
		m_Money = 0; 
		m_Rounds = 1;
		m_Troops = 0;
	}
}
