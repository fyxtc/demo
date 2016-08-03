using UnityEngine;
using System.Collections;
using Spine.Unity;

public abstract class BaseController : MonoBehaviour {

	protected BaseModel Model{get;set;}
    public bool IsMy{get;set;}
	protected abstract void InitModel();
    public float speed = 0.1f;

	protected enum TroopStatus{
        STATUS_IDLE, STATUS_FORWARD, STATUS_ATTACK, STATUS_BACK,
    }

    protected TroopStatus status;

    protected enum TroopCommand{
        CMD_IDLE, CMD_FORWARD, CMD_ATTACK, CMD_BACK,
    }

    public Vector3 startPos;
    public Vector3 endPos;

    #region Inspector
    // [SpineAnimation] attribute allows an Inspector dropdown of Spine animation names coming form SkeletonAnimation.
    [SpineAnimation]
    public string runAnimationName;

    [SpineAnimation]
    public string idleAnimationName;

    [SpineAnimation]
    public string walkAnimationName;

    [SpineAnimation]
    public string shootAnimationName;
    #endregion

    SkeletonAnimation skeletonAnimation;

    // Spine.AnimationState and Spine.Skeleton are not Unity-serialized objects. You will not see them as fields in the inspector.
    public Spine.AnimationState spineAnimationState;
    public Spine.Skeleton skeleton;

    void Start () {
        // Make sure you get these AnimationState and Skeleton references in Start or Later. Getting and using them in Awake is not guaranteed by default execution order.
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        spineAnimationState = skeletonAnimation.state;
        skeleton = skeletonAnimation.skeleton;

        // StartCoroutine(DoDemoRoutine());

        InitModel ();
        // IsMy = true; // test
         // skeleton.FlipX = !IsMy;
        Idle();
        startPos = transform.position;
        endPos = new Vector3(IsMy ? 6 : -6, transform.position.y);
        handleCommand(TroopCommand.CMD_IDLE);
    }

    void Update(){
        switch(status){
        case TroopStatus.STATUS_IDLE:
            break;
        case TroopStatus.STATUS_FORWARD:
			transform.position = Vector3.Lerp(transform.position, endPos, 1/((transform.position - endPos).magnitude) * speed);
            // Debug.Log(transform.position.x + ", " + endPos.x);
            if(transform.position == endPos){
                handleCommand(TroopCommand.CMD_ATTACK);
            }
            break;
        case TroopStatus.STATUS_ATTACK:
            break;
        case TroopStatus.STATUS_BACK:
			transform.position = Vector3.Lerp(transform.position, startPos, 1/((transform.position - startPos).magnitude) * speed);
            if(transform.position == startPos){
                handleCommand(TroopCommand.CMD_IDLE);
            }
            break;
        }

    }

    void FixedUpdate(){
    }

    void handleCommand(TroopCommand cmd){
        Debug.Log("handleCommand " + cmd);
        switch(cmd){
        case TroopCommand.CMD_IDLE:
            Idle();
            break;
        case TroopCommand.CMD_FORWARD:
            Forward();
            break;
        case TroopCommand.CMD_ATTACK:
            Attack();
            break;
        case TroopCommand.CMD_BACK:
            Back();
            break;
        }
    }

    void Forward(){
        status = TroopStatus.STATUS_FORWARD;
        skeleton.FlipX = !IsMy;
        spineAnimationState.SetAnimation(0, walkAnimationName, true);
    }

    void Attack(){
        status = TroopStatus.STATUS_ATTACK;
        spineAnimationState.SetAnimation(0, shootAnimationName, false);
        Invoke("DoBackDelay", 0.5f);
    }

    void Back(){
        status = TroopStatus.STATUS_BACK;
        skeleton.FlipX = IsMy;
        spineAnimationState.SetAnimation(0, walkAnimationName, true);
    }

    void Idle(){
        status = TroopStatus.STATUS_IDLE;
        skeleton.FlipX = !IsMy;
        spineAnimationState.SetAnimation(0, idleAnimationName, true);
        Invoke("DoForwardDelay", 2.0f);
    }

    void DoBackDelay () {
		handleCommand(TroopCommand.CMD_BACK);
    }

    void DoForwardDelay () {
        handleCommand(TroopCommand.CMD_FORWARD);
    }

        
    /// <summary>This is an infinitely repeating Unity Coroutine. Read the Unity documentation on Coroutines to learn more.</summary>
    IEnumerator DoDemoRoutine () {
        
        while (true) {
            // SetAnimation is the basic way to set an animation.
            // SetAnimation sets the animation and starts playing it from the beginning.
            // Common Mistake: If you keep calling it in Update, it will keep showing the first pose of the animation, do don't do that.

            spineAnimationState.SetAnimation(0, walkAnimationName, true);
            yield return new WaitForSeconds(1.5f);

            // skeletonAnimation.AnimationName = runAnimationName; // this line also works for quick testing/simple uses.
            spineAnimationState.SetAnimation(0, runAnimationName, true);
            yield return new WaitForSeconds(1.5f);

            spineAnimationState.SetAnimation(0, idleAnimationName, true);
            yield return new WaitForSeconds(1f);

            skeleton.FlipX = true;      // skeleton allows you to flip the skeleton.
            yield return new WaitForSeconds(0.5f);
            skeleton.FlipX = false;
            yield return new WaitForSeconds(0.5f);

        }
    }
}
