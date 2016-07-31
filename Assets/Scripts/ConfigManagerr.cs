using System.Collections;
using LitJson;
using System.IO;
using System.Text;
using System;
using UnityEngine;

public class BaseCharacter{
    protected int id;
    protected int life;
    protected int attack;
    protected int defense;   
    protected int maxCount;
    protected int rank;
    protected int interval;
    public int Id{
        set{id = value;}
        get{return id;}
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
    public int Interval{
        set{interval = value;}
        get{return interval;}
    }
}

public class Saber : BaseCharacter{

}

public class Archer: BaseCharacter{

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
        saber.Id = (int)saberData["id"];
        saber.Life = (int)saberData["life"];
        saber.Attack = (int)saberData["attack"];
        saber.Defense = (int)saberData["defense"];
        saber.MaxCount = (int)saberData["max_count"];
        saber.Rank = (int)saberData["rank"];
        saber.Interval = (int)saberData["interval"];

        JsonData archerData = data["archer"];
        archer.Id = (int)archerData["id"];
        archer.Life = (int)archerData["life"];
        archer.Attack = (int)archerData["attack"];
        archer.Defense = (int)archerData["defense"];
        archer.MaxCount = (int)archerData["max_count"];
        archer.Rank = (int)archerData["rank"];
        archer.Interval = (int)archerData["interval"];

    }

    string readFile(){
        StreamReader sr = new StreamReader("Assets\\Config\\" + CHARACTER_CONFIG_FILE,Encoding.Default);
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
        return characterConfig;
    }

    // static void Main(string[] arg){
    //     ConfigManager.share().loadConfig();
    // }
}



