//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace Utage
{

	/// <summary>
	/// ページ内のテキストデータ
	/// </summary>
	public class AdvScenarioTextDataInPage
	{
		public AdvCommand Command { get; set; }	//ローカライズされたテキストを取得するためのもの。エンティティ処理の場合入れ替わるので注意

		public bool IsPageEnd { get; private set; }
		public bool IsNextBr { get; private set; }
		public bool IsCharacterShowOnly { get; private set; }
		public AdvPageControllerType PageCtrlType { get; private set; }
		public AdvCharacterInfo CharacterInfo  { get; set; }
		public AssetFile VoiceFile  { get; set; }
		public bool IsEmptyText { get; private set; }
		public float AutoBrTime { get; private set; }

		public AdvScenarioTextDataInPage(AdvCommand command)
		{
			Command = command;
			if (command.RowData == null)
			{
				//暗黙的だけど、選択肢の後の改ページ待ちのために
				IsCharacterShowOnly = false;
				PageCtrlType = AdvPageControllerType.None;
				IsPageEnd = true;
				IsEmptyText = true;
				return;
			}
			else
			{
				//暗黙的だけどキャラ表示のみの場合
				IsCharacterShowOnly = command.IsEmptyCell(AdvColumnName.Text)  && command.IsEmptyCell(AdvColumnName.PageCtrl);
				if (IsCharacterShowOnly)
				{
					IsEmptyText = true;
					IsPageEnd = false;
				}
				else
				{
					IsEmptyText = false;
					if (command.IsEmptyCell(AdvColumnName.PageCtrl))
					{
						this.PageCtrlType = AdvPageControllerType.None;
					}
					else
					{
						float autoBrTime;
						string pageCtrl = command.ParseCell<string>(AdvColumnName.PageCtrl);
						if (float.TryParse(pageCtrl, out autoBrTime))
						{
							AutoBrTime = autoBrTime;
						}
						else
						{
							this.PageCtrlType = command.ParseCellOptional<AdvPageControllerType>(AdvColumnName.PageCtrl, AdvPageControllerType.None);
						}
					}
					IsPageEnd = AdvPageController.IsPageEndType(PageCtrlType);
					IsNextBr = AdvPageController.IsBrType(PageCtrlType);

					//エディター用のチェック
					if (AdvCommand.IsEditorErrorCheck)
					{
						TextData textData = new TextData(command.ParseCellLocalizedText());
						if (!string.IsNullOrEmpty(textData.ErrorMsg))
						{
							Debug.LogError(command.ToErrorString(textData.ErrorMsg));
						}
					}
				}
			}
		}
	}
}