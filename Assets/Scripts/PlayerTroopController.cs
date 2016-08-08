using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class PlayerTroopController : MonoBehaviour {
    private const int CHARACTER_MAX_COUNT = 9; // 目前九种兵
    public bool isMy = true;
    public bool IsMy{
        get{return isMy;}
        set{isMy = value;}
    }
    float[] posConfig = {-1.5f, -3.0f, -4.5f, -6.0f};
    int maxCount = 4; // 最多能带四个兵团
    Dictionary<TroopType, int> data = new Dictionary<TroopType, int>(); 
	Dictionary<TroopType, List<GameObject>> troops = new Dictionary<TroopType, List<GameObject>>();
    Dictionary<TroopType, int> rankMap = new Dictionary<TroopType, int>(); 
	// public GameObject saber;
	// public GameObject archer;
 //    public GameObject recover;
 //    public GameObject dancer;
 //    public GameObject spyer;
 //    public GameObject rider;
 //    public GameObject flyer;
 //    public GameObject magician;
 //    public GameObject titan;
    public GameObject[] prefabs; // 注意！！！这个prefabs的下标就是Trooptype来索引的
    public bool IsGameOver{get; set;}

    public GameObject otherTroopObj;
	public PlayerTroopController OtherTroopController{ get; set; }

    // UI
    public List<Text> countText;
    public List<Button> skillButtons;
    bool isBuffing = false;
    SkillController buffingController = null;
    public event EventHandler TrickEventHandler;
    protected TrickConfig trickConfig = ConfigManager.share().TrickConfig;



	// Use this for initialization
	void Start () {
        Debug.Assert(prefabs.Length <= CHARACTER_MAX_COUNT, "prefabs count error");
        if(IsMy){
            data.Add(TroopType.TROOP_SABER, 1);
            data.Add(TroopType.TROOP_ARCHER, 1);
            data.Add(TroopType.TROOP_RECOVER, 1);
            data.Add(TroopType.TROOP_DANCER, 1);
        }else{
            data.Add(TroopType.TROOP_RIDER, 1);
            data.Add(TroopType.TROOP_FLYER, 1);
            data.Add(TroopType.TROOP_MAGICICAN, 1);
            data.Add(TroopType.TROOP_TITAN, 1);
        }
        OtherTroopController = otherTroopObj.GetComponent<PlayerTroopController>();
        InitTroops();
        // Debug.Log("my is my " + isMy + " other is my " + OtherTroopController.isMy);

        foreach(Button btn in skillButtons){
            btn.GetComponent<SkillController>().SkillEventHandler += OnSkillEvent;
        }
    }



    void SetController(ref GameObject obj){
        obj.GetComponent<BaseController>().IsMy = isMy;
        obj.GetComponent<BaseController>().OtherTroopController = OtherTroopController;
        obj.GetComponent<BaseController>().MyTroopController = this;
        obj.GetComponent<BaseController>().MyTroopController.TrickEventHandler += obj.GetComponent<BaseController>().OnTrickEvent;
    }

    void InstantiateObject(TroopType type, int count, ref List<GameObject> characters){
        for(int i = 0; i < count; i++){
			GameObject obj = Instantiate(prefabs[(int)type]);
            SetController(ref obj);
            characters.Add(obj);
        }
    }

    void InitTroops(){
        int textIndex = 0;
        foreach (KeyValuePair<TroopType, int> item in data) {
            TroopType troopType = item.Key;
            int count = item.Value;
            // Debug.Log("key=" + item.Key.ToString() + "；value=" + item.Value.ToString());  
            List<GameObject> characters = new List<GameObject>();
            InstantiateObject(troopType, count, ref characters);
            troops.Add(troopType, characters);
            // Debug.Log(textIndex + " / " + countText.Count);
            countText[textIndex].text = "" + count;
            rankMap.Add(troopType, textIndex);
            textIndex++;
        }

        while(countText.Count > troops.Count){
            int last = countText.Count - 1;
            Destroy(countText[last]);
            countText.RemoveAt(last);
        }
        Debug.Assert(troops.Count > 0 && troops.Count <= maxCount, "troops count error " + troops.Count);
        Resort();
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
        return countText[rankMap[type]]; // 这里应该要根据rank来的。。。先简单写
    }

    void CleanDeadObj(){
		bool isOver = true;
        foreach (KeyValuePair<TroopType, List<GameObject>> item in troops) {
           TroopType troopType = item.Key;
            List<GameObject> troop = item.Value;
            // 这里现在是有bug的，不能在这样的for里面remove
            for(int i = 0; i < troop.Count; i++){
                GameObject obj = troop[i];
                BaseController controller = obj.GetComponent<BaseController>();
                if(controller.IsDead){
                    TrickEvent ev = new TrickEvent();
                    ev.Tricks = controller.Model.Tricks;
                    ev.IsStart = false;
                    ev.IsSelf = false; // 这个值其实无所谓吧。。反正都是移除特技的事件，好像不一定，如果是natural的debuff不就尴尬了。。。
                    TrickEventHandler(this, ev);
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

    public void DispatchTricks(List<int> trickIds, bool isStart){
        // 所有这个时刻触发的特技的分发处理，所有的basecontroller的status trick应该都要回传到这里再次转发
        // Debug.Log("DispatchTricks count:" + trickIds.Count);
        foreach(int id in trickIds){
            DispatchTrick(id, isStart, trickConfig.GetModel(id).IsSelf);
        }
    }

    public void DispatchTrick(int trickId, bool isStart, bool isSelf){
        Debug.Log("DispatchTrick " + trickId + ", " + isStart + ", " + isSelf);
        TrickEvent ev = new TrickEvent();
        ev.Tricks = new int[1]{trickId};
        ev.IsStart = isStart;
        ev.IsSelf = isSelf;
        ev.IsMy = isMy;
        TrickEventHandler(this, ev);
    }


    
    void Resort(){
        int rank = 0;
        foreach (KeyValuePair<TroopType, List<GameObject>> item in troops) {
            TroopType troopType = item.Key;
            List<GameObject> troop = item.Value;
            float posX = GetRankPos(rank);
            double interval = GetTroopInterval(troopType);
            // Debug.Log("interval: " + GetTroopInterval(troopType).ToString() + ", " + interval.ToString());

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

    double GetTroopInterval(TroopType type){
        double res = 0.0;
		res = ConfigManager.share ().CharacterConfig.GetModel(type).Interval;
        return res;
    }

    float GetRankPos(int rank){
        return posConfig[rank];
    }

    // Update is called once per frame
    void Update () {
        if(!IsGameOver){
    		CleanDeadObj();
        }
	}
}
