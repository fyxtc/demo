using UnityEngine;
using System.Collections;

public class MapController : MonoBehaviour {
    public GameObject[] tollGates;
    private int unlockGate;

	// Use this for initialization
	void Start () {
	   Debug.Assert(tollGates.Length != 0, "tollGates length 0");
    }
    
    // Update is called once per frame
    void Update () {
    
    }

    // 总结：我们尽量将其他Object的reference设置等事情放在Awake处理。然后将这些reference的Object的赋值设置放在Start()中来完成。
    void Awake(){
        UpdateMap();
    }

    void UpdateMap(){
        unlockGate = PlayerManager.Instance.unlockGate;
        Debug.Log("update map to lock gate: " + unlockGate);
        for(int i = unlockGate+1; i < tollGates.Length; i++){
            tollGates[i].SetActive(false);
        }
    }

}
