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
    public int attack;
    public int defense;   
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
    public int Attack{
        set{attack = value;}
        get{return attack;}
    }
    public int Defense{
        set{defense = value;}
        get{return defense;}
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
		return "type:" + type + ", attack:" + attack + ", defense:" + defense + ", maxCount:" + maxCount + ", rank:" + rank + ", interval:" + interval + ", attackRange:" + attackRange + ", attackCD:" + attackCD + ", hitRate:" + hitRate + ", tricks:" + tricks.Length;
    }
}

public enum TroopType{
    TROOP_SABER=0, TROOP_ARCHER=1, TROOP_ALL=100
}

public class Saber : BaseModel{

}

public class Archer: BaseModel{

}

public class CharacterConfig{
    public const string CHARACTER_CONFIG_FILE = "character_config.json";
    public BaseModel[] models;
    private Saber saber = new Saber();
    private Archer archer = new Archer();
    public Saber Saber{
        set{saber = value;}
        get{return saber;}
    }
    public Archer Archer{
        set{archer = value;}
        get{return archer;}
    }

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
	public double Attack{ get; set;}
	public double Defense{ get; set;}
	public double HitRate{ get; set;}
    public double Time{ get; set;}
    public double CD{ get; set;}
}

public enum SkillType{
    SKILL_ATTACK=0, SKILL_DEFENSE, SKILL_HIT, SKILL_INVALID=-1
}

public class SkillEvent : EventArgs{
    public SkillType Type{get; set;}
    public SkillStatus Status{get; set;}
    public bool IsMy{get; set;}
}

public class SkillConfig{
    int skillCount = 3;
    public const string SKILL_CONFIG_FILE = "skill_config.json";
    List<SkillModel> skillModels = new List<SkillModel>();

    public void LoadConfig(){
        string str = DemoUtil.ReadConfigFile(SKILL_CONFIG_FILE);
        JsonData data = JsonMapper.ToObject(str);
        for(int i = 0; i < skillCount; i++){
            string key = "skill"+i;
            SkillModel model = new SkillModel();
            JsonData skillType = data[key];
            model.Attack = (double)skillType["attack"];
            model.Defense = (double)skillType["defense"];
            model.HitRate = (double)skillType["hitRate"];
            model.Time = (double)skillType["time"];
            model.CD = (double)skillType["cd"];
            skillModels.Add(model);
        }
    }

    public SkillModel GetSkillModel(SkillType type){
        if(skillModels.Count == 0){
            LoadConfig();
        }
        return skillModels[(int)type];
    }

}


public enum TrickProperty{
    PROPERTY_ATTACK=0, PROPERTY_DEFENSE, PROPERTY_SPEED, PROPERTY_HIT, PROPERTY_LIFE,
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

    public override string ToString(){
        return "id:"+id+", type:"+type+", property:"+property+", effect:"+effect+", rate:"+rate+", target:"+target.Length+", skill:"+skill+", status:"+status;
    }
}

public class SkillTrickModel : TrickModel{

}

public class StatusTrickModel : TrickModel{

}

public class TrickEvent : EventArgs{
    public int[] Tricks{get; set;}
    public bool IsStart{get; set;}
}

public class TrickConfig{
    public const string TRICK_CONFIG_FILE = "trick_config.json";
    private TrickModel[] models;
    public void LoadConfig(){
        string str = DemoUtil.ReadConfigFile(TRICK_CONFIG_FILE);
        JsonData data = JsonMapper.ToObject(str);
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



