using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class PlayerManager {
    private const string KEY_CUR_GATE = "KEY_CUR_GATE";
    private const string KEY_UNLOCK_GATE = "KEY_UNLOCK_GATE";
    private const string KEY_SIMPLE_STARS = "KEY_SIMPLE_STARS";
    private const string KEY_DIFF_STARS = "KEY_DIFF_STARS";
    private const string KEY_GOLD = "KEY_GOLD";
    private const string KEY_OWN_TROOPS = "KEY_OWN_TROOPS";
    private const string KEY_USING_TROOPS = "KEY_USING_TROOPS";
    private const string KEY_SKILLS = "KEY_SKILLS";

    //改成 Application.persistentDataPath永久存储
    public readonly int MAX_GATE = 15;
    public static readonly PlayerManager Instance = new PlayerManager();
    // public static readonly PlayerManager Instance = LocalUtil.SharedInstance.Load<PlayerManager>(LOCAL_FILE);
    private int curGate = 0;
    private int unlockGate = 0;
    private List<int> simpleStars = new List<int>();
    private List<int> diffStars = new List<int>();
    private int gold;
    private List<int> ownTroops;
    private List<int> usingTroops;
    private List<int> skills;

    public int CurGate{get{return curGate;}}
    public int UnlockGate{get{return unlockGate;}}
    public int Gold{get{return gold;}}
    public List<int> SimpleStars{get{return simpleStars;}}
    public List<int> DiffStars{get{return diffStars;}}

    // 这里完全没有必要用DICT，因为个数一定是指定的最大个数
    public List<int> OwnTroops{get{return ownTroops;}}
    public List<int> UsingTroops{get{return usingTroops;}}
    public List<int> Skills{get{return skills;}}

    public void LoadLocalData(){
        curGate = PlayerPrefs.GetInt(KEY_CUR_GATE);
        unlockGate = PlayerPrefs.GetInt(KEY_UNLOCK_GATE);
        gold = PlayerPrefs.GetInt(KEY_GOLD);
        simpleStars = DemoUtil.String2List(PlayerPrefs.GetString(KEY_SIMPLE_STARS), MAX_GATE);
        diffStars = DemoUtil.String2List(PlayerPrefs.GetString(KEY_DIFF_STARS), MAX_GATE);

        ownTroops = DemoUtil.String2List(PlayerPrefs.GetString(KEY_OWN_TROOPS));
        usingTroops = DemoUtil.String2List(PlayerPrefs.GetString(KEY_USING_TROOPS));
        skills = DemoUtil.String2List(PlayerPrefs.GetString(KEY_SKILLS));

        // test
        // UpdateOwnTroops(TroopType.TROOP_SABER);

        Debug.Log("LOAD DATA: " +"ownTroops:"+ownTroops.Count+", usingTroops:"+usingTroops.Count+", skills:"+skills.Count + ", curGate:"+curGate+", unlockGate:"+unlockGate+", gold:"+gold+", stars:"+simpleStars.Count);
    }

    public void UpdateCurGate(int gate){
        curGate = gate;
        PlayerPrefs.SetInt(KEY_CUR_GATE, curGate);
        // Debug.Log("save curGate " + curGate);
    }

    public void UpdateUnlockGate(){
        unlockGate = Mathf.Min(Mathf.Max(curGate+1, unlockGate), MAX_GATE);
        // Debug.Log("save unlockGate " + unlockGate);
        PlayerPrefs.SetInt(KEY_UNLOCK_GATE, unlockGate);
    }

    public void UpdateStars(int star, bool isSimple){
        List<int> stars = simpleStars;
        if(!isSimple){
            stars = diffStars;
        }

        if(star > stars[curGate]){
            stars[curGate] = star;
            string key = isSimple ? KEY_SIMPLE_STARS : KEY_DIFF_STARS;
            Debug.Log("save star: " + DemoUtil.List2String(stars));
            PlayerPrefs.SetString(key, DemoUtil.List2String(stars));
        }
    }

    public void UpdateGold(int count){
        gold += count;
        Debug.Assert(gold >= 0);
        PlayerPrefs.SetInt(KEY_GOLD, gold);
    }

    public void UpdateOwnTroops(TroopType type, bool save=false){
        if(ownTroops.Count(i => i.Equals((int)type)) == 1){
            return;
        }
        ownTroops.Add((int)type);
        Debug.Assert(ownTroops.Count(i => i.Equals((int)type)) == 1);
        if(save){
            PlayerPrefs.SetString(KEY_OWN_TROOPS, DemoUtil.List2String(ownTroops));
        }
    }

    public void UpdateUsingTroops(TroopType type, bool addIn, bool save=false){
        if(addIn){
            usingTroops.Add((int)type);
            Debug.Assert(usingTroops.Count(i => i.Equals((int)type)) == 1);
        }else{
            usingTroops.Remove((int)type);
        }
        if(save){
            PlayerPrefs.SetString(KEY_USING_TROOPS, DemoUtil.List2String(usingTroops));
        }
    }

    public void UpdateSkills(SkillType type){
        if(ownTroops.Count(i => i.Equals((int)type)) == 1){
            Debug.Assert(false);
            return;
        }
        skills.Add((int)type);
        PlayerPrefs.SetString(KEY_SKILLS, DemoUtil.List2String(skills));
    }

    public void SaveTroops(){
        PlayerPrefs.SetString(KEY_OWN_TROOPS, DemoUtil.List2String(ownTroops));
        PlayerPrefs.SetString(KEY_USING_TROOPS, DemoUtil.List2String(usingTroops));
        // Debug.Log("save usingTroops: " + DemoUtil.List2String(usingTroops));
    }

    public override string ToString(){
        return "curGate:"+curGate+", unlockGate:"+unlockGate+", gold:"+gold+", stars:"+simpleStars.Count+", ownTroops:"+ownTroops.Count;
    }
}
