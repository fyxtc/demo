using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPBarController : MonoBehaviour {
    public Image hp;
    public GameObject Character{get; set;}
    public Vector3 diffVec;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	   if(Character){
            BaseController controller = Character.GetComponent<BaseController>();
			if (!controller.IsDead) {
				transform.position = Camera.main.WorldToScreenPoint (Character.transform.position + diffVec);
				float max = ConfigManager.share ().CharacterConfig.GetModel (controller.Model.Type).Life;
				float cur = controller.Model.Life;
				hp.fillAmount = cur / max;
			}
       }
	}
}
