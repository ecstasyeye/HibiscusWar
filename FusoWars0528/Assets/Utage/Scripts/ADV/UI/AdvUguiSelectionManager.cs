//----------------------------------------------
// UTAGE: Unity Text Adventure Game Engine
// Copyright 2014 Ryohei Tokimura
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Utage;
using System.Collections.Generic;

namespace Utage
{

	/// <summary>
	/// 選択肢表示のサンプル
	/// </summary>
	[AddComponentMenu("Utage/ADV/UiSelectionManager")]
	public class AdvUguiSelectionManager : MonoBehaviour
	{
		/// <summary>ADVエンジン</summary>
		public AdvEngine Engine { get { return this.engine ?? (this.engine = GetComponent<AdvEngine>()); } }
		[SerializeField]
		AdvEngine engine;

		/// <summary>選択済みのテキスト色</summary>
		/// <summary>選択済みのテキスト色を変えるか</summary>
		enum SelectedColorMode
		{
			None,		//選択済みでも変えない
			Change,		//選択済みで色を変える
		}
		[SerializeField]
		SelectedColorMode selectedColorMode = SelectedColorMode.None;
		
		/// <summary>選択済みのテキスト色</summary>
		[SerializeField]
		Color selectedColor = new Color(0.8f,0.8f,0.8f);


		//表示する選択肢プレハブのリスト
		List<GameObject> PrefabList { get { return prefabList; } }
		[SerializeField]
		List<GameObject> prefabList;

		AdvSelectionManager SelectionManager { get { return engine.SelectionManager; } }

		/// <summary>選択肢のリストビュー</summary>
		public UguiListView ListView
		{
			get { return listView ?? (listView = GetComponent<UguiListView>()); }
		}
		UguiListView listView;


		CanvasGroup CanvasGroup
		{
			get
			{
				return canvasGroup ?? (canvasGroup = UtageToolKit.GetComponentCreateIfMissing<CanvasGroup>(this.gameObject));
			}
		}
		CanvasGroup canvasGroup;

		public List<GameObject> Items { get { return items; } }
		List<GameObject> items = new List<GameObject>();

		/// <summary>開く</summary>
		public void Open()
		{
			this.gameObject.SetActive(true);
		}

		/// <summary>閉じる</summary>
		public void Close()
		{
			this.gameObject.SetActive(false);
		}

		void Awake()
		{
			SelectionManager.OnClear.AddListener(OnClear);
			SelectionManager.OnBeginShow.AddListener(OnBeginShow);
			SelectionManager.OnBeginWaitInput.AddListener(OnBeginWaitInput);
			ClearAll();
		}

		//全てクリア
		void ClearAll()
		{
			ListView.ClearItems();
			foreach (var item in Items)
			{
				UtageToolKit.SafeDestroy(item);
			}
			Items.Clear();
		}

		//選択肢を作成
		void CreateItems()
		{
			ClearAll();

			List<GameObject> listViewItems = new List<GameObject>();
			foreach( var data in SelectionManager.Selections )
			{
				GameObject go = GameObject.Instantiate(GetPrefab(data)) as GameObject;
				AdvUguiSelection selection = go.GetComponentInChildren<AdvUguiSelection>();
				if(selection) selection.Init(data, OnTap);

				switch (selectedColorMode)
				{
					case SelectedColorMode.Change:
						if (Engine.SystemSaveData.SelectionData.Check(data))
						{
							go.SendMessage("OnInitSelected", selectedColor);
						}
						break;
					case SelectedColorMode.None:
					default:
						break;
				}


				Items.Add(go);
				//X,Y座標の指定がないならリストビューに追加
				if (data.X == null || data.Y == null)
				{
					listViewItems.Add(go);
				}
				else
				{
					UtageToolKit.AddChild(this.transform, go, new Vector3(data.X.Value, data.Y.Value, 0));
				}
			}
			ListView.AddItems(listViewItems);
			ListView.Reposition();
		}

		//選択肢表示に使うプレハブを取得
		GameObject GetPrefab( AdvSelection selectionData )
		{
			GameObject prefab = null;
			if (!string.IsNullOrEmpty(selectionData.PrefabName))
			{
				//プレハブ名と一致するものを探す
				prefab = PrefabList.Find(x => x.name == selectionData.PrefabName);
				if (prefab != null)
				{
					return prefab;
				}
				else
				{
					//見つからないとき
					Debug.LogError("Not found Selection Prefab : " + selectionData.PrefabName);
				}
			}

			//プレハブ名の指定がなかったら、リストの一番目を
			//旧バージョンとの互換性のためListViewから選ぶ
			prefab = PrefabList.Count > 0 ? PrefabList[0] : ListView.ItemPrefab;
			return prefab;
		}

		//リストビューのアイテムが作成されるときに呼ばれるコールバック
		void CallbackCreateItem(GameObject go, int index)
		{
			AdvSelection data = SelectionManager.Selections[index];
			AdvUguiSelection selection = go.GetComponentInChildren<AdvUguiSelection>();
			selection.Init(data, OnTap);
		}

		//選択肢が押された
		void OnTap(AdvUguiSelection item)
		{
			SelectionManager.Select(item.Data );
			ClearAll();
		}

		//クリア
		public void OnClear(AdvSelectionManager manager)
		{
			ClearAll();
		}
		
		//表示開始
		public void OnBeginShow( AdvSelectionManager manager )
		{
			CreateItems();
			CanvasGroup.interactable = false;
		}

		//入力待ち開始
		public void OnBeginWaitInput(AdvSelectionManager manager)
		{
			CanvasGroup.interactable = true;
		}
	}
}
