using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TroopsStatusController : MonoBehaviour {
	public GameObject[] showTroops;
	public GameObject[] prefabs;
	public GameObject[] buttons;
	// private GameObject[] showTroops = new GameObject[4]; // sorted by rank

	void Awake(){
		for(int i = 0; i < showTroops.Length; i++){
			GameObject newObj = Instantiate(prefabs[i], showTroops[i].transform.position, showTroops[i].transform.rotation) as GameObject;
			newObj.transform.SetParent(showTroops[i].transform.parent);
			newObj.transform.localScale = new Vector3(15, 15, 0);
			Destroy(showTroops[i]);
			showTroops[i] = newObj;
		}

		for(int i = 0; i < buttons.Length; i++){
			buttons[i].GetComponent<TroopStatusButton>().Handler += OnTroopStatusEvent;
		}
	}

	void OnTroopStatusEvent(object sender, EventArgs e){
		TroopStatusEvent ev = (TroopStatusEvent)e;
		TroopType type = ev.Type;
		bool isAdd = ev.IsAdd;
		int targetIndex = FindObjectByTroopType(type);
		if(isAdd){

		}else{
			Destroy(showTroops[targetIndex]);
			showTroops[targetIndex] = null;
		}
	}

	GameObject CreateTroopByType(TroopType type){
		// TODO: SORT
		int i = (int)type;
		GameObject newObj = Instantiate(prefabs[i], showTroops[i].transform.position, showTroops[i].transform.rotation) as GameObject;
		newObj.transform.SetParent(showTroops[i].transform.parent);
		newObj.transform.localScale = new Vector3(15, 15, 0);
		Destroy(showTroops[i]);
		return newObj;
		// showTroops[i] = newObj;		
	}

	int FindObjectByTroopType(TroopType type){
		for(int i = 0; i < showTroops.Length; i++){
			if(showTroops[i].GetComponent<TroopShowController>().type == type){
				return i;
			}
		}
		return -1;
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
