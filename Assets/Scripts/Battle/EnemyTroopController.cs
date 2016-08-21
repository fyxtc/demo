using UnityEngine;
using System.Collections;

public class EnemyTroopController : PlayerTroopController {
    // protected override void InitData(){
    //     int currentGate = 0;
    //     GateModel m = ConfigManager.share().GateConfigSimple.GateModels[currentGate];
    //     data = new SortedDictionary<TroopType, int >(m.troops, new TroopTypeComparer());
    //     foreach (KeyValuePair<TroopType, int> item in data) {
    //         TroopType troopType = item.Key;
    //         int count = item.Value;
    //         // Debug.Log("troopType " + troopType + ": " + count);
    //     }
    //     skillIds = new List<int>(m.skills);
    // }
}
