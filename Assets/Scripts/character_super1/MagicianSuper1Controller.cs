using UnityEngine;
using System.Collections;

public class MagicianSuper1Controller : MagicianController {
    protected override void AddTrickHarm(ref HarmModel harmModel, TroopType enemyType){
    	base.AddTrickHarm(ref harmModel, enemyType);
		harmModel.IsCritical = CanCritical();

    }

    bool CanCritical(){
    	float cirticalRate = 0.5f;
    	bool res = UnityEngine.Random.value <= cirticalRate;
    	if(res){
    		Debug.Log("Trick: " + Model.Type + " Critical Attack");
    	}
    	return res;
    }
}
