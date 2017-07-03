using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class RoundManager : MonoBehaviour
{

	public static RoundManager Main = null;

	public int m_RoundCount = 1;
	private Round_Type m_RoundType = Round_Type.Start;
	private bool m_IsGameStart = false;
	public enum Round_Type : int
	{
		Start,			//回合開始
		Map,			//地圖階段
		Plot,			//劇情
		Fighting,		//戰鬥
		FightVictory,	//戰鬥勝利
		FightFailed,	//戰鬥失敗
		RoundOver		//回合結束
	}


	private void Awake()
	{
		Main = this;
	}

	/// <summary>
	/// 切換到下一個回合狀態
	/// </summary>
	public void NextRound()
	{
		if (m_IsGameStart == false)
		{
			m_IsGameStart = true;
		}
		else
		{
			m_RoundType = m_RoundType + 1;
		}

		if(m_RoundType > Round_Type.RoundOver)
		{
			m_RoundCount++;
			m_RoundType = Round_Type.Start;
		}

		RoundEvent();
	}

	/// <summary>
	/// 處理回合內事件分派
	/// </summary>
	private void RoundEvent()
	{
		//Debug.Log(m_RoundType);
		switch (m_RoundType)
		{
			case Round_Type.Start:
				OnRoundStart();
				break;
			case Round_Type.Map:
				OnRoundMap();
				break;
			case Round_Type.Plot:
				OnRoundPlot();
				break;
			case Round_Type.Fighting:
				OnRoundFighting();
				break;
			case Round_Type.FightVictory:
				OnRoundFightVictory();
				break;
			case Round_Type.FightFailed:
				OnRoundFightFailed();
				break;
			case Round_Type.RoundOver:
				OnRoundRoundOver();
				break;
			default:
				break;
		}
	}

	private void OnRoundStart()
	{

	}

	private void OnRoundMap()
	{

	}

	private void OnRoundPlot()
	{

	}

	private void OnRoundFighting()
	{

	}

	private void OnRoundFightVictory()
	{

	}

	private void OnRoundFightFailed()
	{

	}

	private void OnRoundRoundOver()
	{

	}
	
}
