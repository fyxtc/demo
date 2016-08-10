using UnityEngine;
using System.Collections;

public class FlyerController : BaseController {
	protected override bool CanMiss(HarmModel harmModel){
		if(!DemoUtil.IsRemoteCategory(harmModel.Type)){
			// Debug.Log("miss attacker type " + Attacker.GetComponent<BaseController>().Model.Type);
			return false;
		}else{
			return base.CanMiss(harmModel);
		}
	}
}
