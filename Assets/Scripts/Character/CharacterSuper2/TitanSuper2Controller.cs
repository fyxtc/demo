using UnityEngine;
using System.Collections;

public class TitanSuper2Controller : TitanController {
	protected override void AddTrickHarm(ref HarmModel harmModel, TroopType enemyType){
		if(DemoUtil.IsRemoteCategory(enemyType)){
			Debug.Log("Trick: " + Model.Type + " improve hitRate to remote type " + enemyType);
			harmModel.HitRate = Mathf.Max(1.0f, (int)(harmModel.HitRate+0.1));
		}
	}

}
