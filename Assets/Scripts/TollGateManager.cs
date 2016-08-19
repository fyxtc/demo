using UnityEngine;
using System.Collections;

public class TollGateManager {
    public static readonly TollGateManager Instance = new TollGateManager();
    public int CurGate{get; set;}
}
