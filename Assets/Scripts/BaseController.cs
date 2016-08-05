using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using System;

public abstract class BaseController : MonoBehaviour {
    protected BaseModel model;
    protected BaseModel Model{
        get{
            if(isBuffing){
                return AddBuffModel();
            }else{
                return model;
            }
        }
        set{
            model = value;
        }
    }
    BaseModel AddBuffModel(){
        BaseModel resModel = (BaseModel)model.Clone(); // 用Model，就无限递归了。。。，还有，不能用model，因为是引用，直接修改model的值了。。fuck
        resModel.Attack = (int)(resModel.Attack * (1.0 + skillBuffModel.Attack));
        resModel.Defense = (int)(resModel.Defense * (1.0 + skillBuffModel.Defense));
        resModel.HitRate = resModel.HitRate * (1.0 + skillBuffModel.HitRate);
        if(resModel.HitRate > 1.0){
            resModel.HitRate = 1.0;
        }
        // Debug.Log("buff: attack " + resModel.Attack);
        return resModel;
    }
    public bool IsMy{get;set;}
    protected abstract void InitModel();
    public float speed = 0.1f;
    private GameObject attackedTarget;
    public PlayerTroopController OtherTroopController{get; set;}
    public GameObject Attacker{get; set;}
    public bool IsDead{ get; set;}
    SkillModel skillBuffModel = new SkillModel();
    bool isBuffing = false;

    protected enum TroopStatus{
        STATUS_IDLE, STATUS_FORWARD, STATUS_ATTACK, STATUS_BACK, STATUS_DEAD,
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

        for(int i = 0; i < 3; i++){
            string tag = "skill_"+i;
            GameObject.Find(tag).GetComponent<SkillController>().SkillEventHandler += OnSkillEvent;
        }
    }

    void Update(){
        switch(status){
        case TroopStatus.STATUS_IDLE:
            break;
        case TroopStatus.STATUS_FORWARD:
            transform.position = Vector3.Lerp(transform.position, endPos, 1/((transform.position - endPos).magnitude) * speed);
            // Debug.Log(transform.position.x + ", " + endPos.x);
            // if(transform.position == endPos){
            //     handleCommand(TroopCommand.CMD_ATTACK);
            // }
            if(attackedTarget = FindAttackedTarget()){
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

    GameObject FindAttackedTarget(){
        // Debug.Log(OtherTroopController);
        GameObject obj = OtherTroopController.GetAttackTarget();
        if (obj == null) {
            return null;
        }
        if(IsMy){
            if(transform.position.x + Model.AttackRange >= obj.transform.position.x){
                return obj;
            }
        }else{
            if(transform.position.x - Model.AttackRange <= obj.transform.position.x){
                return obj;
            }
        }
        return null;
    }

    void FixedUpdate(){
    }

    void handleCommand(TroopCommand cmd){
        // Debug.Log("handleCommand " + cmd);
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
        if(IsMy){
            // Debug.Log("my x:" + transform.position.x + " target x:" + attackedTarget.transform.position.x);
        }
        Invoke("DoBackDelay", 0.5f);
        attackedTarget.GetComponent<BaseController>().Attacker = this.gameObject;
        attackedTarget.GetComponent<BaseController>().BeingAttacked();
        // attackedTarget = null;
    }

    public void BeingAttacked(){
        if(CanMiss()){
            // miss anim
            // Debug.Log("attack miss");
        }else{
            Model.Life -= CalcHarm();
            // Debug.Log("Life : " + Model.Life);
            if(Model.Life <= 0){
                Dead();
            }
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }
    }

    int CalcHarm(){
        float att = Attacker.GetComponent<BaseController>().Model.Attack;
        if(!IsMy){
            // Debug.Log("attacker att " + att);
        }

        float def = Model.Defense;
        int res = (int)(att * (att / (att + def)));
        Debug.Assert(res >= 0, "CalcHarm error");
        return res;
    }

    bool CanMiss(){
        return UnityEngine.Random.value > Attacker.GetComponent<BaseController>().Model.HitRate; 
    }

    void Dead(){
        Debug.Log("dead");
        Model.Life = 0;
        status = TroopStatus.STATUS_DEAD;
        IsDead = true;
        // Destroy(gameObject);
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
        Invoke("DoForwardDelay", (float)Model.AttackCD);
    }

    void DoBackDelay () {
        handleCommand(TroopCommand.CMD_BACK);
    }

    void DoForwardDelay () {
        handleCommand(TroopCommand.CMD_FORWARD);
    }


    void OnSkillEvent(object sender, EventArgs e)
    {
        SkillEvent ev = (SkillEvent)e;
        // 给自己的小兵加
        if(ev.IsMy == IsMy){
            Debug.Log("type " + ev.Type + ", status " + ev.Status);
            isBuffing = ev.Status == SkillStatus.STATUS_USING;
            // 感觉能叠加技能更混乱一点。。。也难写一点。。。。，先不叠加吧
            if(isBuffing){
                skillBuffModel = ConfigManager.share().SkillConfig.GetSkillModel(ev.Type);
            }
        }
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
