//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	//動的にロードしないアセットをロードファイルのように扱うためのクラス
	public abstract class AssetFileBase : AssetFile
	{
		public AssetFileBase(string filePath, AssetFileSetting setting, StringGridRow rowData)
		{
			this.FileName = filePath;
			this.FileType = setting.FileType;
			this.EncodeType = setting.EncodeType;
			this.SubFiles = new Dictionary<string, AssetFile>();
			this.RowData = rowData;
		}
		public string FileName { get; protected set; }
		public StringGridRow RowData { get; protected set; }

		/// <summary>関連ファイルも含めてすべてロード終了したか</summary>
		public bool IsLoadEnd
		{
			get
			{
				if (!IsLoadEndOriginal) return false;
				foreach (var subFile in SubFiles.Values)
				{
					if (!subFile.IsLoadEndOriginal)
					{
						return false;
					}
				}
				return true;
			}
		}
		public bool IsLoadEndOriginal { get; protected set; }
		public bool IsLoadError { get; protected set; }

		public string LoadErrorMsg { get; protected set; }
		public int CountLoadErrorRetry { get; set; }

		public bool IsReadyStreaming { get { return true; } }

		public string Text { get; protected set; }

		public byte[] Bytes { get; protected set; }

		public Texture2D Texture { get; protected set; }

		public AudioClip Sound { get; protected set; }

		public StringGrid Csv { get; protected set; }

		public Object UnityObject { get; protected set; }

		protected Sprite Sprite { get; set; }

		/// <summary>
		/// 派生ファイル群
		/// 設定ファイルなどを読んだときに
		/// その中で設定されているファイルもロードする必要があるときに使う
		/// </summary>
		public Dictionary<string, AssetFile> SubFiles { get; protected set; }


		//ロード終了時に呼ばれる処理
		public AssetFileEvent OnLoadComplete { get; set; }

		/// サブファイルのロードコールバック
		public AssetFileEvent OnLoadSubFiles { get; set; }

		public int Version
		{
			get { return -1; }
			set { }
		}

		public int CacheVersion { get { return -1; } }

		AssetFileLoadFlags loadFlags;
		public AssetFileLoadFlags LoadFlags
		{
			get { return loadFlags; }
		}

		public AssetFileLoadPriority Priority { get; protected set; }

		/// <summary>
		/// ロードしたテクスチャから作ったスプライト
		/// </summary>
		/// <param name="pixelsToUnits">スプライトを作成する際の、座標1.0単位辺りのピクセル数</param>
		/// <returns>作成したスプライト</returns>
		public virtual Sprite GetSprite(GraphicInfo graphic, float pixelsToUnits)
		{
			return Sprite;
		}

		/// <summary>
		/// 派生ファイルを追加してロードを開始させる
		/// </summary>
		public virtual void LoadAndAddSubFile(string path)
		{
			AssetFile file = AssetFileManager.Load(path, this.Version, this);
			SubFiles.Add(path, file);
		}

		public virtual void AddLoadFlag(AssetFileLoadFlags flags)
		{
			loadFlags |= flags;
		}

		public virtual void Use(object obj) { }

		public virtual void Unuse(object obj) { }

		public virtual void AddReferenceComponet(GameObject go)
		{
			AssetFileReference fileReference = go.AddComponent<AssetFileReference>();
			fileReference.Init(this);
		}

		/// <summary>
		/// ロードの準備開始
		/// </summary>
		/// <param name="loadPriority">ロードの優先順</param>
		/// <param name="referenceObj">ファイルを参照するオブジェクト</param>
		/// <returns></returns>
		public virtual void ReadyToLoad(AssetFileLoadPriority loadPriority, System.Object referenceObj)
		{
			//ロードプライオリティの反映
			if (loadPriority < this.Priority)
			{
				this.Priority = loadPriority;
			}
			Use(referenceObj);
			this.UnusedSortID = System.Int32.MaxValue;
		}

		/// <summary>
		/// ロードの準備開始
		/// </summary>
		/// <param name="loadPriority">ロードの優先順</param>
		/// <param name="referenceObj">ファイルを参照するオブジェクト</param>
		/// <returns></returns>
		public int MemSize { get; set; }

		public int UnusedSortID { get; set; }

		public abstract IEnumerator CoLoadAsync(float timeOutDownload);
		public abstract void Unload();
		
		public virtual int IncLoadErrorRetryCount() { return 0; }

		public virtual bool NeedsToCache { get { return false; } }

		public virtual void LoadFailed(System.Action<AssetFile> callBackLoadError, System.Action<AssetFile> realod)
		{
			if (callBackLoadError != null)
			{
				callBackLoadError(this);
			}
		}

		public virtual void ResetLoadErrorRetryCount() { }

		public bool IsLoadRetry { get; set; }

		public virtual void LoadComplete() { }

		public virtual bool CheckUnuse() { return false; }

		public virtual AssetFileEncodeType EncodeType { get; set; }

		public virtual AssetFileType FileType { get; set; }
	}
}
