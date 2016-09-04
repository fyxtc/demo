using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

public class CamerController : MonoBehaviour {
    // ConfigManager configManager;
	// Use this for initialization

    void Awake(){

        
    }

    void Start () {
    	 
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
    
    #if UNITY_EDITOR
        [MenuItem ("Assets/Build AssetBundles")]
        static void BuildAllAssetBundles ()
        {
            // BuildPipeline.BuildAssetBundles ("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            string path = Application.dataPath + "/StreamingAssets";
            BuildPipeline.BuildAssetBundles (path, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        }
    #endif
}
