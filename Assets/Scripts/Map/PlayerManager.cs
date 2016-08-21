using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class PlayerManager {
    private const string KEY_CUR_GATE = "KEY_CUR_GATE";
    private const string KEY_UNLOCK_GATE = "KEY_UNLOCK_GATE";
    private const string KEY_SIMPLE_STARS = "KEY_SIMPLE_STARS";
    private const string KEY_DIFF_STARS = "KEY_DIFF_STARS";
    private const string KEY_GOLD = "KEY_GOLD";
    private const string KEY_TROOPS = "KEY_TROOPS";

    //改成 Application.persistentDataPath永久存储
    public readonly int MAX_GATE = 15;
    public static readonly PlayerManager Instance = new PlayerManager();
    // public static readonly PlayerManager Instance = LocalUtil.SharedInstance.Load<PlayerManager>(LOCAL_FILE);
    private int curGate = 0;
    private int unlockGate = 0;
    private List<int> simpleStars = new List<int>();
    private List<int> diffStars = new List<int>();
    private int gold;
    private Dictionary<TroopType, int> troops;

    public int CurGate{get{return curGate;}}
    public int UnlockGate{get{return unlockGate;}}
    public int Gold{get{return gold;}}
    public List<int> SimpleStars{get{return simpleStars;}}
    public List<int> DiffStars{get{return diffStars;}}
    public Dictionary<TroopType, int> Troops{get{return troops;}}

    public void LoadLocalData(){
        curGate = PlayerPrefs.GetInt(KEY_CUR_GATE);
        unlockGate = PlayerPrefs.GetInt(KEY_UNLOCK_GATE);
        gold = PlayerPrefs.GetInt(KEY_GOLD);
        simpleStars = DemoUtil.String2List(PlayerPrefs.GetString(KEY_SIMPLE_STARS), MAX_GATE);
        diffStars = DemoUtil.String2List(PlayerPrefs.GetString(KEY_DIFF_STARS), MAX_GATE);

        Debug.Log("LOAD DATA: " + "curGate:"+curGate+", unlockGate:"+unlockGate+", gold:"+gold+", stars:"+simpleStars.Count);
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

    public void UpdateTroops(TroopType type){
        troops.Add(type, ConfigManager.share().CharacterConfig.GetModel(type).MaxCount);
    }

    public override string ToString(){
        return "curGate:"+curGate+", unlockGate:"+unlockGate+", gold:"+gold+", stars:"+simpleStars.Count+", troops:"+troops.Count;
    }
}
