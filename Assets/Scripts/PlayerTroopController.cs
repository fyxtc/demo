using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class TroopTypeComparer : IComparer<TroopType>
{
	public int Compare(TroopType lhs, TroopType rhs)
	{
		CharacterConfig config = ConfigManager.share ().CharacterConfig;
        // Debug.Log("lhs: " + config.GetModel (lhs).rank + ", rhs: " + config.GetModel (rhs).rank);
		return config.GetModel (lhs).rank - config.GetModel (rhs).rank;
	}
}

public class PlayerTroopController : MonoBehaviour {
    protected const int CHARACTER_MAX_COUNT = 28; // 目前9*3种兵 + 1servant
    public bool isMy = true;
    public bool IsMy{
        get{return isMy;}
        set{isMy = value;}
    }
    float[] posConfig = {-1.5f, -3.0f, -4.5f, -6.0f};
    int maxCount = 4; // 最多能带四个兵团
    SortedDictionary<TroopType, int> data = new SortedDictionary<TroopType, int>(new TroopTypeComparer()); 
	// Dictionary<TroopType, int> data = new Dictionary<TroopType, int>(); 
	Dictionary<TroopType, List<GameObject>> troops = new Dictionary<TroopType, List<GameObject>>();
    Dictionary<TroopType, int> rankMap = new Dictionary<TroopType, int>(); 
    public GameObject[] prefabs; // 注意！！！这个prefabs的下标就是Trooptype来索引的
    public bool IsGameOver{get; set;}

    public GameObject otherTroopObj;
	public PlayerTroopController OtherTroopController{ get; set; }

    // UI
    public List<Text> countText;
    public List<Button> skillButtons;
    public List<int> skillIds;
    bool isBuffing = false;
    SkillController buffingController = null;
    public event EventHandler TrickEventHandler;
    protected TrickConfig trickConfig = ConfigManager.share().TrickConfig;



	// Use this for initialization
	void Start () {
        Debug.Assert(prefabs.Length <= CHARACTER_MAX_COUNT, "prefabs count error");
        if(IsMy){
            int[] my = ConfigManager.share().TestConfig.TestModels[0].troops;
            for(int i = 0; i < my.Length; i++){
                if(my[i] !=0){
                    data.Add((TroopType)i, my[i]);
                }
            }
        }else{
            // int[] enemy = ConfigManager.share().TestConfig.TestModels[1].troops;
            // for(int i = 0; i < enemy.Length; i++){
            //     if(enemy[i] !=0){
            //         data.Add((TroopType)i, enemy[i]);
            //     }
            // }

            // Debug.Assert(enemy.troopType.Length == enemy.troopsCount.Length, "error gate type and count");
            int currentGate = 0;
            GateModel m = ConfigManager.share().GateConfig.GateModels[currentGate];
			data = new SortedDictionary<TroopType, int >(m.troops, new TroopTypeComparer());
            foreach (KeyValuePair<TroopType, int> item in data) {
                TroopType troopType = item.Key;
                int count = item.Value;
                // Debug.Log("troopType " + troopType + ": " + count);
            }
            skillIds = new List<int>(m.skills);
        }

        Debug.Assert(data.Count > 0 && data.Count <= maxCount, "troops count error " + data.Count);
        OtherTroopController = otherTroopObj.GetComponent<PlayerTroopController>();
        InitTroops();
        // Debug.Log("my is my " + isMy + " other is my " + OtherTroopController.isMy);
        InitSkills();
    }

    protected virtual void InitSkills(){
        // TODO, subclass
        if(IsMy){
            foreach(Button btn in skillButtons){
                btn.GetComponent<SkillController>().SkillEventHandler += OnSkillEvent;
            }
        }else{
            for(int i = skillButtons.Count - 1; i >= 0; i--){
                SkillController controller = skillButtons[i].GetComponent<SkillController>();
                // Debug.Log("1:" + controller);
                // Debug.Log("2:" + controller.Model);
                // Debug.Log("3" + (int)(controller.Model.Type));
                if(!skillIds.Contains(i)){
                    // 注意顺序，一定要先Destroy，否则就remove之后找不到这个引用了
                    // Destroy(skillButtons[i].parent);
                    skillButtons.RemoveAt(i);
                }
            }
            foreach(Button btn in skillButtons){
                btn.GetComponent<SkillController>().SkillEventHandler += OnSkillEvent;
            }
            Destroy(GameObject.Find("fuck"));
			// List<int> resultList = skillIds.FindAll(delegate(int id) { return skillIds.Contains(id); });
			Debug.Log ("skill count" + skillButtons.Count);
        }
    }

    public void OnExplodeEvent(object sender, EventArgs e){
        ExplodeEvent ev = e as ExplodeEvent;
        Debug.Log((IsMy?"my get ": "enemy get ") + "OnExplodeEvent: " + ev);
        foreach (KeyValuePair<TroopType, List<GameObject>> item in troops) {
            List<GameObject> troop = item.Value;
            foreach(GameObject obj in troop){
                if(IsInExplode(ev.Center, ev.Radius, obj.transform.position)){
                    Debug.Log("explode in " + obj.GetComponent<BaseController>().Model.Type);
                    HarmModel m = new HarmModel(ev.Harm);
                    m.Type = obj.GetComponent<BaseController>().Model.Type;
                    obj.GetComponent<BaseController>().BeingAttacked(m);
                }
            }
        }
    }

