using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharData : ScriptableObject
{
    public int m_index = 0;
    public string m_CharIcon = "";  //圖片 名稱
    public string m_CharName = "";
    public float m_HP = 0;
    public string m_See = "";//偵查
    public string m_Bow = "";   //弓兵
    public string m_Knight = "";//騎兵
    public string m_Infantry = "";  //步兵
    public string m_HP_Now = "";//現在血量
    public string m_Pos = ""; //現在在哪一個位置
    public string m_Content = "";//角色介紹
    public string m_Skill = "";//
    public int m_Force =0;//兵力
}
