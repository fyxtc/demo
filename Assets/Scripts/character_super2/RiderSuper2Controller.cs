using UnityEngine;
using System.Collections;

public class RiderSuper2Controller : RiderController {

	protected override void Dead(){
        Debug.Log("dead " + (IsMy?"my ":"enemy ")  + Model.Type );
        model.Life = 0;
        Status = TroopStatus.STATUS_DEAD;
        IsDead = true; 
        spineAnimationState.SetAnimation(0, dieAnimationName, false);
        spineAnimationState.Complete += delegate (Spine.AnimationState state, int trackIndex, int loopCount) {
            // TODO: 神圣复活效果
            model.Life = (int)(characterConfig.GetModel(model.Type).Life * 0.5); // 一半血量复活
            IsDead = false;
            HandleCommand(TroopCommand.CMD_IDLE);
            IsZombie = true;
        };
    }


}
