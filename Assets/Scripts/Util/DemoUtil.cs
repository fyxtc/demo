using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class DemoUtil{
    public static string ReadConfigFile(string file){
        string res = "";
        // win
        // string path = "Assets"+Path.DirectorySeparatorChar+Path.DirectorySeparatorChar+"Config"+Path.DirectorySeparatorChar + file;
        string path = file;
        Debug.Log("load file: " + path);
        
        // mac
        // string DPath = Application.dataPath;
        // string path = DPath + Path.DirectorySeparatorChar + "Config" + Path.DirectorySeparatorChar + file;

        // lcy
        // string path = "D:\\unity_workspace\\Demo\\exe\\demo_Data\\Config\\" + file;
        
        // 第一种
        StreamReader sr = new StreamReader(path, Encoding.Default);
        string line;
        while ((line = sr.ReadLine()) != null) 
        {
            res += line;
        }

        // 第二种
        // res = ((TextAsset)Resources.Load(file)).text; // Resources里面的文件夹不行，只能直接放，不能嵌套

        // 第三种
        // string sPath= Application.streamingAssetsPath + Path.DirectorySeparatorChar + file;
        // WWW www = new WWW(sPath);
        // res = www.text;

        // 第四种

        // Debug.Log(res);
        return res;
    }

    public static TroopCategory GetTroopCategory(TroopType type){
        if(type == TroopType.TROOP_FLYER || type == TroopType.TROOP_SABER_SUPER1){
            return TroopCategory.CATEGORY_FLY;
        }else{
            return TroopCategory.CATEGORY_LAND;
        }
    }

    public static bool IsFlyCategory(TroopType type){
        bool res = (type == TroopType.TROOP_FLYER || type == TroopType.TROOP_FLYER_SUPER1 || type == TroopType.TROOP_FLYER_SUPER2
                    || type == TroopType.TROOP_SABER_SUPER1);
        return res;
    }

    public static bool IsRemoteCategory(TroopType type){
        bool res = (type == TroopType.TROOP_ARCHER || type == TroopType.TROOP_ARCHER_SUPER1 || type == TroopType.TROOP_ARCHER_SUPER2
                    || type == TroopType.TROOP_SABER_SUPER2 || type == TroopType.TROOP_TITAN_SUPER2
                    || type == TroopType.TROOP_MAGICICAN || type == TroopType.TROOP_MAGICICAN_SUPER1 || type == TroopType.TROOP_MAGICICAN_SUPER2);
        return res;
    }

    public static bool IsNearCategory(TroopType type){
        return (!DemoUtil.IsFlyCategory(type) && !DemoUtil.IsRemoteCategory(type));
    }

    public static bool IsAttackIgnoreType(TroopType attackerType, TroopType enemyType){
        // 近战都打不到飞行兵
        if(!DemoUtil.IsRemoteCategory(attackerType) && DemoUtil.IsFlyCategory(enemyType)){
            // Debug.Log("ingore " + attackerType + " to " + enemyType);
            return true;
        }else if(DemoUtil.IsFlyCategory(attackerType) && DemoUtil.IsFlyCategory(enemyType)){
            return true;
        }
        return false;
    }

    public static string List2String(List<int> l1){
        List<string> l2 = l1.ConvertAll<string>(x => x.ToString());
        string res = string.Join(",", l2.ToArray());
        return res;
    }

    public static List<string> ListInt2ListString(List<int> l1){
        // List<string> l2 = l1.ConvertAll<string>(delegate(int i) { return i.ToString(); });
        List<string> l2 = l1.ConvertAll<string>(x => x.ToString());
        return l2;
    }

    public static List<int> String2List(string str, int initZeroCount=0, int initValue=0){
        List<string> l1 = new List<string>(str.Split(','));
        // Debug.Log(string.Join(",", l1.ToArray()));
        // Debug.Log(l1.Count + ":1" + l1[0] + "1" + (l1[0] == "" ? 0 : int.Parse(l1[0])));
        if(l1[0] != ""){
            List<int> res = l1.ConvertAll<int>(x => x == "" ? 0 : int.Parse(x));
            return res;
        }else{
            List<int> res = new List<int>();
            for(int i = 0; i < initZeroCount; i++){
                res.Add(initValue);
            }
            return res;
        }
    }

    public static HashSet<TroopType> String2Set(string str){
        List<int> l1 = DemoUtil.String2List(str);
        List<TroopType> res = l1.ConvertAll<TroopType>(x => (TroopType)x);
        return new HashSet<TroopType>(res);
    }

    public static TroopType GetSuper1(TroopType type){
        return (TroopType)((int)type + 9*1); // 基础兵9种
    }

    public static TroopType GetSuper2(TroopType type){
        return (TroopType)((int)type + 9*2); // 基础兵9种
    }

    public static TroopType GetBase(TroopType type){
        int t = (int)type;
        int baseCount = 9; // todo: move to configmanager
        if(t <= (int)TroopType.TROOP_TITAN){
            return type;
        }else if(t <= (int)TroopType.TROOP_TITAN_SUPER1){
            return (TroopType)(t - baseCount);
        }else if(t <= (int)TroopType.TROOP_TITAN_SUPER2){
            return (TroopType)(t - baseCount*2);
        }else{
            Debug.Assert(false);
            return TroopType.TROOP_SERVANT;
        }
    }


    // public static string Dict2String(Dictionary<TroopType, int> dictionary){
    //     var res = string.Join(";", dictionary);
    //     return res;
    // }
}
