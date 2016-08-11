using UnityEngine;
using System.Collections;
using System;

public class FlyWeaponRocket2 : BaseFlyWeapon 
{
	private new float speed = 10.0f;
    ExplodeEvent explodeEvent = new ExplodeEvent();
    public event EventHandler ExplodeEventHandler;

	public override float GetSpeed(){
		return this.speed;
	}
	
	protected override void TriggerCallback(){
		SendExplodeEvent(transform.position, 5, IsMy, 100);
	}

    void SendExplodeEvent(Vector3 center, double radius, bool isMy, int harm){
        explodeEvent.Center = center;
        explodeEvent.Radius = radius;
        explodeEvent.IsMy = isMy;
        explodeEvent.Harm = harm;
        ExplodeEventHandler(this, explodeEvent);
    }
}
