using UnityEngine;
using System.Collections;

public class TroopsStatusController : MonoBehaviour {
	public GameObject[] showTroops;
	public GameObject[] prefabs;
	// private GameObject[] showTroops = new GameObject[4]; // sorted by rank

	void Awake(){
		for(int i = 0; i < showTroops.Length; i++){
			GameObject newObj = Instantiate(prefabs[i], showTroops[i].transform.position, showTroops[i].transform.rotation) as GameObject;
			newObj.transform.SetParent(showTroops[i].transform.parent);
			newObj.transform.localScale = new Vector3(15, 15, 0);
			Destroy(showTroops[i]);
			showTroops[i] = newObj;
		}
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
