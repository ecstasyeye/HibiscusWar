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
	//ノベル用の線（アンダーラインや取り消し線）描画情報
	public class UguiNovelTextGeneratorAddtionalLine
	{
		//タイプ
		public enum Type
		{
			UnderLine,
			Strike,
		};
		public Type LineType{ get { return type; }}
		Type type;

		//線の文字データ
		public UguiNovelTextCharacter CharacteData { get { return characteData; } }
		UguiNovelTextCharacter characteData;

		//線を引く文字列データ
		List<UguiNovelTextCharacter> stringData = new List<UguiNovelTextCharacter>();

		//線を引く先頭の文字
		internal UguiNovelTextCharacter TopCharaceter { get { return stringData[0]; } }

		//表示行
		UguiNovelTextLine textLine;

		internal UguiNovelTextGeneratorAddtionalLine( Type type, List<UguiNovelTextCharacter> characters, int index, UguiNovelTextGenerator generator)
		{
			InitSub(type,generator);
			stringData.Add(characters[index]);
			for (int i = index + 1; i < characters.Count; ++i)
			{
				UguiNovelTextCharacter c = characters[i];
				if (IsLineEnd(c)) break;
				stringData.Add(c);
			}
		}

		//自動改行などのために線を増やす必要がある場合
		UguiNovelTextGeneratorAddtionalLine(UguiNovelTextGeneratorAddtionalLine srcLine, int index, int count, UguiNovelTextGenerator generator)
		{
			InitSub(srcLine.type, generator);
			for (int i = 0; i < count; ++i)
			{
				stringData.Add(srcLine.stringData[index+i]);
			}
		}

		void InitSub(Type type, UguiNovelTextGenerator generator)
		{
			this.type = type;
			//ダッシュ（'—'）の文字を作成
			CharData.CustomCharaInfo custom = new CharData.CustomCharaInfo();
			custom.IsDash = true;
			custom.DashSize = 1;
			CharData data = new CharData(generator.DashChar, custom);
			characteData = new UguiNovelTextCharacter(data, generator);
		}

		//
		bool IsLineEnd(UguiNovelTextCharacter c)
		{
			switch(LineType)
			{
				case Type.Strike:
					return (!c.CustomInfo.IsStrike || c.CustomInfo.IsStrikeTop);
				case Type.UnderLine:
					return (!c.CustomInfo.IsUnderLine || c.CustomInfo.IsUnderLineTop);
				default:
					return false;
			}
		}

		//改行などで別の行に線を引く必要があるばあい、その線を作成
		internal List<UguiNovelTextGeneratorAddtionalLine> MakeOtherLineInTextLine(UguiNovelTextGenerator generator)
		{
			InitTextLine(generator);
			return MakeOtherLineInTextLineSub(generator);
		}

		void InitTextLine(UguiNovelTextGenerator generator)
		{
			foreach (UguiNovelTextLine line in generator.LineDataList)
			{
				if (line.Characters.IndexOf(TopCharaceter) >= 0)
				{
					this.textLine = line;
				}
			}
		}
		
		//改行などで別の行に線を引く必要があるばあい、その線を作成
		internal List<UguiNovelTextGeneratorAddtionalLine> MakeOtherLineInTextLineSub(UguiNovelTextGenerator generator)
		{
			List<UguiNovelTextGeneratorAddtionalLine> newLineList = new List<UguiNovelTextGeneratorAddtionalLine>();

			int endIndex = stringData.Count - 1;
			foreach (UguiNovelTextLine line in generator.LineDataList)
			{
				if ( textLine == line ) continue;

				bool isFind = false;
				int index = 0;
				int count = 0;
				for (int i = 0; i < stringData.Count; ++i )
				{
					//他の行に文字がある
					if (line.Characters.IndexOf(stringData[i]) >= 0)
					{
						if (!isFind)
						{
							index = i;
							endIndex = Mathf.Min(i, endIndex);
							isFind = true;
						}
						++count;
					}
				}
				if (isFind)
				{
					UguiNovelTextGeneratorAddtionalLine newLine = new UguiNovelTextGeneratorAddtionalLine(this, index, count,generator);
					newLineList.Add(newLine);
					newLine.InitTextLine(generator);
					if (!newLine.characteData.TrySetCharacterInfo(generator.NovelText.font))
					{
						Debug.LogError("Line Font Missing");
					}
				}
			}
			if(endIndex<stringData.Count-1)
			{
				stringData.RemoveRange(endIndex, stringData.Count-endIndex);
			}
			return newLineList;
		}

		//位置情報の初期化
		internal void InitPosition(UguiNovelTextGenerator generator)
		{
			//文字からの表示位置のオフセット
			Vector2 offset = Vector2.zero;

			float height = textLine.MaxFontSize;

			//高さを合わせる
			switch( LineType )
			{
				case Type.UnderLine:
					offset.y -= height / 2;
					break;
				case Type.Strike:
					break;
			}

			CharacteData.PositionX = TopCharaceter.PositionX + offset.x;
			CharacteData.PositionY = TopCharaceter.PositionY + offset.y;
		}

		//描画用の頂点情報を取得(文字送りに対応)
		internal List<UIVertex> GetDrawVertex(Vector2 endPosition, UguiNovelTextGenerator generator)
		{
			List<UIVertex> list = new List<UIVertex>();
			if (!TopCharaceter.IsVisible) return list;

			float xMin = TopCharaceter.PositionX;
			float xMax = TopCharaceter.EndPositionX;

			foreach (UguiNovelTextCharacter c in stringData)
			{
				if (!c.IsVisible) break;
				xMax = c.EndPositionX;
			}
			Color color = Color.white;
			foreach (UguiNovelTextCharacter c in stringData)
			{
				if (!c.IsVisible) break;
				if (c.Verts != null)
				{
					color = c.Verts[0].color;
				}
			}
			CharacteData.Width = xMax - xMin;
			CharacteData.MakeVerts(color, generator);
			list.AddRange(CharacteData.Verts);
			return list;
		}

	};
}
