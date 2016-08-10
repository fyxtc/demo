using UnityEngine;
using System.Collections;
using System;

public class FlyWeaponRocket2 : BaseFlyWeapon 
{
	private new float speed = 10.0f;
    ExplodeEvent explodeEvent = new ExplodeEvent();
    public event EventHandler ExplodeEventHandler;
	void Start () 
	{
		// speed = 3.0f;
		// Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
		Destroy(gameObject, 2);
	}

	public override float GetSpeed(){
		return this.speed;
	}

	
	void OnTriggerEnter2D (Collider2D col) 
	{
		// 避免刚Instantiate就和自己碰撞。。。。
		if(!SettingFin){
			return;
		}
		// Debug.Log(col);
		// col有可能已经gg了。。。草
		BaseController target = col.GetComponent<BaseController>();
		// 如果不是和角色碰到了，都忽略，比如火箭自己碰自己
		if(!target){
			return;
		}
		// 释放者和碰撞目标不是同一方才应该产生效果
		if(target.IsMy != IsMy){
			target.BeingAttacked(HarmModel);
			SendExplodeEvent(transform.position, 5, IsMy, 10);
			Destroy(gameObject);
			// Debug.Log("Target: " + target.IsMy + " isMy " + IsMy);
			// Debug.Log("collisions: " + (target.IsMy?"my ":"enemy ") + target.Model.Type);
		}
	}

    void SendExplodeEvent(Vector3 center, double radius, bool isMy, int harm){
        explodeEvent.Center = center;
        explodeEvent.Radius = radius;
        explodeEvent.IsMy = isMy;
        explodeEvent.Harm = harm;
        ExplodeEventHandler(this, explodeEvent);
    }
}
