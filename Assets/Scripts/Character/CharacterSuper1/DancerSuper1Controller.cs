using UnityEngine;
using System.Collections;

public class DancerSuper1Controller : BaseController {
    protected override void AddTrickHarm(ref HarmModel harmModel, TroopType enemyType){
        // 直接取最大攻击力
        // 这有bug啊，每次Attack取出来都是新随机的，这里set没用啊。。。。。卧槽这怎么搞，应该放在AttackBuff里面。。
        // 不对，这里没bug把，这个是harmModel，不是角色的model
        harmModel.Attack = Model.AttackMax;
        Debug.Log(Model.Type + " max attack " + harmModel.Attack);
    }
}
