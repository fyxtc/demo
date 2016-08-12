using UnityEngine;
using System.Collections;
using System;

public class MagicianSuper2Controller : BaseController {
	public void Summon(object sender, EventArgs e){
		SummonEvent ev = e as SummonEvent;
		Debug.Log("Trick: " + Model.Type + " summon by " + ev.Type + " dead");
	}
}
