using System.Collections;
using LitJson;
using System.Text;
using System;
using UnityEngine;
using System.Collections.Generic;

public class BaseModel : ICloneable{
    // public just for litjson mapper
    public TroopType type; // 靠，这里厉害了，直接在饮食的时候int强转enum了
    public int life;
    public int attackMin;
    public int attackMax;
    public int defense;   
    public double speed;
    public int maxCount;
    public int rank;
    public double interval;
    public double attackRange;
    public double attackCD;
    public double hitRate;
    public int[] tricks;

    public TroopType Type{
        set{type = value;}
        get{return type;}
    } 
    public int Life{
        set{life = value;}
        get{return life;}
    } 
    public int AttackMin{
        set{attackMin = value;}
        get{return attackMin;}
    }
    public int AttackMax{
        set{attackMax = value;}
        get{return attackMax;}
    }
    public int Attack{
        get{return (int)(UnityEngine.Random.Range(attackMin, attackMax));}
    }
    public int Defense{
        set{defense = value;}
        get{return defense;}
    }
    public double Speed{
        set{speed = value;}
        get{return speed;}
    }
    public int MaxCount{
        set{maxCount = value;}
        get{return maxCount;}
    }
    public int Rank{
        set{rank = value;}
        get{return rank;}
    }
    public double Interval{
        set{interval = value;}
        get{return interval;}
    }
    public double AttackRange{
        set{attackRange = value;}
        get{return attackRange;}
    }
    public double AttackCD{
        set{attackCD = value;}
        get{return attackCD;}
    }
    public double HitRate{
        set{hitRate = value;}
        get{return hitRate;}
    }
    public int[] Tricks{
        set{tricks = value;}
        get{return tricks;}
    }


    public object Clone()
    {
        return this.MemberwiseClone();
    }

    public override string ToString(){
		return "type:" + type + ", attackMin:" + attackMin + ", attackMax:" + attackMax + ", defense:" + defense + ", speed:" + speed + ", life:" + life + ", maxCount:" + maxCount + ", rank:" + rank + ", interval:" + interval + ", attackRange:" + attackRange + ", attackCD:" + attackCD + ", hitRate:" + hitRate + ", tricks:" + tricks.Length;
    }
}

public enum TroopType{
    TROOP_SABER=0, TROOP_ARCHER, TROOP_RECOVER, TROOP_DANCER, TROOP_SPYER, TROOP_RIDER, TROOP_FLYER, TROOP_MAGICICAN, TROOP_TITAN
    ,TROOP_SABER_SUPER1, TROOP_ARCHER_SUPER1, TROOP_RECOVER_SUPER1, TROOP_DANCER_SUPER1, TROOP_SPYER_SUPER1, TROOP_RIDER_SUPER1, TROOP_FLYER_SUPER1, TROOP_MAGICICAN_SUPER1, TROOP_TITAN_SUPER1
    ,TROOP_SABER_SUPER2, TROOP_ARCHER_SUPER2, TROOP_RECOVER_SUPER2, TROOP_DANCER_SUPER2, TROOP_SPYER_SUPER2, TROOP_RIDER_SUPER2, TROOP_FLYER_SUPER2, TROOP_MAGICICAN_SUPER2, TROOP_TITAN_SUPER2
    ,TROOP_SERVANT, TROOP_ALL=100, TROOP_SELF=-1,
}

public enum TroopCategory{
    CATEGORY_LAND, CATEGORY_FLY, CATEGORY_NEAR, CATEGORY_REMOTE,
}

public class Saber : BaseModel{

}

public class Archer: BaseModel{

}

public class CharacterConfig{
    public const string CHARACTER_CONFIG_FILE = "character_config.json";
    public BaseModel[] models;
    public void LoadConfig(){
        string str = DemoUtil.ReadConfigFile(CHARACTER_CONFIG_FILE);
		models = JsonMapper.ToObject<BaseModel[]>(str);
        // foreach (BaseModel m in models){
        //     Debug.Log(m);
        // }
    }

    public BaseModel GetModel(TroopType type){
        return models[(int)type];
    }
}

public enum SkillStatus{
    STATUS_IDLE, STATUS_USING, STATUS_STOP,
}

public class SkillModel{
    public SkillType type;
    public double attack;
    public double defense;
    public double hitRate;
    public int life;
    public double attackRange;
    public double time;
    public double cd;

    public SkillType Type{
        get{return type;}
        set{type = value;}
    }
	public double Attack{ 
        get{return attack;} 
        set{attack = value;}
    }
	public double Defense{ 
        get{return defense;} 
        set{defense = value;}
    }
	public double HitRate{ 
        get{return hitRate;} 
        set{hitRate = value;}
    }
    public int Life{ 
        get{return life;} 
        set{life = value;}
    }    
    public double AttackRange{
        get{return attackRange;}
        set{attackRange = value;}
    }
    public double Time{ 
        get{return time;} 
        set{time = value;}
    }
    public double CD{ 
        get{return cd;} 
        set{cd = value;}
    }

    public override string ToString(){
        return "type:"+Type+", attack:"+attack+", defense:"+defense+", hitRate:"+hitRate+", life"+life+", attackRange"+attackRange+", time:"+time+", cd"+cd;
    }
}

public enum SkillType{
    SKILL_ATTACK=0, SKILL_DEFENSE, SKILL_HIT, SKILL_LIFE, SKILL_THROW
    , SKILL_INVALID=-1
}

public class SkillEvent : EventArgs{
    public SkillType Type{get; set;}
    public SkillStatus Status{get; set;}
    public bool IsMy{get; set;}
}

