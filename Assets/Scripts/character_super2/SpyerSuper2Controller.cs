using UnityEngine;
using System.Collections;

public class SpyerSuper2Controller : BaseController {
	protected override int CalcHarm(HarmModel harmModel){
        float att = harmModel.Attack;
        float def = Model.Defense;
        int res = (int)(att * (att / (att + def)));
        if(def > att){
            if(Attacker.GetComponent<BaseController>()){
                int directHarm = (int)((def - att) * 2); //双倍反弹
                HarmModel harm = new HarmModel(directHarm);
                Attacker.GetComponent<BaseController>().BeingAttacked(harm);
            }
        }
        // Debug.Log((IsMy?"my ":"enemy ") + Model.Type + " CalcHarm: " + res + "  ->  " + "att: " + att + ", def:" + def);
        Debug.Assert(res >= 0, "CalcHarm error");
        return res;
    }
}
