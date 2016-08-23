using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public enum ButtonStatus{
	LOCKED, TOBUY, COMMON, SELECTED
}

public class TroopStatusEvent : EventArgs{
	public TroopType Type{get; set;}
	public bool IsAdd{get; set;}
}

public class TroopStatusButton : MonoBehaviour {
    public Sprite[] sprites;
    public TroopType type;
    public ButtonStatus Status{get; set;}
    public Text statusText;
    private PlayerManager player;
    private TroopStatusEvent e = new TroopStatusEvent();
	public event EventHandler Handler;

    public void UpdateStatus(){
    	ButtonStatus newStatus = GetTroopStatus();
    	Debug.Log(type + ": " + Status + " -> " + newStatus);
    	Status = newStatus;
    	statusText.text = newStatus.ToString();
    }

   	ButtonStatus GetTroopStatus(){
		// 注意判断的优先级
		if(player.UsingTroops.FindIndex(v => v.Equals((int)type)) != -1){
			return ButtonStatus.SELECTED;
		}else if(player.OwnTroops.FindIndex(v => v.Equals((int)type)) != -1){
			return ButtonStatus.COMMON;
		}else{ // 先忽略tobuy状态
			return ButtonStatus.LOCKED;
		}
	}

	void OnClick(){
		// TODO: 不用在每次点击都写到本地存储，存储的操作应该在确定退出之后，也许可以在Update加一个是否写入的bool值
		Debug.Log("click status: " + Status);
		switch(Status){
			case ButtonStatus.LOCKED:
				player.UpdateOwnTroops(type);
				break;
			case ButtonStatus.COMMON:
				player.UpdateUsingTroops(type, true);
				e.IsAdd = true;
				e.Type = type;
				Handler(this, e);
				break;
			case ButtonStatus.SELECTED:
				player.UpdateUsingTroops(type, false);
				e.IsAdd = false;
				e.Type = type;
				Handler(this, e);
				break;
			case ButtonStatus.TOBUY:
				player.UpdateOwnTroops(type);
				break;
			default:
				Debug.Assert(false, "error button status");
				break;
		}
		UpdateStatus();
	}

    void Awake(){
    	// Debug.Log("status : " + type);
    	GetComponent<Image>().sprite = sprites[(int)type];
    	player = PlayerManager.Instance;
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
