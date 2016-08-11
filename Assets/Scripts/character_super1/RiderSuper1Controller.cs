using UnityEngine;
using System.Collections;

public class RiderSuper1Controller : RiderController {
    protected override void Start(){
        base.Start();
        SetRelifeDelegate();
    }

    void SetRelifeDelegate(){
        spineAnimationState.Complete += delegate (Spine.AnimationState state, int trackIndex, int loopCount) {
            // TODO: 神圣复活效果
            if(state.GetCurrent(trackIndex).Animation.Name == "death"){
                DeadAnimCompleteCallback();
            }
        };
    }

    protected virtual void DeadAnimCompleteCallback(){
        model.Life = (int)(characterConfig.GetModel(model.Type).Life * 0.5); // 一半血量复活
        Debug.Log("Trick: " + Model.Type + " relife " + model.Life);
        IsDead = false;
        Status = TroopStatus.STATUS_IDLE;
        HandleCommand(TroopCommand.CMD_IDLE);

    }

    protected override void Dead(){
        Debug.Log("dead " + (IsMy?"my ":"enemy ")  + Model.Type );
        model.Life = 0;
        Status = TroopStatus.STATUS_DEAD;
        IsDead = true; 
        spineAnimationState.SetAnimation(0, dieAnimationName, false);
        // spineAnimationState.Data.SetMix (shootAnimationName, dieAnimationName, 0.2f);
        // 注意不能在End的回调里再调用setanim，因为setanim总是会触发end，所以会无限递归
    }
}
