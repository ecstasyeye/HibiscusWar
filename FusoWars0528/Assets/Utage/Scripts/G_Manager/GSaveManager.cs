using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using GV = GameGlobalValue;
namespace Utage
{
    public class GSaveManager : MonoBehaviour, IAdvCustomSaveDataIO
    {
        public static GSaveManager s_Main = null;
        public GCustomSaveData m_CSaveData = null;

        const int Version = 0;//版本

        private void Awake()
        {
            s_Main = this;
        }

        public string SaveKey
        {
            get
            {
                return "GCustomSaveData";
            }
        }

        public void OnClear()
        {
            //Debug.LogError("OnClear");
            if (m_CSaveData != null)
            {
                m_CSaveData.m_Test1 = 0;
                //m_Money = 0;
                //m_Rounds = 0;
            }
        }

        public void OnRead(BinaryReader reader)
        {
			GV.m_IsTitle = false;
			ClearData();

			int version = reader.ReadInt32();
            if (version == Version)
            {
                int count = reader.ReadInt32();
				GV.m_Money = reader.ReadInt32();
				GV.m_Rounds = reader.ReadInt32();
				GV.m_Troops = reader.ReadInt32();
				GV.m_CharCount = reader.ReadInt32();
				GV.m_CharList.Clear();
                for (int i = 0; i< GV.m_CharCount; i++)
                {
					GV.m_CharList.Add(reader.ReadInt32());
					GV.m_CharPosList.Add(reader.ReadInt32());
					GV.m_CharHpList.Add(reader.ReadInt32());
                }
                for (int i = 0; i < 16; i++)
                {
					CastleData data = GV.m_DicCastleData[i + 1];
					data.m_GetType = reader.ReadInt32();
                }
                //Debug.LogWarning("OnRead"+count);
                m_CSaveData.m_Test1 = count;
                //Debug.LogWarning("m_Rounds" + m_Rounds);
            }
        }

        public void OnWrite(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(m_CSaveData.m_Test1);
            writer.Write(GV.m_Money);
            writer.Write(GV.m_Rounds);
            writer.Write(GV.m_Troops);
            writer.Write(GV.m_CharCount);
            for(int i = 0; i< GV.m_CharCount; i++)
            {
                writer.Write(GV.m_CharList[i]);
                writer.Write(GV.m_CharPosList[i]);
                writer.Write(GV.m_CharHpList[i]);
            }
            for (int i = 0; i < 16; i++)
            {
				CastleData data = GV.m_DicCastleData[i + 1];
				writer.Write(data.m_GetType);
            }
            //Debug.LogWarning("OnWrite"+m_CSaveData.m_Test1);
            //Debug.LogWarning("m_Rounds" + m_Rounds);
        }

		private void ClearData()
		{
			GV.m_CharList.Clear();
			GV.m_CharPosList.Clear();
			GV.m_CharHpList.Clear();

		}

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    m_CSaveData.m_Test1 = 999;
            //    //if (AdvSaveManager._instance != null)
            //    //{
            //    //    AdvSaveData asd = AdvSaveManager._instance.CurrentAutoSaveData;
            //    //    asd.CustomBuffer.m_test1 = 999;
            //    //    asd.CustomBuffer.m_test2 = 1001;
            //    //}
            //}
            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    m_CSaveData.m_Test1 = 555;
            //}
        }

        public void SetSomething(AdvCommandSendMessageByName command)
        {
			//Debug.Log(command.ParseCellOptional<string>(AdvColumnName.Arg3, "arg3"));
			GV.m_Money++;
        }

        public void AddMoney(AdvCommandSendMessageByName command)
        {
            int money = int.Parse(command.ParseCellOptional<string>(AdvColumnName.Arg3, "arg3"));
			GV.m_Money += money;
        }

        public void SetOverRound(AdvCommandSendMessageByName command)
        {
			GV.m_Rounds++;
        }
    }
}
