using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SkillShowButton : MonoBehaviour {
	[SerializeField]
	public SkillType Type{get; set;}
	public Sprite[] sprites;
	public int foucsIndex;
	public GameObject panel;

	public void UpdateType(SkillType type){
		Type = type;
		Debug.Log("update show type " + type);
		if(type != SkillType.SKILL_INVALID){
	    	GetComponent<Image>().sprite = sprites[(int)type];
		}else{
			// TODO: +
		}
	}

	public void UpdateStatus(bool isSelected){
		GetComponent<Image>().color = isSelected ? Color.gray : Color.white;
	}

	void OnClick(){
		panel.GetComponent<SkillsStatusController>().FoucsIndex = foucsIndex;
	}

	void Awake(){
		GetComponent<Button>().onClick.AddListener(OnClick);
	}

	// Use this for initialization
	void Start () {
		if(foucsIndex == 0){
			UpdateStatus(true); // default selected
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
