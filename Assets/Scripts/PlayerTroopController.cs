﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerTroopController : MonoBehaviour {
    public bool isMy = true;
    float[] posConfig = {-0.5f, -2.5f, -3.0f, -4.5f};
    int maxCount = 4; // 最多能带四个兵团
    Dictionary<TroopType, int> data = new Dictionary<TroopType, int>(); 
	Dictionary<TroopType, List<GameObject>> troops = new Dictionary<TroopType, List<GameObject>>();
	public GameObject saber;
	public GameObject archer;

	// Use this for initialization
	void Start () {
        data.Add(TroopType.Saber, 1);
        data.Add(TroopType.Archer, 1);
        InitTroops();
    }

    void InitTroops(){
        foreach (KeyValuePair<TroopType, int> item in data) {
            TroopType troopType = item.Key;
            int count = item.Value;
            // Debug.Log("key=" + item.Key.ToString() + "；value=" + item.Value.ToString());  
            List<GameObject> characters = new List<GameObject>();
            switch (troopType){
            case TroopType.Saber:
                for(int i = 0; i < count; i++){
					GameObject obj = Instantiate(saber);
                    obj.GetComponent<SaberController>().IsMy = isMy;
                    characters.Add(obj);
                }
                break;
            case TroopType.Archer:
                for(int i = 0; i < count; i++){
                    GameObject obj = Instantiate(archer);
                    obj.GetComponent<ArcherController>().IsMy = isMy;
                    characters.Add(obj);
                }
                break;
            }
            troops.Add(troopType, characters);
        }
        Debug.Assert(troops.Count > 0 && troops.Count < maxCount, "troops count error");
        resort();
    }
    
    void resort(){
        int rank = 0;
        foreach (KeyValuePair<TroopType, List<GameObject>> item in troops) {
            TroopType troopType = item.Key;
            List<GameObject> troop = item.Value;
            float posX = getRankPos(rank);
            double interval = getTroopInterval(troopType);
            // Debug.Log("interval: " + getTroopInterval(troopType).ToString() + ", " + interval.ToString());

            for(int i = 0; i < troop.Count; i++){
                GameObject obj = troop[i];
                double x = posX - interval * i;
                if(!isMy){
                    x = -x;
                }
				// Vector3 v = new Vector3((float)x, obj.transform.position.y, obj.transform.position.z);
                // Debug.Log("x = " + x.ToString());
				Vector3 v = new Vector3((float)x, obj.transform.position.y, 0);
				obj.transform.position = v;
				// obj.transform.Translate(v);
            }
            rank++;
        }
    }

    double getTroopInterval(TroopType type){
        double res = 0.0;
        switch (type) 
        {
		case TroopType.Saber:
			res = ConfigManager.share ().getCharacterConfig ().Saber.Interval;
			break;
		case TroopType.Archer:
			res = ConfigManager.share ().getCharacterConfig ().Archer.Interval;
			break;
        default:
            Debug.Assert(false);
            break;
        }
        return res;
    }

    float getRankPos(int rank){
        return posConfig[rank];
    }

    // Update is called once per frame
    void Update () {
	}
}
