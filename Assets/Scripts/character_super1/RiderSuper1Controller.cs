using UnityEngine;
using System.Collections;

public class RiderSuper1Controller : RiderController {
    protected override void OnDeadAnimComplete(){
        // TODO: 神圣效果
        model.Life = (int)(characterConfig.GetModel(model.Type).Life * 0.5); // 一半血量复活
        Debug.Log("Trick: " + Model.Type + " relife " + model.Life);
        IsDead = false;
        Status = TroopStatus.STATUS_IDLE;
        HandleCommand(TroopCommand.CMD_IDLE);
    }
}
