using UnityEngine;
using System.Collections;

public class FlyerController : BaseController {
	protected override int CalcHarm(){
		// 额外伤害，可能后面需要改成和攻击力或者啥有关的比例值吧，先给定值
		int addedHarm = 0; 
		if(Attacker.GetComponent<BaseController>().Model.Type == TroopType.TROOP_ARCHER){
			addedHarm = 10;
			Debug.Log(TroopType.TROOP_ARCHER + " addedHarm " + addedHarm + " to " + Model.Type);
		}
		return base.CalcHarm() + addedHarm;
	}

	protected override bool CanMiss(){
		if(!Attacker.GetComponent<BaseController>().IsRemoteCategory()){
			// Debug.Log("miss attacker type " + Attacker.GetComponent<BaseController>().Model.Type);
			return false;
		}else{
			return base.CanMiss();
		}
	}
}
