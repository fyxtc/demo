using UnityEngine;
using System.Collections;

public class RecoverSuper1Controller : BaseController {
    protected override void AddTrickHarm(ref HarmModel harmModel, TroopType enemyType){
        // 先做成对任何部队的伤害值提高吧。。。防御还要有时间？不然和自己攻击提高没啥区别吧。。   
        harmModel.Attack += 10;
    }
}
