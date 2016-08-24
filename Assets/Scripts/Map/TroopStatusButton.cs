using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public enum ButtonStatus{
	LOCKED, TOBUY, COMMON, SELECTED
}

public enum ButtonAction{
	UNLOCKED, SELECTED, DISSELECTED
}

public class TroopStatusEvent : EventArgs{
	public TroopType Type{get; set;}
	public ButtonAction Action{get; set;}
}

public class TroopStatusButton : MonoBehaviour {
    public Sprite[] sprites;
    public TroopType type;
    public ButtonStatus Status{get; set;}
    public Text statusText;
    private PlayerManager player;
    private TroopStatusEvent e = new TroopStatusEvent();
	public event EventHandler Handler;
	public GameObject panel;

    public void UpdateStatus(){
    	ButtonStatus newStatus = GetTroopStatus();
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
	    		color = Color.blue;
	    		break;
	    	case ButtonStatus.TOBUY:
	    		color = Color.red;
	    		break;
    	}
    	GetComponent<Image>().color = color;
    }

   	public ButtonStatus GetTroopStatus(){
		// 注意判断的优先级
		if(player.UsingTroops.FindIndex(v => v.Equals((int)type)) != -1){
			return ButtonStatus.SELECTED;
		}else if(player.OwnTroops.FindIndex(v => v.Equals((int)type)) != -1){
			return ButtonStatus.COMMON;
		}else{
			// 如果自己就是base
			if(DemoUtil.GetBase(type) == type){
				return ButtonStatus.LOCKED;
			}else{
				// if(panel.GetComponent<TroopsStatusController>().GetButtonStatus(DemoUtil.GetBase(type)) != ButtonStatus.LOCKED ){
				// 如果有base了，才知tobuy状态
				if(player.OwnTroops.FindIndex(v => v.Equals((int)DemoUtil.GetBase(type))) != -1){
					return ButtonStatus.TOBUY;			
				}else{ 
					return ButtonStatus.LOCKED;
				}
			}
		}
			
	}

	void OnClick(){
		// TODO: 不用在每次点击都写到本地存储，存储的操作应该在确定退出之后，也许可以在Update加一个是否写入的bool值
		// Debug.Log("click status: " + Status);
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
			case ButtonStatus.TOBUY:
				player.UpdateOwnTroops(type);
				Debug.Log("show buy window");
				break;
			default:
				Debug.Assert(false, "error button status");
				break;
		}
		// UpdateStatus();// move to troop status controller
	}

    void Awake(){
    	// Debug.Log("status : " + type);
    	GetComponent<Image>().sprite = sprites[(int)type];
    	player = PlayerManager.Instance;
    	Status = ButtonStatus.LOCKED; // default status
    }

	// Use this for initialization
	void Start () {
    	UpdateStatus();
    	GetComponent<Button>().onClick.AddListener(OnClick);
		// Debug.Assert(Status != null);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
