using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SkillController : MonoBehaviour {
	public SkillType skillType; 
	public SkillModel Model{get; set;}
	public event EventHandler SkillEventHandler;
	public SkillEvent SkillEvent{get; set;}
	public bool isMy = true;

	SkillStatus status;

	void InitModel(){
		Model = ConfigManager.share().SkillConfig.GetSkillModel(skillType);
		SkillEvent = new SkillEvent();
		SkillEvent.Type = skillType;
		SkillEvent.IsMy = isMy;
        // Debug.Log(skillType + " " + transform.position.x +": " + Model.Attack + ", " + Model.Defense + ", " + Model.HitRate);
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
		// Debug.Log ("skill " + skillType + " start");
        // Debug.Log(Model.Attack + ", " + Model.Defense + ", " + Model.HitRate);
		status = SkillStatus.STATUS_USING;
		Debug.Assert(Model.CD > Model.Time, "error cd time");
		Invoke("SkillStop", (float)Model.Time);
		Invoke("SkillReset", (float)Model.CD);

		SkillEvent.Status = status;
		SkillEventHandler(this, SkillEvent);
	}

	void SkillStop(){
		status = SkillStatus.STATUS_STOP;
		SkillEvent.Status = status;
		SkillEventHandler(this, SkillEvent);
		// Debug.Log ("skill " + skillType + " stop");
	}

	void SkillReset(){
		status = SkillStatus.STATUS_IDLE;
		// SkillEvent.Status = status;
		// SkillEventHandler(this, SkillEvent);
		// Debug.Log ("skill " + skillType + " reset");
	}
}
