using UnityEngine;
using System.Collections;

public class CamerController : MonoBehaviour {
    ConfigManager configManager;
	// Use this for initialization
	void Start () {
        configManager = ConfigManager.share();
    	configManager.LoadConfig();
		// Debug.Log("saber: " + configManager.CharacterConfig.Saber.Life);
		// Debug.Log("archer: " + configManager.CharacterConfig.Archer.Attack);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
