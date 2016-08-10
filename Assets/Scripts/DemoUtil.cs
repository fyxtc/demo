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
}
