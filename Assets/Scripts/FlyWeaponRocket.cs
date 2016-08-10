﻿using UnityEngine;
using System.Collections;

public class FlyWeaponRocket : BaseFlyWeapon 
{
	private new float speed = 10.0f;
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
			Destroy(gameObject);
			// Debug.Log("Target: " + target.IsMy + " isMy " + IsMy);
			// Debug.Log("collisions: " + (target.IsMy?"my ":"enemy ") + target.Model.Type);
		}
	}
}
