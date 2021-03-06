//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Utage
{

	/// <summary>
	/// ファイルタイプ
	/// </summary>
	public enum AssetFileType
	{
		/// <summary>テキスト</summary>
		Text,
		/// <summary>バイナリ</summary>
		Bytes,
		/// <summary>テクスチャ</summary>
		Texture,
		/// <summary>サウンド</summary>
		Sound,
		/// <summary>CSVファイル（テキストファイルの拡張）</summary>
		Csv,
		/// <summary>その他のオブジェクト</summary>
		UnityObject,
	};

	/// <summary>
	/// ファイルのおき場所のタイプ
	/// </summary>
	public enum AssetFileStrageType
	{
		/// <summary>WEB（一度ＤＬしたものは、デバイスストレージにキャッシュする）</summary>
		Web,
		/// <summary>WEB（デバイスストレージにキャッシュしない）</summary>
		WebNocache,				//
		/// <summary>ストリーミングアセット</summary>
		StreamingAssets,
		/// <summary>リソース</summary>
		Resources,
	};

	/// <summary>
	/// 暗号化のタイプ
	/// </summary>
	public enum AssetFileEncodeType
	{
		None,				//エンコードしない
		EncodeOnCache,		//キャッシュ書き込みのときにエンコードする
		AlreadyEncoded,		//すでにエンコードされている
		AssetBundle,		//アセットバンドルにする
	};

	/// <summary>
	/// ロードする際のフラグ
	/// </summary>
	[System.Flags]
	public enum AssetFileLoadFlags
	{
		/// <summary>なにもなし</summary>
		None = 0x00,
		/// <summary>ストリーミングでロードする</summary>
		Streaming = 0x01,
		/// <summary>3Dサウンドとしてロードする</summary>
		Audio3D = 0x02,
		/// <summary>テクスチャにミップマップを使う</summary>
		TextureMipmap = 0x04,
		/// <summary>CSVをロードする際にTSV形式でロードする</summary>
		Tsv = 0x08,
	};

	/// <summary>
	/// ロードの優先順
	/// </summary>
	public enum AssetFileLoadPriority
	{
		Default,				//通常
		Preload,				//先読み
		BackGround,				//バックグラウンドでのロード
		DownloadOnly,			//ダウンロードのみ
	};


	/// <summary>
	/// ファイルのインターフェース
	/// </summary>
	public interface AssetFile
	{
		/// <summary>ファイル名</summary>
		string FileName { get; }

		/// <summary>エクセルなどで設定したデータ（nullの場合もあるので注意）</summary>
		StringGridRow RowData { get; }

		/// <summary>関連ファイルも含めてすべてロード終了したか</summary>
		bool IsLoadEnd { get; }

		/// <summary>関連ファイルを除いてこのファイルのみロード終了したか</summary>
		bool IsLoadEndOriginal { get; }

		/// <summary>ロードエラーしたか</summary>
		bool IsLoadError { get; }

		/// <summary>ロードエラーメッセージ</summary>
		string LoadErrorMsg { get; }

		/// <summary>ストリーム再生ができるか</summary>
		bool IsReadyStreaming { get; }

		/// <summary>ロードしたテキスト</summary>
		string Text { get; }

		/// <summary>ロードしたバイナリ</summary>
		byte[] Bytes { get; }

		/// <summary>ロードしたテクスチャ</summary>
		Texture2D Texture { get; }

		/// <summary>ロードしたサウンド</summary>
		AudioClip Sound { get; }

		/// <summary>ロードしたCSV</summary>
		StringGrid Csv { get; }

		/// <summary>ロードしたCSV</summary>
		UnityEngine.Object UnityObject { get; }

		/// <summary>
		/// ロードしたテクスチャから作ったスプライトを取得
		/// </summary>
		/// <param name="pixelsToUnits"></param>
		/// <returns></returns>
		Sprite GetSprite( GraphicInfo graphicInfo, float pixelsToUnits);

		/// <summary>
		/// バージョン
		/// </summary>
		int Version { get; set; }

		/// <summary>
		/// キャッシュファイルのバージョン
		/// </summary>
		int CacheVersion { get; }

		/// <summary>
		/// ロードフラグ
		/// </summary>
		AssetFileLoadFlags LoadFlags { get; }

		/// <summary>
		/// ロードフラグを追加
		/// </summary>
		void AddLoadFlag(AssetFileLoadFlags flags);

		/// <summary>
		/// オブジェクトがファイルを使用することを宣言（参照を設定する）
		/// </summary>
		/// <param name="obj">使用するオブジェクト</param>
		void Use(System.Object obj);

		/// <summary>
		/// オブジェクトがファイルを使用することをやめる（参照を解除する）
		/// </summary>
		/// <param name="obj">使用をやめるオブジェクト</param>
		void Unuse(System.Object obj);

		/// <summary>
		/// Gameオブジェクトに、このファイルの参照コンポーネントを追加
		/// これを使った後は、GameオブジェクトがDestoryされると自動的に、参照が解除される
		/// </summary>
		/// <param name="go">参照をするGameObject</param>
		void AddReferenceComponet(GameObject go);

		/// <summary>
		/// ロード終了時のコールバック
		/// </summary>
		AssetFileEvent OnLoadComplete { get; set; }

		/// <summary>
		/// サブファイルのロードコールバック
		/// </summary>
		AssetFileEvent OnLoadSubFiles { get; set; }

		AssetFileLoadPriority Priority{ get; }

		/// <summary>
		/// ロードの準備開始
		/// </summary>
		/// <param name="loadPriority">ロードの優先順</param>
		/// <param name="referenceObj">ファイルを参照するオブジェクト</param>
		/// <returns></returns>
		void ReadyToLoad(AssetFileLoadPriority loadPriority, System.Object referenceObj);

		/// <summary>
		/// ロードの準備開始
		/// </summary>
		/// <param name="loadPriority">ロードの優先順</param>
		/// <param name="referenceObj">ファイルを参照するオブジェクト</param>
		/// <returns></returns>
		int MemSize { get; }

		int UnusedSortID { get; }

		void Unload();

//		int IncLoadErrorRetryCount();

//		void ResetLoadErrorRetryCount();

		int CountLoadErrorRetry { get; set;}

		bool IsLoadRetry { get; }

		IEnumerator CoLoadAsync(float timeOutDownload);

		void LoadComplete();

		void LoadFailed(Action<AssetFile> callBackLoadError, Action<AssetFile> realod);

		bool CheckUnuse();

		AssetFileEncodeType EncodeType { get; }

		AssetFileType FileType { get; }

		/// <summary>
		/// キャッシュを新たに書き込む必要があるか
		/// </summary>
		bool NeedsToCache { get; }

		/// <summary>
		/// 派生ファイル群
		/// 設定ファイルなどを読んだときに
		/// その中で設定されているファイルもロードする必要があるときに使う
		/// </summary>
		Dictionary<string, AssetFile> SubFiles { get; }

		/// <summary>
		/// 派生ファイルを追加してロードを開始させる
		/// </summary>
		void LoadAndAddSubFile(string path);


/*
		bool IsWriteNewCache { get; }

		bool IsCaching { get; }
	
		void DeleteOldCacheFile();

		void ReadyToWriteCache(int p1, string p2, bool isDebugCacheFileName);

		string CachePath { get; }

		AudioClip WriteCacheFileSound { get; }

		byte[] CacheWriteBytes { get; }
*/	};

	public delegate void AssetFileLoadComplete(AssetFile file);
	public delegate void AssetFileEvent(AssetFile file);
}