using UnityEngine;
using System.Collections;

public class RecoverSuper2Controller : RecoverController {
    protected override void AddSkillLifeBuff(bool isStart){
        base.AddSkillLifeBuff(isStart);
        if(isStart){
            model.Life += 10; // 瞬间加上
            BaseModel config = characterConfig.GetModel(model.Type);
            // 瞬间上限，如果被打下来之后就还是走config的life
            int newLifeMax = config.Life + 10;
            if(model.Life > newLifeMax){
                model.Life = newLifeMax;
            }
        }
    }
}
