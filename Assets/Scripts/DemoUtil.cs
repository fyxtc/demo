using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class DemoUtil{
    public static string ReadConfigFile(string file){
        StreamReader sr = new StreamReader("Assets"+Path.DirectorySeparatorChar+"Config"+Path.DirectorySeparatorChar + file,Encoding.Default);
        string line;
        string res = "";
        while ((line = sr.ReadLine()) != null) 
        {
            res += line;
        }
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
        bool res = (type == TroopType.TROOP_ARCHER || type == TroopType.TROOP_ARCHER_SUPER1 || type == TroopType.TROOP_ARCHER_SUPER2);
        return res;
    }

    public static bool IsAttackIgnoreType(TroopType attackerType, TroopType enemyType){
        // 近战都打不到飞行兵
        if(!DemoUtil.IsRemoteCategory(attackerType) && DemoUtil.IsFlyCategory(enemyType)){
            // Debug.Log("ingore " + attackerType + " to " + enemyType);
            return true;
        }
        return false;
    }
}
