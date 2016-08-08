using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using System;

public abstract class BaseController : MonoBehaviour {
    protected BaseModel model;
    public BaseModel Model{
        get{
            BaseModel resModel = model;
            if(isBuffing){
                resModel = AddSkillModel();
                // Debug.Log("buffing: " + resModel);
            }
            resModel = AddTrickModels(resModel);
			// if(trickingIds.Count > 0){
   //              Debug.Log("base: " + model);
   //              Debug.Log("tricking: " + resModel);
   //          }
            return resModel;
        }
        set{
            // 注意这里set不能通过skill/trick调啊，不然直接改原型了
            model = value;
        }
    }

    public bool IsMy{get;set;}
    protected abstract void InitModel();
    // public float speed = 0.1f;
    private GameObject attackedTarget;
    public PlayerTroopController OtherTroopController{get; set;}
    public PlayerTroopController MyTroopController{get; set;}
    public GameObject Attacker{get; set;}
    public bool IsDead{ get; set;}
    // unity好像默认访问级别不是private是protected
    SkillModel skillBuffModel = new SkillModel();
    bool isBuffing = false;
    List<int> trickingIds = new List<int>();
    protected TrickConfig trickConfig = ConfigManager.share().TrickConfig;

    public GameObject skillTip;

    public enum TroopStatus{
        STATUS_IDLE, STATUS_FORWARD, STATUS_ATTACK, STATUS_BACK, STATUS_DEAD,
    }

    protected TroopStatus status;

    protected enum TroopCommand{
        CMD_IDLE, CMD_FORWARD, CMD_ATTACK, CMD_BACK,
    }

	public TrickController TrickController{ get; set; }
    protected abstract void InitTrickController();


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
        InitTrickController();
        startPos = transform.position;
        endPos = new Vector3(IsMy ? 6 : -6, transform.position.y);
        handleCommand(TroopCommand.CMD_IDLE);

