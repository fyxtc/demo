using UnityEngine;
using System.Collections;

public class SpyerController : BaseController {
	protected override void Dead(){
		base.Dead();
		Debug.Log("bang");
	}
}
