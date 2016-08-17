using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

// 增加一个技能需要在editor里面修改skillButtons，每个兵种skill_tip里面的images

public class SkillController : MonoBehaviour {
	public SkillType skillType;
	public SkillType SkillType{
		get{return skillType;} 
		set{skillType = value;}
	} 
	public SkillModel Model{get; set;}
	public event EventHandler SkillEventHandler;
	public SkillEvent SkillEvent{get; set;}
	public bool IsMy{get; set;}
	public bool IsForceStop{get; set;}
	private float totalCD = 0;
	private float curCD = 0;

	SkillStatus status;

	public void InitModel(){
		Model = ConfigManager.share().SkillConfig.GetModel(SkillType);
		// if(!IsMy){
			// Debug.Log(skillType + ", " + Model);
		// }
		SkillEvent = new SkillEvent();
		SkillEvent.Type = SkillType;
		// IsMy表示是不是对自己使用，只有非debuff才是对自己使用
		SkillEvent.IsMy = IsMy;
		SkillEvent.IsDebuff = Model.IsDebuff;
        // Debug.Log(SkillType + " " + transform.position.x +": " + Model.Attack + ", " + Model.Defense + ", " + Model.HitRate);
	}

	// Use this for initialization
	void Start () {
		InitModel();
		GetComponent<Button>().onClick.AddListener(OnClick);
	}
	
	// Update is called once per frame
	void Update () {
		if(status == SkillStatus.STATUS_CD && totalCD > 0 && curCD < totalCD){
			curCD += Time.deltaTime;
			float percent = Mathf.Min(curCD / totalCD, 1.0f);
			GetComponent<Image> ().fillAmount = percent;
			// if(isMy){
				// Debug.Log(SkillType + ": " + status + ", " + curCD + ", " + totalCD + ", " + percent);
			// }
			if(percent >= 1.0f){
				SkillReset();
			}
		}
	}

	void OnClick(){
		if(status == SkillStatus.STATUS_IDLE){
			SkillStart();
		}
	}

	public void AIClick(){
		Debug.Assert(status == SkillStatus.STATUS_IDLE || status == SkillStatus.STATUS_STOP, "AI dummy " + status);
		SkillStart();
	}

	void SkillStart(){
		// todo: send event this kind skill start
		// Debug.Log ("skill " + SkillType + " start");
        // Debug.Log(Model.Attack + ", " + Model.Defense + ", " + Model.HitRate);
		status = SkillStatus.STATUS_USING;
		Debug.Assert(Model.CD >= Model.Time, "error cd time");
		Invoke("SkillStop", (float)Model.Time);
		Invoke("SkillReset", (float)Model.CD);

		SkillEvent.Status = status;
		// Debug.Log(SkillEventHandler + ", " + SkillEvent + ", this: " + this);
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

	public void CDBegin(float cd){
		// Debug.Log(SkillType +  " CDBegin " + cd);
		status = SkillStatus.STATUS_CD;
		this.totalCD = cd;
	}

	void SkillReset(){
		status = SkillStatus.STATUS_IDLE;
		totalCD = 0;
		curCD = 0;
		// SkillEvent.Status = status;
		// SkillEventHandler(this, SkillEvent);
		// Debug.Log ("skill " + SkillType + " reset");
	}
}
