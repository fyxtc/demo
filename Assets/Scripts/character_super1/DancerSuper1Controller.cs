using UnityEngine;
using System.Collections;

public class DancerSuper1Controller : BaseController {
    protected override void AddTrickHarm(ref HarmModel harmModel, TroopType enemyType){
        // 直接取最大攻击力
        harmModel.Attack = Model.AttackMax;
    }
}
