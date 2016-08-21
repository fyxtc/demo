using UnityEngine;
using System.Collections;

public class TitanSuper1Controller : TitanController {
	protected override void AddTrickHarm(ref HarmModel harmModel, TroopType enemyType){
		if(isBuffing && skillBuffModel.Type == SkillType.SKILL_ATTACK){
			Debug.Log("Trick: " + Model.Type + " attack & hitRate");
			harmModel.Attack += 10;
			harmModel.HitRate = Mathf.Max(1.0f,(int)(harmModel.HitRate+0.1));
		}
	}	
}
