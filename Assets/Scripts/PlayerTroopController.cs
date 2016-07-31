using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerTroopController : MonoBehaviour {
    Dictionary<int, int> data = new Dictionary<int, int>(); 
	PlayerTroopModel model;
	List<List<BaseCharacter>> troops;

	// Use this for initialization
	void Start () {
        data.Add(1, 5);
        data.Add(2, 5);
		model = new PlayerTroopModel(data);
		troops = model.Troops;;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
