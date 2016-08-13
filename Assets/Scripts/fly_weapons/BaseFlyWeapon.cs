using UnityEngine;
using System.Collections;

public class BaseFlyWeapon : MonoBehaviour {
	// 这里不能用public在editor指定，因为这样取到的属性都是config的属性，没有buff了
	public BaseController Owner{get; set;}
	public float speed = 0.0f;
	public bool IsMy{get; set;}
	public bool SettingFin{get; set;}
	public HarmModel HarmModel{get; set;}
	public GameObject Target{get; set;}
	public Vector3 outPoint = Vector3.zero;

	public virtual float GetSpeed(){
		Debug.Assert(speed > 0.0f, "no fly weapon speed");
		return speed;
	}


	void Start () 
	{
		// Debug.Log("fire !!");
		// speed = 3.0f;
		// Destroy the rocket after 2 seconds if it doesn't get destroyed before then.
		// Destroy(gameObject, 2);
		// InvokeRepeating("UpdateWeaponAngle", 0.0f, 1.0f);
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
			Debug.Log("fly weapon attack");
			target.BeingAttacked(HarmModel);
			Destroy(gameObject); // 注意不能用this，this指的是销毁自己这个脚本
			TriggerCallback();
		}
	}

	protected virtual void TriggerCallback(){

	}
	
	// 默认带跟踪效果，如果有设置目标的话
	protected virtual void Update () {
		UpdatePosition();
		UpdateWeaponAngle();
		if(IsOut()){
			Debug.Log("OUT! Destroy!");
			Destroy(gameObject);
		}
	}

	protected bool IsOut(){
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
		if(pos.x > Screen.width || pos.x < 0 || pos.y > Screen.height || pos.y < 0){
			return true;
		}else{
			return false;
		}
	}

	protected void UpdatePosition(){
		if(Target && !Target.GetComponent<BaseController>().IsDead){
			Vector2 size = Target.GetComponent<BoxCollider2D>().size;
			Vector3 targetPos = Target.transform.position  + new Vector3(0, size.y/2*Target.transform.localScale.y, 0);
			Vector3 myPos = transform.position;
			// Debug.Log("myPos: " + myPos + ", targetPos:" + targetPos);
			transform.position = Vector3.MoveTowards(myPos, targetPos, Time.deltaTime * GetSpeed());
			// transform.Translate(targetPos);
			// float distance = Vector3.Distance(targetPos, myPos);
			// transform.LookAt(Target.transform);     

            // Quaternion missileRotation = Quaternion.LookRotation(Target.transform.position - transform.position, Vector3.up);
            //transform.rotation = Quaternion.Slerp(missile.transform.rotation, missileRotation, Time.deltaTime * missileRotateSpeed);
            // transform.rotation = missileRotation;   
            // transform.Translate(Vector3.forward * Time.deltaTime * missileSpeed);
		}else{
			if(outPoint == Vector3.zero){
				float outGap = 100;
				Vector3 curAngle = transform.eulerAngles;
				Vector3 curPos = transform.position;
				Vector3 curPosPixels = Camera.main.WorldToScreenPoint(curPos);
				float y = Screen.height - curPosPixels.y + outGap;
				float x = y / (Mathf.Atan (curAngle.z));
				Vector3 diffPos = new Vector3 (x, y, 0);
				// Debug.Log("diffPos " + diffPos);
				outPoint = Camera.main.WorldToScreenPoint(curPos) + diffPos;
				outPoint = Camera.main.ScreenToWorldPoint(outPoint);
				// Debug.Log(curPos + " -> " + outPoint + " angle:" + curAngle);
				transform.position = Vector3.MoveTowards(curPos, outPoint, Time.deltaTime * GetSpeed());
				// Debug.Log("what fuck the target 1 ?");
			}
			transform.position = Vector3.MoveTowards(transform.position, outPoint, Time.deltaTime * GetSpeed());
		}
	}

	protected virtual void UpdateWeaponAngle(){
		if(Target && !Target.GetComponent<BaseController>().IsDead){
			Vector2 size = Target.GetComponent<BoxCollider2D>().size;
			Vector3 targetPos = Target.transform.position + new Vector3(0, size.y/2*Target.transform.localScale.y, 0);
			Vector3 myPos = transform.position;
			float disX = Mathf.Abs (targetPos.x - myPos.x);
			float disY = Mathf.Abs (targetPos.y - myPos.y);
			float newAngle = Mathf.Atan2 (disY, disX) * Mathf.Rad2Deg;
			float oldAngle = transform.eulerAngles.z;
			float angle = newAngle - oldAngle;
			angle = IsMy ? angle : -angle;
			// Debug.Log(newAngle + " - " + oldAngle + " = " + angle);
			Vector3 angleVector = new Vector3(0, 0, newAngle);
			transform.eulerAngles = angleVector;
			// Debug.Log("set angle " + angleVector.z);

			// float newAngle = Mathf.Atan2 (disY, disX);
			// transform.Rotate(0, angle, 0);

			// transform.Rotate(angleVector);
			// transform.rotation = Quaternion.Euler(angleVector);
			// transform.eulerAngles  = angleVector;

			// float spd = 1;
			// float forceX = spd;// 取现在的角度，然后飞出屏幕即可
			// float forceY = Mathf.Atan(transform.rotation.eulerAngles.z) * spd;
			// Vector2 force = new Vector2(forceX, forceY);
			// // Debug.Log("force: " + force);
			// GetComponent<Rigidbody2D> ().velocity = force;
		}else{
			Debug.Log("what fuck the target 2 ?");
		}
	}
}
