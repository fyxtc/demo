using System.Collections;
using LitJson;
using System.IO;
using System.Text;
using System;
using UnityEngine;

public class BaseModel{
    protected int type;
    protected int life;
    protected int attack;
    protected int defense;   
    protected int maxCount;
    protected int rank;
    protected double interval;
    public double AttackRange{get; set;}
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

    public void loadConfig(){
        string str = readFile();
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

        JsonData archerData = data["archer"];
        archer.Type = (int)archerData["type"];
        archer.Life = (int)archerData["life"];
        archer.Attack = (int)archerData["attack"];
        archer.Defense = (int)archerData["defense"];
        archer.MaxCount = (int)archerData["max_count"];
        archer.Rank = (int)archerData["rank"];
        archer.Interval = (double)archerData["interval"];
        archer.AttackRange = (double)archerData["attackRange"];

    }

    string readFile(){
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

public class ConfigManager{
    public static ConfigManager instance;
    private CharacterConfig characterConfig = new CharacterConfig();
    bool isLoaded = false;

    public static ConfigManager share(){
		if(instance == null){
            instance = new ConfigManager();
        }
        return instance;
    }
    public void loadConfig(){
        if(!isLoaded){
            isLoaded = true;
            characterConfig.loadConfig();
        }
    }
    public CharacterConfig getCharacterConfig(){
        loadConfig();
        return characterConfig;
    }

    // static void Main(string[] arg){
    //     ConfigManager.share().loadConfig();
    // }
}



