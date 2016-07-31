using UnityEngine;
using System.Collections;

public class CamerController : MonoBehaviour {
    ConfigManager configManager;
	// Use this for initialization
	void Start () {
        configManager = ConfigManager.share();
    	configManager.loadConfig();
		Debug.Log("saber: " + configManager.getCharacterConfig().Saber.Life);
		Debug.Log("archer: " + configManager.getCharacterConfig().Archer.Attack);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
