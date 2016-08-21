using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Prime31.TransitionKit;

public class TollGateController : MonoBehaviour{
	public int curGate;
	public GameObject detailPanel;

	void Start(){
		GetComponent<Button>().onClick.AddListener(OnClick);
		detailPanel.SetActive(false);
	}

	void OnClick(){
		// Debug.Log("click gate " + curGate);
		detailPanel.GetComponent<TollGateDetailController>().UpdateUI(curGate, true);
		detailPanel.SetActive(!detailPanel.activeSelf);
	}
}
