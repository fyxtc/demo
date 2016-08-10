using UnityEngine;
using System.Collections;

public class DancerSuper2Controller : DancerController {
    protected virtual bool CanMiss(HarmModel harmModel){
        double hitRate = harmModel.HitRate * 0.8;
        return UnityEngine.Random.value >= hitRate; 
    }
}