    bool IsInExplode(Vector3 center, double radius, Vector3 point){
        return (Vector3.Distance(center, point) <= radius);
    }


    void SetController(ref GameObject obj, TroopType type){
        BaseController controller = obj.GetComponent<BaseController>();
        controller.IsMy = isMy;
        controller.OtherTroopController = OtherTroopController;
        controller.MyTroopController = this;
        controller.MyTroopController.TrickEventHandler += controller.OnTrickEvent;
        // 注意这里的爆炸事件应该是让对方来接
        controller.ExplodeEventHandler += OtherTroopController.OnExplodeEvent;
        // servant死了不能再触发了
        if(type != TroopType.TROOP_SERVANT){
            controller.SummonHandler += controller.MyTroopController.OnSummonEvent;
        }/*else{
            controller.SummonHandler += null;   
        }*/
    }

    void InstantiateObject(TroopType type, int count, ref List<GameObject> characters){
        for(int i = 0; i < count; i++){
			GameObject obj = Instantiate(prefabs[(int)type]);
            SetController(ref obj, type);
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


    public GameObject GetAttackedTarget(TroopType attackerType){
        if(troops.Count == 0){
            Debug.Log(isMy ? "you lose" : "you win");
            IsGameOver = true;
            return null;
        }
        GameObject target = null;
        foreach (KeyValuePair<TroopType, List<GameObject>> item in troops) {
            List<GameObject> troop = item.Value;
            if(troop.Count == 0){
                continue;
            }else{
                BaseController controller = troop[0].GetComponent<BaseController>();
                bool canChoose = !target && !DemoUtil.IsAttackIgnoreType(attackerType, controller.Model.Type)
                                && !controller.IsZombie
                                && !controller.IsDead;
                if(canChoose){
                    target = troop[0];
                    // Debug.Log("target 1 " + target.GetComponent<BaseController>().Model.Type + ": zombie " + target.GetComponent<BaseController>().IsZombie);
                }
            }

            for(int i = 0; i < troop.Count; i++){
                GameObject obj = troop[i];
                BaseController controller = obj.GetComponent<BaseController>();
                bool canChoose = !DemoUtil.IsAttackIgnoreType(attackerType, controller.Model.Type)
                            && !controller.IsZombie
                            && !controller.IsDead;
                if(!canChoose){
                    continue;
                }
                // 死亡和zombie都不能成为目标
                if(obj.GetComponent<BaseController>().IsDead || obj.GetComponent<BaseController>().IsZombie){
                    continue;
                }
                if(!target){
                    target = obj;
                    continue;
                }

                TroopType enemyType = obj.GetComponent<BaseController>().Model.Type;
                if(!DemoUtil.IsAttackIgnoreType(attackerType, enemyType)){
                    if(isMy ? obj.transform.position.x > target.transform.position.x : obj.transform.position.x < target.transform.position.x){
                        // Debug.Log("target 2");
                        target = obj;
                    }                
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
                if(controller.IsCanBeClean){
                    TrickEvent ev = new TrickEvent();
                    ev.Tricks = controller.Model.Tricks;
                    ev.IsStart = false;
                    ev.IsSelf = false; // 这个值其实无所谓吧。。反正都是移除特技的事件，好像不一定，如果是natural的debuff不就尴尬了。。。
                    TrickEventHandler(this, ev);
                    Destroy(obj);
                    troop.RemoveAt(i);
                }
            }
            if(troopType != TroopType.TROOP_SERVANT){
                GetTextByTroopType(troopType).text = troop.Count + "";
            }
            // 如果数量大于0，而且必须不是zombie才行
			if (troop.Count > 0 && !troop[0].GetComponent<BaseController>().IsZombie) {
				isOver = false;
			}
        }
        if(isOver){
            // 这里涉及到平局的概念，比如空中还有技能在飞呢。。。
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

    void OnSummonEvent(object sender, EventArgs e){
        // 这个servant由this维护比较好
        List<GameObject> servants = new List<GameObject>();
        foreach (KeyValuePair<TroopType, List<GameObject>> item in troops) {
            TroopType troopType = item.Key;
            List<GameObject> troop = item.Value;
            foreach(GameObject obj in troop){
                if(troopType == TroopType.TROOP_MAGICICAN_SUPER2){
                    MagicianSuper2Controller m2 = obj.GetComponent<MagicianSuper2Controller>();
                    // 死了的召唤师（包括自己死了）不能再调用自己的summon
                    if(!m2.IsDead){
                        GameObject servant = m2.Summon(sender, e);
                        SetController(ref servant, TroopType.TROOP_SERVANT);
                        servants.Add(servant);
                    }
                }
            }
        }

        if(servants.Count > 0){
            Debug.Assert(servants.Count == 1, "servants Count > 1:" + servants.Count);
            if(troops.ContainsKey(TroopType.TROOP_SERVANT)){
                List<GameObject> existedServants = troops[TroopType.TROOP_SERVANT];
                existedServants.Add(servants[0]);
            }else{
                troops.Add(TroopType.TROOP_SERVANT, servants);
            }
        }

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
