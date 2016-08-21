using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Prime31.TransitionKit;
using UnityEngine.SceneManagement;

public class TollGateDetailController : MonoBehaviour {
    public int CurGate{get; set;}
    public int CurStar{get; set;}
    private GateConfig gateConfig;

    public GameObject detailPanel;
    public Text title;
    public Button closeButton;
    public GameObject[] troopsIcon = new GameObject[4]; // 最多四个
    public Text desc;
    public Button diffButton;
    public Button simpleButton;
    public Text diffText;
    public Button attackButton;
    public Sprite[] icons;

    void Start(){
        closeButton.onClick.AddListener(OnClose);
        attackButton.onClick.AddListener(OnAttack);
        diffButton.onClick.AddListener(OnDiff);
        simpleButton.onClick.AddListener(OnSimple);
    }

    void OnClose(){
        detailPanel.SetActive(!detailPanel.activeSelf);
    }

    void OnAttack(){
        // OnClose();
        PlayerManager.Instance.curGate = CurGate;

        // var slices = new VerticalSlicesTransition()
        // {
        //     nextScene = 2,
        //     divisions = Random.Range( 3, 20 )
        // };
        // TransitionKit.instance.transitionWithDelegate( slices );

        SceneManager.LoadScene(2);
    }

    void OnEnable(){
        // UpdateUI();
    }

    void OnSimple(){
        UpdateUI(CurGate, true);
        diffButton.enabled = true;
        diffButton.image.color = Color.white;
        simpleButton.enabled = false;
        simpleButton.image.color = Color.gray;
    }

    void OnDiff(){
        UpdateUI(CurGate, false);
        simpleButton.enabled = true;
        simpleButton.image.color = Color.white;
        diffButton.enabled = false;
        diffButton.image.color = Color.gray;
    }

    public void UpdateUI(int curGate, bool isSimple){
        // Debug.Log("onable " + CurGate);
        CurGate = curGate;
        title.text = "Toll Gate " + CurGate;

        if(isSimple){
            gateConfig = ConfigManager.share().GateConfigSimple;
        }else{
            gateConfig = ConfigManager.share().GateConfigDiff;
        }
        // Debug.Log(gateConfig.GateModels[CurGate].troops.Count);
        GateModel m = gateConfig.GateModels[CurGate];
        int index = 0;
        SortedDictionary<TroopType, int> data = new SortedDictionary<TroopType, int >(m.troops, new TroopTypeComparer());
        foreach (KeyValuePair<TroopType, int> item in data) {
            TroopType troopType = item.Key;
            int count = item.Value;
            troopsIcon[index].SetActive(true);
            troopsIcon[index].GetComponent<Image>().sprite = icons[(int)troopType];
            index++;
            // Debug.Log("GATE troops:" + troopType + " " + count);
        }

        for(int i = index; i < troopsIcon.Length; i++){
            if(troopsIcon[i]){
                troopsIcon[i].SetActive(false);
            }
        }

        diffText.text = isSimple ? "Simple" : "Difficulty";
    }

}
