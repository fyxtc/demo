using UnityEngine;
using System.Collections;
using Prime31.TransitionKit;

public class TollGateController : MonoBehaviour{
	public int curGate;
	void Start(){
		// Debug.Log("curGate " + curGate);
	}

	void OnMouseDown(){
		Debug.Log("OnMouseDown gate " + curGate);
		TollGateManager.Instance.CurGate = curGate;
		var slices = new VerticalSlicesTransition()
		{
			nextScene = 2,
			divisions = Random.Range( 3, 20 )
		};
		TransitionKit.instance.transitionWithDelegate( slices );
	}

}
