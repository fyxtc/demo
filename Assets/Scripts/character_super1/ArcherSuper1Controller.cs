using UnityEngine;
using System.Collections;

public class ArcherSuper1Controller : ArcherController {
    protected override void AddTrickHarm(ref HarmModel harmModel, TroopType enemyType){
        if(CanAddHarm()){
            addedHarm = 11;
            harmModel.AddedHarm += addedHarm;
            Debug.Log(Model.Type + " addedHarm " + addedHarm + " to " + enemyType);
        }
    }

    private bool CanAddHarm(){
        double rate = 0.5; // 0 得给-1，因为value范围[0,1]
        bool res = UnityEngine.Random.value <= rate; 
        return res;
    }
}
