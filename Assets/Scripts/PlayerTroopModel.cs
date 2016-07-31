using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

enum TroopType{
    Saber=1,Archer=2
}

// public class PlayerTroopModel {
//     int maxCount = 4; // 最多能带四个兵团
//     Dictionary<TroopType, List<BaseCharacter>> troops;
//     bool isMy = true;
//     Dictionary<int, int> data; 

//     public Dictionary<TroopType, List<BaseCharacter>> Troops{
//         get{return troops;}
//     }
//     public bool IsMy{
//         set{isMy = value;}
//         get{return isMy;}
//     }

// 	public PlayerTroopModel(Dictionary<int, int> data){
//         this.data = data;
//         InitData();
//     }

//     void InitData(){
//         troops = new Dictionary<TroopType, List<BaseCharacter>>();
//         foreach (KeyValuePair<int, int> item in data){
//             Debug.Log("key=" + item.Key.ToString() + "；value=" + item.Value.ToString());  
//             TroopType troopType = (TroopType)item.Key;
//             int count = item.Value;
//             List<BaseCharacter> tempTroop = new List<BaseCharacter>();
//             switch (troopType){
// 			case TroopType.Saber:
//                 for(int i = 0; i < count; i++){
//                     tempTroop.Add(new Saber());
//                 }
//                 break;
// 			case TroopType.Archer:
//                 for(int i = 0; i < count; i++){
//                     tempTroop.Add(new Archer());
//                 }
//                 break;
//             }
//             troops.Add(troopType, tempTroop);
//         }

//         Debug.Assert(troops.Count > 0 && troops.Count < maxCount, "troops count error");
//     }
// }
