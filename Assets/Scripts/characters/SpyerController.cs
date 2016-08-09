using UnityEngine;
using System.Collections;
using System;


public class SpyerController : BaseController {

	protected override void Dead(){
		base.Dead();
		SendExplodeEvent(transform.position, 10, IsMy, 100);
	}
}
