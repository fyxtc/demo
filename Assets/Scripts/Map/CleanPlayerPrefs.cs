using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CleanPlayerPrefs : MonoBehaviour {

	void Awake(){
		GetComponent<Button>().onClick.AddListener(delegate() { PlayerPrefs.DeleteAll();});
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
