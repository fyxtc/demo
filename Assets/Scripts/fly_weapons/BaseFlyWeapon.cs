using UnityEngine;
using System.Collections;

public class BaseFlyWeapon : MonoBehaviour {
	// 这里不能用public在editor指定，因为这样取到的属性都是config的属性，没有buff了
	public BaseController Owner{get; set;}
	public float speed = 20.0f;
	public bool IsMy{get; set;}
	public bool SettingFin{get; set;}
	public HarmModel HarmModel{get; set;}
	public GameObject Target{get; set;}

	public virtual float GetSpeed(){
		return speed;
	}


	void Start () 
	{
		// Debug.Log("fire !!");
		// speed = 3.0f;
		// Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
		// Destroy(gameObject, 2);
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
	
	// 默认带跟踪效果，如果有设置目标的话
	protected virtual void Update () {
		if(Target){
			Vector3 targetPos = Target.transform.position;
			Vector3 myPos = transform.position;
			// Debug.Log(myPos + " -> " + targetPos);
   //          transform.position = Vector3.Lerp(myPos, targetPos, (float)(1/((myPos - targetPos).magnitude) * GetSpeed()));
			float disX = Mathf.Abs (targetPos.x - myPos.x);
			float disY = Mathf.Abs (targetPos.y - myPos.y);
			float angle = Mathf.Atan2 (disY, disX);
			transform.Rotate(new Vector3(0, 0, IsMy ? angle : -angle));

			float spd = 1;
			float forceX = spd;// 取现在的角度，然后飞出屏幕即可
			float forceY = Mathf.Atan(transform.rotation.eulerAngles.z) * spd;
			Vector2 force = new Vector2(forceX, forceY);
			// Debug.Log("force: " + force);
			GetComponent<Rigidbody2D> ().velocity = force;
		}else{
		}
	}
}
