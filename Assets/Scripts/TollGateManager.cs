using UnityEngine;
using System.Collections;

public class TollGateManager {
    public static readonly TollGateManager Instance = new TollGateManager();
    public int CurGate{get; set;}
    public int CurStar{get; set;}
    public int CurDifficulty{get; set;}
    public int UnlockGate{get; set;}
}
