using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SkillController : MonoBehaviour {
	public SkillType skillType;
	public SkillType SkillType{
		get{return skillType;} 
		set{skillType = value;}
	} 
	public SkillModel Model{get; set;}
	public event EventHandler SkillEventHandler;
	public SkillEvent SkillEvent{get; set;}
	public bool isMy = true;
	public bool IsForceStop{get; set;}

	SkillStatus status;

	void InitModel(){
		Model = ConfigManager.share().SkillConfig.GetSkillModel(SkillType);
		SkillEvent = new SkillEvent();
		SkillEvent.Type = SkillType;
		SkillEvent.IsMy = isMy;
        // Debug.Log(SkillType + " " + transform.position.x +": " + Model.Attack + ", " + Model.Defense + ", " + Model.HitRate);
	}

	// Use this for initialization
	void Start () {
		InitModel();
		GetComponent<Button>().onClick.AddListener(OnClick);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick(){
		if(status == SkillStatus.STATUS_IDLE){
			SkillStart();
		}
	}

	void SkillStart(){
		// todo: send event this kind skill start
		// Debug.Log ("skill " + SkillType + " start");
        // Debug.Log(Model.Attack + ", " + Model.Defense + ", " + Model.HitRate);
		status = SkillStatus.STATUS_USING;
		Debug.Assert(Model.CD > Model.Time, "error cd time");
		Invoke("SkillStop", (float)Model.Time);
		Invoke("SkillReset", (float)Model.CD);

		SkillEvent.Status = status;
		SkillEventHandler(this, SkillEvent);
	}

	public void SkillStop(){
		// 有可能先被覆盖导致stop了，原来就是没加这个判断才导致发了两次。。。傻逼了，还加什么forcestop...orz
		if(status != SkillStatus.STATUS_STOP){
			status = SkillStatus.STATUS_STOP;
			SkillEvent.Status = status;
			SkillEventHandler(this, SkillEvent);
			// Debug.Log ("skill " + SkillType + " stop");
		}
	}

	void SkillReset(){
		status = SkillStatus.STATUS_IDLE;
		// SkillEvent.Status = status;
		// SkillEventHandler(this, SkillEvent);
		// Debug.Log ("skill " + SkillType + " reset");
	}
}
