using UnityEngine;
using System.Collections;

public class SpyerSuper1Controller : BaseController {
    protected override void AddTrickHarm(ref HarmModel harmModel, TroopType enemyType){
        harmModel.HitRate = 1.0;
    }
}
