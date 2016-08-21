using UnityEngine;
using System.Collections;

public class RiderSuper2Controller : RiderSuper1Controller {
    protected override void OnDeadAnimComplete(){
        // TODO: zombie形态效果
        IsDead = false;
        Status = TroopStatus.STATUS_IDLE;
        HandleCommand(TroopCommand.CMD_IDLE);
        Debug.Log("Trick: " + Model.Type + " become zombie");
        IsZombie = true;
    }
}
