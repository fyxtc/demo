using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class DemoUtil{
    public static string ReadConfigFile(string file){
        string res = "";
        // win
        string path = "Assets"+Path.DirectorySeparatorChar+Path.DirectorySeparatorChar+"Config"+Path.DirectorySeparatorChar + file;
        
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

    public static List<int> String2List(string str){
        List<string> l1 = new List<string>(str.Split(','));
        // Debug.Log(string.Join(",", l1.ToArray()));
        // Debug.Log(l1.Count + ":1" + l1[0] + "1" + (l1[0] == "" ? 0 : int.Parse(l1[0])));
        if(l1.Count > 0){
            List<int> res = l1.ConvertAll<int>(x => x == "" ? 0 : int.Parse(x));
            return res;
        }else{
            return default(List<int>);
        }
    }

    // public static string Dict2String(Dictionary<TroopType, int> dictionary){
    //     var res = string.Join(";", dictionary);
    //     return res;
    // }
}
