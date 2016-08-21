using UnityEngine;
using System.Collections;

public class SpyerSuper1Controller : BaseController {
    protected override void AddTrickHarm(ref HarmModel harmModel, TroopType enemyType){
    	Debug.Log("Trick: " + Model.Type + " hitRate 100%");
        harmModel.HitRate = 1.0;
    }
}
