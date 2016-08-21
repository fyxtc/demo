using UnityEngine;
using System.Collections;

public class StarPanel : MonoBehaviour {
    public GameObject[] star;

    void Awake(){
    }

    // Use this for initialization
    void Start () {
    
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    public void UpdateStar(int curStar){
        for(int i = 0; i < 3; i++){
            star[i].SetActive(i < curStar ? true : false);
        }
    }
}
