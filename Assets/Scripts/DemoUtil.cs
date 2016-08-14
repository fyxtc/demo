using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class DemoUtil{
    public static string ReadConfigFile(string file){
        string res = "";
        // string path = "Assets"+Path.DirectorySeparatorChar+"Resources"/*+Path.DirectorySeparatorChar+"Config"*/+Path.DirectorySeparatorChar + file;
        string DPath = Application.dataPath;
        // int num = DPath.LastIndexOf("/");
        // DPath = DPath.Substring(0, num);
        string path = DPath + Path.DirectorySeparatorChar + file;
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
}
