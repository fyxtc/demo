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





	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
