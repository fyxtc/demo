using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SkillStatusEvent : EventArgs{
	public SkillType Type{get; set;}
	public ButtonAction Action{get; set;}
}

public class SkillStatusButton : MonoBehaviour {
	public Sprite[] sprites;
	public SkillType type;
	private ButtonStatus Status{get; set;}
    private PlayerManager player;
    public Text statusText;
    private SkillStatusEvent e = new SkillStatusEvent();
	public event EventHandler Handler;

	void OnClick(){
		e.Type = type;
		switch(Status){
			case ButtonStatus.LOCKED:
				e.Action = ButtonAction.UNLOCKED;
				Handler(this, e);
				break;
			case ButtonStatus.COMMON:
				e.Action = ButtonAction.SELECTED;
				Handler(this, e);
				break;
			case ButtonStatus.SELECTED:
				e.Action = ButtonAction.DISSELECTED;
				Handler(this, e);
				break;
			default:
				Debug.Assert(false, "error button status");
				break;
		}
	}

	public void UpdateStatus(){
    	ButtonStatus newStatus = GetSkillStatus();
    	// Debug.Log(type + ": " + Status + " -> " + newStatus);
    	Status = newStatus;
    	statusText.text = newStatus.ToString();

    	Color color = Color.white;
    	switch(Status){
    		case ButtonStatus.COMMON:
	    		color = Color.white;
	    		break;
	    	case ButtonStatus.LOCKED:
	    		color = Color.gray;
	    		break;
	    	case ButtonStatus.SELECTED:
	    		// color = Color.blue;
	    		break;
	    	case ButtonStatus.TOBUY:
	    		Debug.Assert(false);
	    		// color = Color.red;
	    		break;
    	}
    	GetComponent<Image>().color = color;
    }

   	public ButtonStatus GetSkillStatus(){
		// 注意判断的优先级
		if(player.UsingSkills.FindIndex(v => v.Equals((int)type)) != -1){
			return ButtonStatus.SELECTED;
		}else if(player.OwnSkills.FindIndex(v => v.Equals((int)type)) != -1){
			return ButtonStatus.COMMON;
		}else{
			return ButtonStatus.LOCKED;
		}
	}

	void Awake(){
    	GetComponent<Image>().sprite = sprites[(int)type];
    	GetComponent<Button>().onClick.AddListener(OnClick);
    	player = PlayerManager.Instance;
	}

	// Use this for initialization
	void Start () {
		UpdateStatus();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
