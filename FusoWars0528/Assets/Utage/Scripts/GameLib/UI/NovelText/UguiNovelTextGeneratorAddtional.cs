//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace Utage
{
	//ノベル用テキストの付加的な描画情報（ルビやアンダーラインなど）
	public class UguiNovelTextGeneratorAddtional
	{
		//表示の最大幅
		public List<UguiNovelTextGeneratorAddtionalRuby> RubyList
		{
			get { return rubyList; }
		}
		List<UguiNovelTextGeneratorAddtionalRuby> rubyList = new List<UguiNovelTextGeneratorAddtionalRuby>();

		//線（アンダーラインや取り消し線）
		public List<UguiNovelTextGeneratorAddtionalLine> LineList
		{
			get { return lineList; }
		}
		List<UguiNovelTextGeneratorAddtionalLine> lineList = new List<UguiNovelTextGeneratorAddtionalLine>();

		internal UguiNovelTextGeneratorAddtional(List<UguiNovelTextCharacter> characters, UguiNovelTextGenerator generataor )
		{
			for (int i = 0; i < characters.Count; ++i  )
			{
				UguiNovelTextCharacter character = characters[i];

				//線の作成
				if (character.CustomInfo.IsStrikeTop)
				{
					lineList.Add(new UguiNovelTextGeneratorAddtionalLine(UguiNovelTextGeneratorAddtionalLine.Type.Strike, characters, i, generataor));
				}
				if (character.CustomInfo.IsUnderLineTop)
				{
					lineList.Add(new UguiNovelTextGeneratorAddtionalLine(UguiNovelTextGeneratorAddtionalLine.Type.UnderLine, characters, i, generataor));
				}

				//ルビ情報の作成
				if (character.CustomInfo.IsRubyTop)
				{
					rubyList.Add(new UguiNovelTextGeneratorAddtionalRuby(characters, i, generataor));
				}
			}		
		}

		//フォントから文字の情報を設定する
		internal bool TrySetFontCharcters(Font font)
		{
			//線用のフォント設定
			foreach (UguiNovelTextGeneratorAddtionalLine line in lineList)
			{
				if (!line.CharacteData.TrySetCharacterInfo(font))
				{
					return false;
				}
			}

			//ルビのフォント設定
			foreach (UguiNovelTextGeneratorAddtionalRuby rubyGroup in rubyList)
			{
				foreach (UguiNovelTextCharacter ruby in rubyGroup.RubyList)
				{
					if (!ruby.TrySetCharacterInfo(font))
					{
						return false;
					}
				}
			}
			return true;
		}

		//全文字情報リストを作成
		internal List<UguiNovelTextCharacter> MakeCharacterList()
		{
			List<UguiNovelTextCharacter> characterList = new List<UguiNovelTextCharacter>();

			//線用のフォント設定
			foreach (UguiNovelTextGeneratorAddtionalLine line in lineList)
			{
				characterList.Add(line.CharacteData);
			}

			//ルビのフォント設定
			foreach (UguiNovelTextGeneratorAddtionalRuby rubyGroup in rubyList)
			{
				foreach (UguiNovelTextCharacter ruby in rubyGroup.RubyList)
				{
					characterList.Add(ruby);
				}
			}
			return characterList;
		}

		//文字情報を取得した後の初期化
		internal void InitAfterCharacterInfo(UguiNovelTextGenerator generator)
		{
			foreach (UguiNovelTextGeneratorAddtionalRuby ruby in RubyList)
			{
				ruby.InitAfterCharacterInfo(generator);
			}
		}

		//行の先頭だった場合のスペースサイズを取得
		internal float GetTopLetterSpace(UguiNovelTextCharacter lineTopCharacter, UguiNovelTextGenerator generator)
		{
			float space = 0;
			foreach (UguiNovelTextGeneratorAddtionalRuby ruby in RubyList)
			{
				if (!ruby.IsWideType && ruby.TopCharaceter == lineTopCharacter)
				{
					space = generator.LetterSpaceSize / 2;
					break;
				}
			}
			return space;
		}

		//文字の最大幅を取得
		internal float GetMaxWidth(UguiNovelTextCharacter currentData)
		{
			if (currentData.CustomInfo.IsRubyTop)
			{
				foreach (UguiNovelTextGeneratorAddtionalRuby ruby in RubyList)
				{
					if (ruby.TopCharaceter == currentData)
					{
						return Mathf.Max(ruby.StringWidth, ruby.RubyWidth);
					}
				}
			}
			return currentData.Width;
		}

		//表示位置の初期化
		internal void InitPosition(UguiNovelTextGenerator generator)
		{
			
			//改行などによって複数の線が必要な場合を考慮
			List<UguiNovelTextGeneratorAddtionalLine> newLineList = new List<UguiNovelTextGeneratorAddtionalLine>();
			foreach (UguiNovelTextGeneratorAddtionalLine line in lineList)
			{
				newLineList.AddRange(line.MakeOtherLineInTextLine(generator));
			}
			//新たな線を追加
			lineList.AddRange(newLineList);

			//位置の初期化
			foreach (UguiNovelTextGeneratorAddtionalLine line in lineList)
			{
				line.InitPosition(generator);
			}

			foreach (UguiNovelTextGeneratorAddtionalRuby ruby in RubyList)
			{
				ruby.InitPosition(generator);
			}
		}

		//頂点を作成
		internal void MakeVerts(Color color, UguiNovelTextGenerator generator)
		{
			foreach (UguiNovelTextGeneratorAddtionalLine line in lineList)
			{
				line.CharacteData.MakeVerts(color, generator);
			}

			foreach (UguiNovelTextGeneratorAddtionalRuby ruby in RubyList)
			{
				foreach (UguiNovelTextCharacter character in ruby.RubyList)
				{
					character.MakeVerts(color, generator);
				}
			}
		}

		//描画頂点を追加
		internal void AddVerts(List<UIVertex> verts, Vector2 endPosition, UguiNovelTextGenerator generator)
		{
			foreach (UguiNovelTextGeneratorAddtionalLine line in lineList)
			{
				verts.AddRange(line.GetDrawVertex(endPosition, generator));
			}

			foreach (UguiNovelTextGeneratorAddtionalRuby ruby in RubyList)
			{
				verts.AddRange(ruby.GetDrawVertex(endPosition));
			}
		}

	};
}
