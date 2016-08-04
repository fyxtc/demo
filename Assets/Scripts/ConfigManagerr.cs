using System.Collections;
using LitJson;
using System.IO;
using System.Text;
using System;
using UnityEngine;
using System.Collections.Generic;

public class BaseModel{
    protected int type;
    protected int life;
    protected int attack;
    protected int defense;   
    protected int maxCount;
    protected int rank;
    protected double interval;
    public double AttackRange{get; set;}
    public double AttackCD{get; set;}
    public double HitRate{get; set;}
    public int Type{
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
}

public class Saber : BaseModel{

}

public class Archer: BaseModel{

}

public class CharacterConfig{
    public const string CHARACTER_CONFIG_FILE = "character_config.json";
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
        string str = ReadFile();
        JsonData data = JsonMapper.ToObject(str);

        JsonData saberData = data["saber"];
        saber.Type = (int)saberData["type"];
        saber.Life = (int)saberData["life"];
        saber.Attack = (int)saberData["attack"];
        saber.Defense = (int)saberData["defense"];
        saber.MaxCount = (int)saberData["max_count"];
        saber.Rank = (int)saberData["rank"];
        saber.Interval = (double)saberData["interval"];
        saber.AttackRange = (double)saberData["attackRange"];
        saber.AttackCD = (double)saberData["attackCD"];
		saber.HitRate = (double)saberData["hitRate"];

        JsonData archerData = data["archer"];
        archer.Type = (int)archerData["type"];
        archer.Life = (int)archerData["life"];
        archer.Attack = (int)archerData["attack"];
        archer.Defense = (int)archerData["defense"];
        archer.MaxCount = (int)archerData["max_count"];
        archer.Rank = (int)archerData["rank"];
        archer.Interval = (double)archerData["interval"];
        archer.AttackRange = (double)archerData["attackRange"];
        archer.AttackCD = (double)archerData["attackCD"];
        archer.HitRate = (double)archerData["hitRate"];

    }

    string ReadFile(){
        StreamReader sr = new StreamReader("Assets"+Path.DirectorySeparatorChar+"Config"+Path.DirectorySeparatorChar + CHARACTER_CONFIG_FILE,Encoding.Default);
        string line;
        string res = "";
        while ((line = sr.ReadLine()) != null) 
        {
            res += line;
        }
        return res;
    }
}

public class SkillModel{
	public double Attack{ get; set;}
	public double Defense{ get; set;}
	public double HitRate{ get; set;}
}

public enum SkillType{
    SKILL_ATTACK=0, SKILL_DEFENSE, SKILL_HIT
}

public class SkillConfig{
    int skillCount = 3;
    public const string SKILL_CONFIG_FILE = "skill_config.json";
    List<SkillModel> skillModels = new List<SkillModel>();

    public void LoadConfig(){
        string str = ReadFile();
        JsonData data = JsonMapper.ToObject(str);
        for(int i = 0; i < skillCount; i++){
            string key = "skill"+i;
            SkillModel model = new SkillModel();
            JsonData skillType = data[key];
            model.Attack = (double)skillType["attack"];
            model.Defense = (double)skillType["defense"];
            model.HitRate = (double)skillType["hitRate"];
            skillModels.Add(model);
        }
    }

    public SkillModel GetSkillModel(SkillType type){
        if(skillModels.Count == 0){
            LoadConfig();
        }
        return skillModels[(int)type];
    }

    string ReadFile(){
        StreamReader sr = new StreamReader("Assets"+Path.DirectorySeparatorChar+"Config"+Path.DirectorySeparatorChar + SKILL_CONFIG_FILE,Encoding.Default);
        string line;
        string res = "";
        while ((line = sr.ReadLine()) != null) 
        {
            res += line;
        }
        return res;
    }
}

public class ConfigManager{
    public static ConfigManager instance;
	public CharacterConfig CharacterConfig{ get; set;}
	public SkillConfig SkillConfig{ get; set;}
    bool isLoaded = false;

    public ConfigManager(){
        CharacterConfig = new CharacterConfig();
        SkillConfig = new SkillConfig();
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
        }
    }


    // static void Main(string[] arg){
    //     ConfigManager.share().loadConfig();
    // }
}



