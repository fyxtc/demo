using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class SkillsStatusController : MonoBehaviour {
	public GameObject[] showSkills;
	public GameObject[] buttons;
	public GameObject showPanel;
	public Button ok;
	public Button clean;
	public Button cancel;
	public int foucsIndex;
	public int FoucsIndex{
		get{return foucsIndex;}
		set{
			if(value == FoucsIndex){
				return;
			}else{
				showSkills[foucsIndex].GetComponent<SkillShowButton>().UpdateStatus(false);
				foucsIndex = value;
				showSkills[foucsIndex].GetComponent<SkillShowButton>().UpdateStatus(true);
			}
		}
	}

	void OnSkillStatusEvent(object sender, EventArgs e){
		SkillStatusButton btn = sender as SkillStatusButton;
		SkillStatusEvent ev = (SkillStatusEvent)e;
		SkillType type = ev.Type;
		ButtonAction action = ev.Action;
		switch(action){
			case ButtonAction.SELECTED:
				showSkills[FoucsIndex].GetComponent<SkillShowButton>().UpdateType(type);
				PlayerManager.Instance.UpdateUsingSkills(type, FoucsIndex);
				break;
			case ButtonAction.DISSELECTED:
				PlayerManager.Instance.UpdateUsingSkills(type, FoucsIndex);
				// RemoveSkill(type);
				break;
			case ButtonAction.UNLOCKED:
				PlayerManager.Instance.UpdateOwnSkills(type);
				break;
			default:
				Debug.Assert(false);
				break;
		}
		btn.UpdateStatus();
	}

	void Awake(){
		for(int i = 0; i < buttons.Length; i++){
			buttons[i].GetComponent<SkillStatusButton>().Handler += OnSkillStatusEvent;
		}
		clean.onClick.AddListener(() => PlayerPrefs.DeleteAll());
		ok.onClick.AddListener(delegate() { PlayerManager.Instance.SaveSkills();gameObject.SetActive(false);});
		cancel.onClick.AddListener(() => gameObject.SetActive(false));
	}

	// Use this for initialization
	void Start () {
		List<int> skills = PlayerManager.Instance.UsingSkills;
		// List<int> skills = new List<int>{0, 1};
		Debug.Assert(skills.Count <= 2, "skills using count > 2");
		for(int i = 0; i < skills.Count; i++){
			showSkills[i].GetComponent<SkillShowButton>().UpdateType((SkillType)skills[i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
