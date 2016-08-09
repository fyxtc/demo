using UnityEngine;
using System.Collections;

public class RiderController : BaseController {
	private double missBuff = 0.5;
	protected override bool CanMiss(){
        // 对啊卧槽，如果对方已经死了，这里怎么取啊卧槽。。。难道还要把对象放到事件发过来吗。。日
    	double hitRate = Attacker.GetComponent<BaseController>().Model.HitRate;
    	if(Status == TroopStatus.STATUS_FORWARD || Status == TroopStatus.STATUS_BACK){
    		hitRate = hitRate * (1.0 - missBuff);
	    	// Debug.Log("miss buff hitRate: " + hitRate);
    	}
        return UnityEngine.Random.value > hitRate; 
    }
}
