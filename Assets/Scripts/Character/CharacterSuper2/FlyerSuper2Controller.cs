using UnityEngine;
using System.Collections;
using System;

public class FlyerSuper2Controller : FlyerController {
    public override void OnSkillEvent(object sender, EventArgs e){
    	Debug.Log("Trick: " + Model.Type + " ignore any skills");
    	return;
	}

	public override void OnTrickEvent(object sender, EventArgs e){
    	Debug.Log("Trick: " + Model.Type + " ignore any tricks");
		return;
	}
}
