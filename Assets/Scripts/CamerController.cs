using UnityEngine;
using UnityEngine.SceneManagement;

public class CamerController : MonoBehaviour {
    ConfigManager configManager;
	// Use this for initialization
	void Start () {
        configManager = ConfigManager.share();
    	configManager.LoadConfig(true);
    	 
		// Debug.Log("saber: " + configManager.CharacterConfig.Saber.Life);
		// Debug.Log("archer: " + configManager.CharacterConfig.Archer.Attack);
	}

    // IEnumerator LoadConfig(string file)
    // {
    //     string sPath= Application.streamingAssetsPath + Path.DirectorySeparatorChar + file;
    //     WWW www = new WWW(sPath);
    //     yield return www;
    //     _result = www.text;
    // }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {  
            // ConfigManager.share().LoadConfig(true);
			SceneManager.LoadScene ("main");
		} 
	}
}
