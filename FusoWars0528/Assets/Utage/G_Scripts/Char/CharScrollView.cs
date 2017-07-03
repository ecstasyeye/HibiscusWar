using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharScrollView : MonoBehaviour {

    public ScrollRect m_ScrollView = null;
    public GridLayoutGroup m_Grid = null;
    public CharUnit_UI m_CloneObj = null;

    public List<CharUnit_UI> m_CharUnitList = new List<CharUnit_UI>();

    private void Awake()
    {
        if(GameGlobalValue.m_DicCharData.Count>0)
        {
            int num = 0;
            var e = GameGlobalValue.m_DicCharData.GetEnumerator();
            while (e.MoveNext())
            {
                if (m_CharUnitList.Count <= num)
                {
                    CharUnit_UI unit = Instantiate(m_CloneObj, m_Grid.transform);
                    unit.InitData(e.Current.Value);
                    m_CharUnitList.Add(unit);
                }
                else
                {
                    m_CharUnitList[num].InitData(e.Current.Value);
                }
                num++;
            }
        }
    }




}