        Invoke("DispatchNaturalTricks", 0.1f); //不能马上调用，因为这个时候可能别的basecontroller还没有Start，也就还没有Initmodel了
        // Invoke("Test", 5);
    }

    void DispatchNaturalTricks(){
        // if(IsMy){ // test
            List<int> trickIds = TrickController.OnNaturalTrick(true);
            MyTroopController.DispatchTricks(trickIds, true);
        // }
    }

    void Test(){
        if(Model.Type == TroopType.TROOP_SABER && IsMy){
            Dead();
        }
    }

    void Update(){
        switch(status){
        case TroopStatus.STATUS_IDLE:
            break;
        case TroopStatus.STATUS_FORWARD:
            transform.position = Vector3.Lerp(transform.position, endPos, (float)(1/((transform.position - endPos).magnitude) * Model.Speed));
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
            transform.position = Vector3.Lerp(transform.position, startPos, (float)(1/((transform.position - startPos).magnitude) * Model.Speed));
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
            Debug.Log("attack miss");
        }else{
            int harm = CalcHarm();
            Debug.Log("Life: " + Model.Life);
            // 注意！！只有生命值这个玩意必须直接改model底层数据，而不能通过Model，因为这个取到的是副本，改也只是在副本上改
            // Model.Life -= harm;
            model.Life -= harm;
            Debug.Log("Life : " + Model.Life + ", harm:-" + harm);
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
        Debug.Log("dead " + (IsMy?"my ":"enemy ")  + Model.Type );
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


    public void OnSkillEvent(object sender, EventArgs e)
    {
        SkillEvent ev = (SkillEvent)e;
        // 给自己的小兵加
        if(ev.IsMy == IsMy){
            Debug.Log("type " + ev.Type + ", status " + ev.Status);
            isBuffing = ev.Status == SkillStatus.STATUS_USING;
            SpriteRenderer spr = skillTip.GetComponent<SpriteRenderer>();  
            if(isBuffing){
                skillBuffModel = ConfigManager.share().SkillConfig.GetModel(ev.Type);
				spr.sprite = skillTip.GetComponent<SkillTipController>().tipImages[(int)ev.Type];
            }else{
                spr.sprite = null;
            }
            List<int> trickIds = TrickController.GetSkillTrick(ev.Type);
            MyTroopController.DispatchTricks(trickIds, isBuffing);

            Debug.Log("AFTER SKILL&TRICK: " + Model);
        }
    }

    BaseModel AddSkillModel(){
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

    BaseModel AddTrickModels(BaseModel afterBuffModel){
        List<TrickModel> tricks = GetTrickingModels();
        BaseModel resModel = afterBuffModel;
        foreach(TrickModel m in tricks){
            resModel = AddTrickModel(resModel, m);
        }
        return resModel;
    }

    BaseModel AddTrickModel(BaseModel originModel, TrickModel trickModel){
        BaseModel resModel = (BaseModel)originModel.Clone();
		double effect = 1.0 + trickModel.Effect;
		switch(trickModel.Property){
        case TrickProperty.PROPERTY_ATTACK:
            resModel.Attack = (int)(resModel.Attack * effect);
            break;
        case TrickProperty.PROPERTY_DEFENSE:
            resModel.Defense = (int)(resModel.Defense * effect);
            break;
        case TrickProperty.PROPERTY_SPEED:
            resModel.Speed = resModel.Speed * effect;
            break;
        case TrickProperty.PROPERTY_HIT:
            resModel.HitRate = resModel.HitRate * effect;
            if(resModel.HitRate > 1.0){
                resModel.HitRate = 1.0;
            }        
            break;
        case TrickProperty.PROPERTY_LIFE:
            // 固定回复值，注意配置是10.0，这个我估计得特殊处理，每秒回复。。费劲啊，开一个timer去invoke调用，记得timer要清理！！！！
            resModel.Life += (int)effect;
            int maxLife = ConfigManager.share().CharacterConfig.GetModel(Model.Type).Life;
            if(resModel.Life > maxLife){
                resModel.Life = maxLife;
            }
            // 而且注意血量是特么需要重置到自己原本的model的！！！日，不能这样直接复制，会重复叠加了。。。咦？好像可以？
            model.Life = resModel.Life;
            break;
        case TrickProperty.PROPERTY_ATTACK_CD:
            resModel.AttackCD = resModel.AttackCD * effect;
            break;
        default:
            Debug.Assert(false, "error trick property");
            break;
        }

        return resModel;
    }

    List<TrickModel> GetTrickingModels(){
        List<TrickModel> models = new List<TrickModel>();
        foreach(int id in trickingIds){
            TrickModel m = trickConfig.GetModel(id);
            Debug.Assert(m != null, "error trick id");
            models.Add(m);
        }
        return models;
    }


    public void OnTrickEvent(object sender, EventArgs e){
        TrickEvent ev = (TrickEvent)e;
        // 如果发送放和接收方相等，且是对自己产生作用的特技的话
        bool condition1 = IsMy == ev.IsMy && ev.IsSelf;
        // 或者如果发送放和接收方不等，且是对别人产生作用的特技的话
        bool condition2 = IsMy != ev.IsMy && !ev.IsSelf;
        bool canTrigger = condition1 || condition2;
        if(ev.IsStart){
            Debug.Log("can trigger trick " + canTrigger + ": " + IsMy + "," + ev.IsMy + "," + ev.IsSelf);
            if(canTrigger){
                Debug.Assert(ev.Tricks.Length == 1, "error trick length");
                AddTrickBuff(ev.Tricks[0]);
                Debug.Log((IsMy?"my ":"enemy ")+"base:     " + model);
                Debug.Log((IsMy?"my ":"enemy ")+"start id" + ev.Tricks[0] + ": " + Model);
            }
        }else{
            RemoveTrickBuff(ev.Tricks);
            foreach(int id in ev.Tricks){
                Debug.Log((IsMy?"my ":"enemy ") + "stop  id" + id + ": " + Model);
            }
        }
    }

    // 加应该是单个的，一个个处理，而移除却是一群的，而且还有可能在敌方。。。卧槽。。。
    void AddTrickBuff(int trickId){
        if(TroopValid(ConfigManager.share().TrickConfig.GetModel(trickId).Target)){
            trickingIds.Add(trickId);
        }
    }

     bool TroopValid(TroopType[] target){
        foreach(TroopType t in target){
			if(Model.Type == t || t == TroopType.TROOP_ALL){
                return true;
            }
        }
        return false;
    }

    void RemoveTrickBuff(int[] trickIds){
        foreach(int id in trickIds){
            trickingIds.Remove(id);
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
