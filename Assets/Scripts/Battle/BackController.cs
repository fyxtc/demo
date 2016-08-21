using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Button>().onClick.AddListener(OnClick);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick(){
        // TEST
        int curGate = TollGateManager.Instance.CurGate;
        if(curGate + 1 > TollGateManager.Instance.UnlockGate){
            TollGateManager.Instance.UnlockGate = Mathf.Min(15, curGate + 1);
        }
		SceneManager.LoadScene(1);
	}
}
