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
        double rate = 0.5;
        bool res = UnityEngine.Random.value >= rate; 
        return res;
    }
}
