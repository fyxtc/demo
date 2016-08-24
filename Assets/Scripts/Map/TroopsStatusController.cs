using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Linq;

public class TroopsStatusController : MonoBehaviour {
	public GameObject[] showTroops;
	public GameObject[] prefabs;
	public GameObject[] buttons;
	public GameObject showPanel;
	public Button ok;
	public Button clean;
	public Button cancel;
	// private GameObject[] showTroops = new GameObject[4]; // sorted by rank

	void Awake(){
		for(int i = 0; i < showTroops.Length; i++){
			Destroy(showTroops[i]);
			showTroops[i] = null;
		}

		for(int i = 0; i < buttons.Length; i++){
			buttons[i].GetComponent<TroopStatusButton>().Handler += OnTroopStatusEvent;
		}

	}

	public ButtonStatus GetButtonStatus(TroopType type){
		return buttons[(int)type].GetComponent<TroopStatusButton>().GetTroopStatus();
	}

	void OnTroopStatusEvent(object sender, EventArgs e){
		TroopStatusButton btn = sender as TroopStatusButton;
		TroopStatusEvent ev = (TroopStatusEvent)e;
		TroopType type = ev.Type;
		ButtonAction action = ev.Action;
		switch(action){
			case ButtonAction.SELECTED:
				if(GetShowCount() == 4){
					Debug.Log("show count max");
				}else{
					PlayerManager.Instance.UpdateUsingTroops(type, true);
					InsertTroop(type);
				}
				break;
			case ButtonAction.DISSELECTED:
				PlayerManager.Instance.UpdateUsingTroops(type, false);
				RemoveTroop(type);
				break;
			case ButtonAction.UNLOCKED:
				PlayerManager.Instance.UpdateOwnTroops(type);
				// UpdateButtonStatus(type);
				break;
			default:
				Debug.Assert(false);
				break;
		}
		btn.UpdateStatus();
	}


	GameObject CreateTroopByType(TroopType type){
		GameObject newObj = Instantiate(prefabs[(int)type]) as GameObject;
		newObj.transform.SetParent(showPanel.transform);
		newObj.transform.localScale = new Vector3(15, 15, 0);
		return newObj;
	}

	void InsertTroop(TroopType type){
		GameObject newTroop = CreateTroopByType(type);
		int insertIndex = GetInsertIndex(type);
		Debug.Assert(0 <= insertIndex && insertIndex <= showTroops.Length);

		// 为0直接插
		if(insertIndex != 0){
			for(int i = showTroops.Length-1; i > insertIndex; i--){
				if(showTroops[i] == null && showTroops[i-1] != null){
					showTroops[i] = showTroops[i-1];
					showTroops[i-1] = null;
				}
			}
		}
		showTroops[insertIndex] = newTroop;
		ResortSiblingIndex();
	}

	int GetShowCount(){
		return showTroops.Count(x => x != null);
	}

	void ResortSiblingIndex(){
		int count = GetShowCount();
		for(int i = 0; i < showTroops.Length; i++){
			if(showTroops[i]){
				// Debug.Log("set " + i + " index " + (count-1-i));
				showTroops[i].transform.SetSiblingIndex(count-1 - i);
			}else{
				break;
			}
		}
	}

	void RemoveTroop(TroopType type){
		int targetIndex = FindObjectByTroopType(type);
		if(targetIndex != -1){
			Destroy(showTroops[targetIndex]);
			showTroops[targetIndex] = null;
			// resort
			for(int i = targetIndex+1; i < showTroops.Length; i++){
				showTroops[i-1] = showTroops[i];
			}
		}
		ResortSiblingIndex();
	}


	int GetInsertIndex(TroopType type){
		int rank = ConfigManager.share().CharacterConfig.GetModel(type).Rank;
		int insertIndex = -1;
		// 从右往左，总共有三种情况插入，前插，中插，后插，因为中间肯定不存在空隙的，每次remove都会重排
		for(int i = 0; i < showTroops.Length; i++){
			if(showTroops[i]){
				int iRank = GetRank(showTroops[i]);
				Debug.Assert(rank != iRank, "same rank");
				// 找到第一个大于自己的位置，然后后面后移，插完之后还要整体前移，或者每次在remove之后前移，避免0位空
				if(rank < iRank){
					insertIndex = i;
					break;
				}
			}else{
				insertIndex = i;
				break;
			}
		}
		Debug.Assert(insertIndex != -1, "error insert index");
		return insertIndex;
	}

	int GetRank(GameObject obj){
		TroopType type = obj.GetComponent<TroopShowController>().type;
		int rank = ConfigManager.share().CharacterConfig.GetModel(type).Rank;
		return rank;
	}

	int FindObjectByTroopType(TroopType type){
		for(int i = 0; i < showTroops.Length; i++){
			if(showTroops[i]){
				if(showTroops[i].GetComponent<TroopShowController>().type == type){
					return i;
				}
			}
		}
		return -1;
	}

	// Use this for initialization
	void Start () {
		List<int> troops = PlayerManager.Instance.UsingTroops;
		// List<int> troops = new List<int>{0, 1, 2};
		for(int i = 0; i < troops.Count; i++){
			InsertTroop((TroopType)i);
		}

		// foreach(GameObject obj in buttons){
		// 	obj.GetComponent<TroopStatusButton>().UpdateStatus();
		// }
		// 委托的两种写法
		clean.onClick.AddListener(() => PlayerPrefs.DeleteAll());
		ok.onClick.AddListener(delegate() { PlayerManager.Instance.SaveTroops();});
		cancel.onClick.AddListener(() => gameObject.SetActive(false));

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
