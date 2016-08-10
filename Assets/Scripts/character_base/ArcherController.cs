using UnityEngine;
using System.Collections;
using Spine.Unity;

public class ArcherController : BaseController {
    public float flyWeaponSpeed = 20.0f; 
    protected override void AddTrickHarm(ref HarmModel harmModel, TroopType enemyType){
    	if(DemoUtil.IsFlyCategory(enemyType)){
    		int addedHarm = 10;
	    	harmModel.AddedHarm += addedHarm;
			Debug.Log(Model.Type + " addedHarm " + addedHarm + " to " + enemyType);
    	}
	}
}

