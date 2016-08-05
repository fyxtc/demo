using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class PlayerTroopController : MonoBehaviour {
    public bool isMy = true;
    float[] posConfig = {-2.5f, -5f, -3.0f, -4.5f};
    int maxCount = 4; // 最多能带四个兵团
    Dictionary<TroopType, int> data = new Dictionary<TroopType, int>(); 
	Dictionary<TroopType, List<GameObject>> troops = new Dictionary<TroopType, List<GameObject>>();
	public GameObject saber;
	public GameObject archer;
    public bool IsGameOver{get; set;}

    public GameObject otherTroopObj;
    private PlayerTroopController otherTroopController;

    // UI
    public List<Text> countText;
    public List<Button> skillButtons;
    bool isBuffing = false;
    SkillController buffingController = null;

	// Use this for initialization
	void Start () {
        data.Add(TroopType.Saber, 1);
        data.Add(TroopType.Archer, 1);
        otherTroopController = otherTroopObj.GetComponent<PlayerTroopController>();
        InitTroops();
        // Debug.Log("my is my " + isMy + " other is my " + otherTroopController.isMy);

        foreach(Button btn in skillButtons){
            btn.GetComponent<SkillController>().SkillEventHandler += OnSkillEvent;
        }
    }

    void InitTroops(){
        int textIndex = 0;
        foreach (KeyValuePair<TroopType, int> item in data) {
            TroopType troopType = item.Key;
            int count = item.Value;
            // Debug.Log("key=" + item.Key.ToString() + "；value=" + item.Value.ToString());  
            List<GameObject> characters = new List<GameObject>();
            switch (troopType){
            case TroopType.Saber:
                for(int i = 0; i < count; i++){
					GameObject obj = Instantiate(saber);
                    obj.GetComponent<SaberController>().IsMy = isMy;
                    obj.GetComponent<SaberController>().OtherTroopController = otherTroopController;
                    characters.Add(obj);
                }
                break;
            case TroopType.Archer:
                for(int i = 0; i < count; i++){
                    GameObject obj = Instantiate(archer);
                    obj.GetComponent<ArcherController>().IsMy = isMy;
                    obj.GetComponent<ArcherController>().OtherTroopController = otherTroopController;
                    characters.Add(obj);
                }
                break;
            }
            troops.Add(troopType, characters);
            // Debug.Log(textIndex + " / " + countText.Count);
            countText[textIndex].text = "" + count;
            textIndex++;
        }

        while(countText.Count > troops.Count){
            int last = countText.Count - 1;
            Destroy(countText[last]);
            countText.RemoveAt(last);
        }
        Debug.Assert(troops.Count > 0 && troops.Count < maxCount, "troops count error");
        resort();
    }


    public GameObject GetAttackTarget(){
        if(troops.Count == 0){
            Debug.Log(isMy ? "you lose" : "you win");
            return null;
        }
        GameObject target = null;
        foreach (KeyValuePair<TroopType, List<GameObject>> item in troops) {
            List<GameObject> troop = item.Value;
            if(troop.Count == 0){
                continue;
            }else{
                if(!target){
                    target = troop[0];
                }
            }

            for(int i = 0; i < troop.Count; i++){
                GameObject obj = troop[i];
                if(obj.GetComponent<BaseController>().IsDead){
                    continue;
                }
                if(isMy ? obj.transform.position.x > target.transform.position.x : obj.transform.position.x < target.transform.position.x){
                    target = obj;
                }                
            }
        }
		return target;
    }

    Text GetTextByTroopType(TroopType type){
        return countText[(int)type]; // 这里应该要根据rank来的。。。先简单写
    }

    void CleanDeadObj(){
		bool isOver = true;
        foreach (KeyValuePair<TroopType, List<GameObject>> item in troops) {
           TroopType troopType = item.Key;
            List<GameObject> troop = item.Value;
            for(int i = 0; i < troop.Count; i++){
                GameObject obj = troop[i];
                if(obj.GetComponent<BaseController>().IsDead){
                    Destroy(obj);
                    troop.RemoveAt(i);
                }
            }
            GetTextByTroopType(troopType).text = troop.Count + "";
			if (troop.Count > 0) {
				isOver = false;
			}
        }
        if(isOver){
            IsGameOver = true;
            Debug.Log(isMy ? "you lose" : "you win");
        }
    }

    void OnSkillEvent(object sender, EventArgs e){
        SkillEvent ev = (SkillEvent)e;
        // 给自己对应的小兵加
        if(ev.IsMy == isMy){
            // 如果已经处于技能中，而且又来了一个使用，需要覆盖，先stop当前的
            if(isBuffing && ev.Status == SkillStatus.STATUS_USING && buffingController){
                buffingController.IsForceStop = true;
                buffingController.SkillStop();
            }
            buffingController = (SkillController)sender;
            Debug.Assert(buffingController, "buffingController error");
            isBuffing = ev.Status == SkillStatus.STATUS_USING;
            foreach (KeyValuePair<TroopType, List<GameObject>> item in troops) {
                foreach(GameObject obj in item.Value){
					obj.GetComponent<BaseController>().OnSkillEvent(sender, e);
                }
            }
        }
    }
    
    void resort(){
        int rank = 0;
        foreach (KeyValuePair<TroopType, List<GameObject>> item in troops) {
            TroopType troopType = item.Key;
            List<GameObject> troop = item.Value;
            float posX = getRankPos(rank);
            double interval = getTroopInterval(troopType);
            // Debug.Log("interval: " + getTroopInterval(troopType).ToString() + ", " + interval.ToString());

            for(int i = 0; i < troop.Count; i++){
                GameObject obj = troop[i];
                double x = posX - interval * i;
                if(!isMy){
                    x = -x;
                }
				// Vector3 v = new Vector3((float)x, obj.transform.position.y, obj.transform.position.z);
                // Debug.Log("x = " + x.ToString());
				Vector3 v = new Vector3((float)x, obj.transform.position.y, 0);
				obj.transform.position = v;
				// obj.transform.Translate(v);
            }
            // countText[rank].transform.position = new Vector3(posX, countText[rank].transform.position.y, 0);
            rank++;
        }
    }

    double getTroopInterval(TroopType type){
        double res = 0.0;
        switch (type) 
        {
		case TroopType.Saber:
			res = ConfigManager.share ().CharacterConfig.Saber.Interval;
			break;
		case TroopType.Archer:
			res = ConfigManager.share ().CharacterConfig.Archer.Interval;
			break;
        default:
            Debug.Assert(false);
            break;
        }
        return res;
    }

    float getRankPos(int rank){
        return posConfig[rank];
    }

    // Update is called once per frame
    void Update () {
        if(!IsGameOver){
    		CleanDeadObj();
        }
	}
}
