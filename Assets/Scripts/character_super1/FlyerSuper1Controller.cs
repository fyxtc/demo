using UnityEngine;
using System.Collections;

public class FlyerSuper1Controller : FlyerController {
	private int attackCount = 0;
    protected override void Attack(){
    	if(IsDead){
    		// 如果死了，二次攻击当然就不能有了
    		return;
    	}
    	Debug.Log("Attack");
        Status = TroopStatus.STATUS_ATTACK;
        spineAnimationState.SetAnimation(0, shootAnimationName, false);
        AttackedTarget.GetComponent<BaseController>().Attacker = this.gameObject;
        HarmModel harmModel = CreateHarmModel(AttackedTarget.GetComponent<BaseController>().Model.Type);
        if(!IsNeedFlyWeapon()){
            AttackedTarget.GetComponent<BaseController>().BeingAttacked(harmModel);
        }else{
            CreateFlyWeapon(harmModel);
        }

        if(attackCount == 1){
        	Debug.Log("Trick: " + Model.Type + " double attack");
		    Invoke("DoBackDelay", 0.5f); 
        }else{
        	// 如果在下次攻击之前死了怎么办？ 这个invoke还会调吗，test测试了下是不会的。。
        	attackCount++;
		    Invoke("Attack", 0.5f); 
        }
    }

}
