using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapController : MonoBehaviour {
    public GameObject[] tollGates;
    public Text goldText;
    private int unlockGate;
    public Button editTroop;
    public GameObject editTroopPanel;

	// Use this for initialization
	void Start () {
	   Debug.Assert(tollGates.Length != 0, "tollGates length 0");
       editTroopPanel.SetActive(false);
       editTroop.onClick.AddListener(() => editTroopPanel.SetActive(!editTroopPanel.activeSelf));
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    // 总结：我们尽量将其他Object的reference设置等事情放在Awake处理。然后将这些reference的Object的赋值设置放在Start()中来完成。
    void Awake(){
        // 为了测试方便，没必要总是在Awake里调用，应该在游戏开始界面调用一次就够了
        PlayerManager.Instance.LoadLocalData();
        UpdateMap();
        UpdateGold();
    }

    void UpdateMap(){
        unlockGate = PlayerManager.Instance.UnlockGate;
        Debug.Log("update map to lock gate: " + unlockGate);
        for(int i = unlockGate+1; i < tollGates.Length; i++){
            tollGates[i].SetActive(false);
        }
    }

    void UpdateGold(){
        PlayerManager.Instance.UpdateGold(10);
        goldText.text = "GOLD: " + PlayerManager.Instance.Gold;
    }

}
