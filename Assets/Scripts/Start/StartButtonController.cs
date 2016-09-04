using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Prime31.TransitionKit;
using System.IO;

public class StartButtonController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Button>().onClick.AddListener(OnClick);

        ConfigManager configManager = ConfigManager.share();
        configManager.ConfigPath =  Path.Combine(Application.dataPath, "StreamingAssets/Config");
    	configManager.LoadConfig(true);
	}

	void OnClick(){
		var slices = new VerticalSlicesTransition()
		{
			nextScene = 1,
			// nextScene = SceneManager.GetActiveScene().buildIndex == 0 ? 1 : 0,
			divisions = Random.Range( 3, 20 )
		};
		TransitionKit.instance.transitionWithDelegate( slices );
		// SceneManager.LoadScene("main");
	}
	
	// Update is called once per frame
	void Update () {
	}
}
