using UnityEngine;
using System.Collections;

// 成远程部队了。。。
public class SaberSuper2Controller : ArcherController {
    // 伤害值是不一样的，所以不能用直接继承ArcherController，不过应该是有更好的写法的
    protected override void AddTrickHarm(ref HarmModel harmModel, TroopType enemyType){
        if(DemoUtil.IsFlyCategory(enemyType)){
            addedHarm = 10;
            harmModel.AddedHarm += addedHarm;
            Debug.Log(Model.Type + " addedHarm " + addedHarm + " to " + enemyType);
        }
    }
}
