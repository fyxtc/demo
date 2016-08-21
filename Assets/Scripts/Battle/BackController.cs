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
        PlayerManager.Instance.UpdateUnlockGate();
        PlayerManager.Instance.UpdateStars(1, true);
		SceneManager.LoadScene(1);
	}
}
