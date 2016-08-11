using UnityEngine;
using System.Collections;

public class BaseFlyWeapon : MonoBehaviour {
	// 这里不能用public在editor指定，因为这样取到的属性都是config的属性，没有buff了
	public BaseController Owner{get; set;}
	public float speed = 20.0f;
	public bool IsMy{get; set;}
	public bool SettingFin{get; set;}
	public HarmModel HarmModel{get; set;}

	public virtual float GetSpeed(){
		return speed;
	}


	void Start () 
	{
		// Debug.Log("fire !!");
		// speed = 3.0f;
		// Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
		Destroy(gameObject, 2);
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
		if(target.IsMy != IsMy && !target.IsZombie && !target.IsDead){
			target.BeingAttacked(HarmModel);
			Destroy(gameObject);
			TriggerCallback();
		}
	}

	protected virtual void TriggerCallback(){

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
