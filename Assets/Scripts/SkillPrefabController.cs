using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillPrefabController : MonoBehaviour {
	public GameObject bg;
	public GameObject button;
	public GameObject Button{get{return button;}}
	public SkillType SkillType{get; set;}
	public Sprite[] images;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateSkillType(SkillType type, bool isMy){
		SkillType = type;
		Button.GetComponent<SkillController>().SkillType = SkillType;
		Button.GetComponent<Image>().sprite = images[(int)type];
		bg.GetComponent<Image> ().sprite = images [(int)type];
		Button.GetComponent<SkillController> ().InitModel ();
		Button.GetComponent<SkillController>().IsMy = isMy;

	}

	public SkillController GetSkillContrller(){
		return Button.GetComponent<SkillController>();
	}
}
