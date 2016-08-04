using UnityEngine;
using System.Collections;

public class SkillController : MonoBehaviour {
	public SkillType skillType = SkillType.SKILL_ATTACK; //
	public SkillModel Model{get; set;}

	void InitModel(){
		Model = ConfigManager.share().SkillConfig.GetSkillModel(skillType);
        // Debug.Log(Model.Attack + ", " + Model.Defense + ", " + Model.HitRate);
	}

	// Use this for initialization
	void Start () {
		InitModel();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SkillStart(){
		// todo: send event this kind skill start
	}

	void SkillStop(){

	}

	void SkillCD(){

	}
}
