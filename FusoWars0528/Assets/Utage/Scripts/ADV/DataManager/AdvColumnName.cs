//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	public enum AdvColumnName
	{
		Tag,			//タグ
		Param1,			//パラメーター1
		Param2,			//パラメーター2
		CharacterName,	//キャラ名
		Pattern,		//パターン
		Pivot,			//ピボット位置
		Scale,			//スケール
		FileName,		//ファイル名
		Streaming,		//ストリーミング
		Version,		//ファイルバージョン
		LayerName,		//レイヤー名
		Type,			//タイプ
		X,				//座標X
		Y,				//座標Y
		Order,			//描画順
		Label,			//ラベル
		Value,			//値

		Command,		//コマンド
		Arg1,			//引数1
		Arg2,			//引数2
		Arg3,			//引数3
		Arg4,			//引数4
		Arg5,			//引数5
		Arg6,			//引数6

		WindowType,		//メッセージウィンドウのタイプ
		PageCtrl,		//ページコントローラー
		Text,			//テキスト
		Voice,			//ボイス
		VoiceVersion,	//ボイスバージョン
	
		LayerMask,		//レイヤーマスク名（Unityのレイヤー名）
		Title,			//表示タイトル
		Thumbnail,		//サムネイル用ファイルのパス
		ThumbnailVersion,	//サムネイル用ファイルのバージョン

		ScenarioLabel,		//シナリオラベル
		NameText,			//表示名
		CgCategolly,		//CGのカテゴリ
		FileType,			//ファイルのタイプ

		Z,					//Z値
		Categolly,			//カテゴリ
		IgnoreLoad,			//ロード無視
		Conditional,		//条件式

		IconFileName,		//アイコンファイル
		Chapter,			//チャプター
	};
}
