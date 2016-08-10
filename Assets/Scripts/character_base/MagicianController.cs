using UnityEngine;
using System.Collections;

public class MagicianController : BaseController {
    protected override void AddTrickHarm(ref HarmModel harmModel, TroopType enemyType){
    	if(enemyType == TroopType.TROOP_TITAN){
    		int addedHarm = 10;
	    	harmModel.AddedHarm += addedHarm;
			Debug.Log(Model.Type + " addedHarm " + addedHarm + " to " + enemyType);
    	}
	}
}