public class SkillConfig{
    public const string SKILL_CONFIG_FILE = "skill_config.json";
    private SkillModel[] models;

    public void LoadConfig(){
        string str = DemoUtil.ReadConfigFile(SKILL_CONFIG_FILE);
        models = JsonMapper.ToObject<SkillModel[]>(str);
        // foreach (SkillModel m in models){
        //     Debug.Log(m);
        // }
    }

    public SkillModel GetModel(SkillType type){
        return models[(int)type];
    }
}


public enum TrickProperty{
    PROPERTY_ATTACK=0, PROPERTY_DEFENSE, PROPERTY_SPEED, PROPERTY_HIT, PROPERTY_LIFE, PROPERTY_ATTACK_CD
}

public enum TrickType{
    TRICK_SKILL=0, TRICK_STATUS, TRICK_NATURAL, TRICK_INVALID=-1
}

// 这和TroopStatus不完全一样，所以要重新定义
public enum TrickStatusType{
    STATUS_IDLE=0, STATUS_MOVE, STATUS_ATTACK, STATUS_DEFENSE, STATUS_DEAD, STATUS_INVALID=-1
}

public class TrickModel{
    // public just for litjson mapper...orz
    public int id;
    public TrickType type;
    public TrickProperty property;
    public double effect;
    public double rate;
    public TroopType[] target;
    public bool isSelf;
    public TroopType[] opponent;
    // skill 和status都放在base里，这样方便映射，默认为无效值即可，这样该有值的就自己复写了，问题在于现在要怎么转换，或者直接在config再根据type区分list出来，虽然统一读取的
    public SkillType skill = SkillType.SKILL_INVALID;
    public TrickStatusType status = TrickStatusType.STATUS_INVALID;

    public int Id{
		get{return id;}
        set{id = value;}
    }
    public TrickType Type{
		get{return type;}
        set{type = value;}
    }
    public TrickProperty Property{
		get{return property;}
		set{property = value;;}
    }
    public double Effect{
		get{return effect;}
        set{effect = value;}
    }
    public double Rate{
		get{return rate;}
        set{rate = value;}
    }
    public TroopType[] Target{
		get{return target;}
        set{target = value;}
    }
    public SkillType Skill{
        get{return skill;}
        set{skill = value;}
    }
    public TrickStatusType Status{
        get{return status;}
        set{status = value;}
    }
    public bool IsSelf{
        get{return isSelf;}
        set{isSelf = value;}
    }
    public TroopType[] Opponent{
        get{return opponent;}
        set{opponent = value;}
    }

    public override string ToString(){
        return "id:"+id+", type:"+type+", property:"+property+", effect:"+effect+", rate:"+rate+", target:"+target.Length
               +", skill:"+skill+", status:"+status+", opponentCount:"+Opponent.Length;
    }
}

public class SkillTrickModel : TrickModel{

}

public class StatusTrickModel : TrickModel{

}

public class TrickEvent : EventArgs{
    public int[] Tricks{get; set;}
    public bool IsStart{get; set;}
    public bool IsSelf{get; set;}
    public bool IsMy{get; set;}
}

public class TrickConfig{
    public const string TRICK_CONFIG_FILE = "trick_config.json";
    private TrickModel[] models;
    public void LoadConfig(){
        string str = DemoUtil.ReadConfigFile(TRICK_CONFIG_FILE);
        models = JsonMapper.ToObject<TrickModel[]>(str);
        // foreach (TrickModel m in models){
        //     Debug.Log(m);
        // }
    }

    public TrickModel GetModel(int id){
        return models[id];
    }

}

public class ConfigManager{
    public static ConfigManager instance;
	public CharacterConfig CharacterConfig{ get; set;}
	public SkillConfig SkillConfig{ get; set;}
    public TrickConfig TrickConfig{ get; set;}
    bool isLoaded = false;

    public ConfigManager(){
        CharacterConfig = new CharacterConfig();
        SkillConfig = new SkillConfig();
        TrickConfig = new TrickConfig();
        LoadConfig();
    }

    public static ConfigManager share(){
		if(instance == null){
            instance = new ConfigManager();
        }
        return instance;
    }
    public void LoadConfig(){
        if(!isLoaded){
            isLoaded = true;
            CharacterConfig.LoadConfig();
            SkillConfig.LoadConfig();
            TrickConfig.LoadConfig();
        }
    }


    // static void Main(string[] arg){
    //     ConfigManager.share().loadConfig();
    // }
}


public class ExplodeEvent : EventArgs{
	public Vector3 Center{ get; set; }
	public double Radius{ get; set; }
    public bool IsMy{ get; set; }
    public int Harm{ get; set; }

    public override string ToString(){
        return "center:"+ Center + ", radisu:" + Radius + ", isMy:" + IsMy + ", harm:" + Harm;
    }
}

public class HarmModel{
    public int DirectHarm{get; set;}
    public int AddedHarm{get; set;}
    public TroopType Type{get; set;}
    public int Attack{get; set;}
    public double HitRate{get; set;}
    public bool IsCritical{get; set;} // default false

    public HarmModel(TroopType type, int attack, double hitRate){
        DirectHarm = -1;
        Type = type;
        Attack = attack;
        HitRate = hitRate;
    }

    public HarmModel(int directHarm){
        DirectHarm = directHarm;
    }

    public override string ToString(){
        return "DirectHarm:"+DirectHarm+", AddedHarm:"+AddedHarm+ ", Type:"+Type+", Attack:"+Attack+", HitRate:"+HitRate+", IsCritical:"+IsCritical;
    }
}

public class SummonEvent : EventArgs{
    public TroopType Type{get; set;}
}






