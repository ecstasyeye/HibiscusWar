//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------


namespace Utage
{
#if false
	/// <summary>
	/// コマンド：   エンティティ( パラメーターで、コマンド引数を動的に変える) をするコマンドのためのダミー
	/// </summary>
	internal class AdvCommandDummyEntity : AdvCommand
	{
		public static bool IsEntityData(StringGridRow row)
		{
//			foreach (var str in row.Strings)
//			{
//				if (str.Length <= 1) continue;
//				if (str[0] == '&') return true;
//			}
			return false;
		}

		public AdvCommandDummyEntity(string id, StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			this.id = id;
			this.dataManager = dataManager;

			IdErrorCheck(id);
		}

		void IdErrorCheck(string id)
		{

			//IfやJump系のコマンドだったらエラー

			//
		}

		public AdvCommand CreateCommand(AdvEngine engine)
		{
			StringGridRow row = RowData.Clone( ()=>RowData.Grid );
			for (int i = 0; i < row.Strings.Length; ++i )
			{
				if (row.Strings[i].Length <= 1) continue;
				if (row.Strings[i][0] == '&')
				{
					string entity = engine.Param.GetParameter(row.Strings[i].Substring(1)).ToString();
					row.Strings[i] = entity;
				}
			}

			return AdvCommandParser.CreateCommand(id, RowData, dataManager);
		}

		public override void DoCommand(AdvEngine engine)
		{
		}


		string id;
		AdvSettingDataManager dataManager;
	}
#endif
}