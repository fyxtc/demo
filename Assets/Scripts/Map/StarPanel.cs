using UnityEngine;
using System.Collections;

public class StarPanel : MonoBehaviour {
    public GameObject[] star;
    private int curStar;


    void Awake(){
        curStar = 1;
        for(int i = curStar; i < 3; i++){
            star[i].SetActive(false);
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
