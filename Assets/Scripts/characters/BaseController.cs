using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using System;

public abstract class BaseController : MonoBehaviour {
    public TroopType troopType;
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

    public Vector3 startPos;
    public Vector3 endPos;
    public bool IsMy{get;set;}
    protected void InitModel(){
        // 不然直接影响到配置的值，卧槽，这怎么会返回引用啊，日了狗了
        Model = (BaseModel)ConfigManager.share().CharacterConfig.GetModel(troopType).Clone();
        // Debug.Log(Model);
    }
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
    protected CharacterConfig characterConfig = ConfigManager.share().CharacterConfig;

    public GameObject skillTip;

    public enum TroopStatus{
        STATUS_IDLE, STATUS_FORWARD, STATUS_ATTACK, STATUS_BACK, STATUS_DEAD,
    }

    protected TroopStatus status;
    public TroopStatus Status{
        get{return status;}
        set{ 
            // 关闭上一个状态的产生的Trick
            HandleStatusTrick(false); 
            status = value;
            // 开始新状态的trick，被攻击等情况额外处理
            HandleStatusTrick(true); 
        }
    }

    protected enum TroopCommand{
        CMD_IDLE, CMD_FORWARD, CMD_ATTACK, CMD_BACK,
    }

	public TrickController TrickController{ get; set; }
    protected void InitTrickController(){
        TrickController = new TrickController (Model.Tricks);
    }

    public Rigidbody2D flyWeapon;
    // public float flyWeaponSpeed = 20.0f; 
    protected ExplodeEvent explodeEvent = new ExplodeEvent();
    public event EventHandler ExplodeEventHandler;



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
        HandleCommand(TroopCommand.CMD_IDLE);

        Invoke("DispatchNaturalTricks", 0.1f); //不能马上调用，因为这个时候可能别的basecontroller还没有Start，也就还没有Initmodel了
        // InvokeRepeating("AddTrickLifeTimer", 1, 1.0f);
        // Invoke("Test", 5);

        skeletonAnimation.state.Start += delegate (Spine.AnimationState state, int trackIndex) {
             // 你也可以使用一个匿名代理
            Debug.Log(string.Format("track {0} started a new animation.", trackIndex));
        };
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
            //     HandleCommand(TroopCommand.CMD_ATTACK);
            // }
            if(attackedTarget = FindAttackedTarget()){
                HandleCommand(TroopCommand.CMD_ATTACK);
            }
            break;
        case TroopStatus.STATUS_ATTACK:
            break;
        case TroopStatus.STATUS_BACK:
            transform.position = Vector3.Lerp(transform.position, startPos, (float)(1/((transform.position - startPos).magnitude) * Model.Speed));
            if(transform.position == startPos){
                HandleCommand(TroopCommand.CMD_IDLE);
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

    void HandleCommand(TroopCommand cmd){
        // Debug.Log("HandleCommand " + cmd);
        // 如果死了，就不能再接受任何命令了
        if(Status == TroopStatus.STATUS_DEAD){
            return;
        }
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

    TrickStatusType GetTrickStatusType(){
        TrickStatusType type = TrickStatusType.STATUS_INVALID;
        switch(Status){
        case TroopStatus.STATUS_IDLE:
            type = TrickStatusType.STATUS_IDLE;
            break;
        case TroopStatus.STATUS_FORWARD:
        case TroopStatus.STATUS_BACK:
            type = TrickStatusType.STATUS_MOVE;
            break;
        case TroopStatus.STATUS_DEAD:
            type = TrickStatusType.STATUS_DEAD;
            break;
        case TroopStatus.STATUS_ATTACK:
            type = TrickStatusType.STATUS_ATTACK;
            break;
        default:
            Debug.Assert(false, "error status in handle status trick");
            break;
        }
        return type;
    }
    
    void HandleStatusTrick(bool isStart){
        TrickStatusType type = GetTrickStatusType();
        DispatchStatusTricks(type, isStart);
    }


    void DispatchStatusTricks(TrickStatusType type, bool isStart){
        // 像那种什么自己爆炸的效果啥的，还不如在SpyController里面重写dead然后炸了，这种是固定触发的，没必要也来这里分发，复杂死了。。。
        // 按理说，那种瞬时的且不改变属性的那种特教，都不应该放这里分发
        if(!isStart){
            RemoveLastStatusBuff();
        }

        List<int> trickIds = TrickController.OnStatusTrick(type, isStart);
        MyTroopController.DispatchTricks(trickIds, isStart);
        // Debug.Log(type + ", " + isStart + ", count:" + trickIds.Count);
        /*List<int> needDispatchIds = new List<int>();
        foreach(int id in trickIds){
            // 如果目标是自己（注意是自己，不是自己类型，这是两个概念），就不转发了
            TrickModel m = trickConfig.GetModel(id);
            bool isSelfCall = m.Target.Length == 1 && m.Target[0] == TroopType.TROOP_SELF;
            if(isSelfCall){
                // Debug.Log("self call: " + id);
                if(isStart){
                    bool isRightOppo = false;
                    if(attackedTarget != null){
                        TroopType oppoType = attackedTarget.GetComponent<BaseController>().Model.Type;
                        foreach(TroopType t in m.Opponent){
                            if(t == oppoType){
                                isRightOppo = true;
                                break;
                            }
                        }
                    }
                    if(isRightOppo){
                        AddTrickBuff(id);
                    }
                }else{
                    RemoveTrickBuff(id);
                }
            }else{
                needDispatchIds.Add(id);
            }
        }
        MyTroopController.DispatchTricks(needDispatchIds, isStart);*/
    }

    void RemoveLastStatusBuff(){
        List<int> removeIds = new List<int>();
        foreach(int id in trickingIds){
            TrickModel m = trickConfig.GetModel(id);
            if(m.Type == TrickType.TRICK_STATUS && m.Status == GetTrickStatusType()){
                // RemoveTrickBuff(id);
                removeIds.Add(id);
            }
        }
        RemoveTrickBuff(removeIds.ToArray());
    }

    void Forward(){
        Status = TroopStatus.STATUS_FORWARD;
        skeleton.FlipX = !IsMy;
        spineAnimationState.SetAnimation(0, walkAnimationName, true);
    }

    void Attack(){
        Status = TroopStatus.STATUS_ATTACK;
        spineAnimationState.SetAnimation(0, shootAnimationName, false);
        if(IsMy){
            // Debug.Log("my x:" + transform.position.x + " target x:" + attackedTarget.transform.position.x);
        }
        Invoke("DoBackDelay", 0.5f); // 注意这个时间应该要大于攻击cd
        attackedTarget.GetComponent<BaseController>().Attacker = this.gameObject;
        // 这里的beingattacked应该放在必要的动画碰撞时候，比如投弹，弓箭之类
        if(!IsNeedFlyWeapon()){
            attackedTarget.GetComponent<BaseController>().BeingAttacked();
        }else{
            CreateFlyWeapon();
        }
        // attackedTarget = null;
    }

    protected virtual void CreateFlyWeapon(){
        Rigidbody2D weapon;
        if(IsMy)
        {
            weapon = Instantiate(flyWeapon, transform.position, Quaternion.Euler(new Vector3(0,0,0))) as Rigidbody2D;
            weapon.velocity = new Vector2(weapon.GetComponent<BaseFlyWeapon>().GetSpeed(), 0);
        }
        else
        {
            weapon = Instantiate(flyWeapon, transform.position, Quaternion.Euler(new Vector3(0,0,180f))) as Rigidbody2D;
            weapon.velocity = new Vector2(-weapon.GetComponent<BaseFlyWeapon>().GetSpeed(), 0);
        }
        BaseFlyWeapon controller = weapon.GetComponent<BaseFlyWeapon>();
        controller.Owner = this;
        controller.IsMy = IsMy;
        controller.SettingFin = true;
    }

    protected bool IsNeedFlyWeapon(){
        return (Model.Type == TroopType.TROOP_ARCHER);
    }

    public virtual void BeingAttacked(int directHarm = -1){
        // 之前没加这个判断，结果两个spyer互爆，无限循环dead.....
        if(Status == TroopStatus.STATUS_DEAD){
            return;
        }
        // Debug.Log("BeingAttacked");
        DispatchStatusTricks(TrickStatusType.STATUS_DEFENSE, true);
        // 有直接伤害的时候不能miss，directHarm放前面去短路，如果是直接伤害就没必要判断对方的model来取到hitRate算miss率了，避免一定程度的对象已死获取不到的问题
        if( directHarm == -1 && CanMiss()){
            // miss anim
            // Debug.Log("attack miss");
        }else{
            int harm = 0;
            if(directHarm != -1){
                harm = directHarm;
                // Debug.Log(Model.Type + ": direct harm: " + harm);
            }else{
                harm = CalcHarm();
                // Debug.Log("calc total harm: " + harm);
            }
            // Debug.Log("Life: " + Model.Life);
            // 注意！！只有生命值这个玩意必须直接改model底层数据，而不能通过Model，因为这个取到的是副本，改也只是在副本上改
            // Model.Life -= harm;
            model.Life -= harm;
            if(IsMy){
                // Debug.Log("Life : " + Model.Life + ", harm:-" + harm);
            }
            if(Model.Life <= 0){
                Dead();
            }
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.0f, transform.position.z);
        }
        DispatchStatusTricks(TrickStatusType.STATUS_DEFENSE, false);
    }

    protected virtual int CalcHarm(){
        float att = Attacker.GetComponent<BaseController>().Model.Attack;
        float def = Model.Defense;
        int res = (int)(att * (att / (att + def)));
        // Debug.Log((IsMy?"my ":"enemy ") + Model.Type + " CalcHarm: " + res + "  ->  " + "att: " + att + ", def:" + def);
        Debug.Assert(res >= 0, "CalcHarm error");
        return res;
    }

    protected virtual bool CanMiss(){
        return UnityEngine.Random.value > Attacker.GetComponent<BaseController>().Model.HitRate; 
    }

    protected virtual void Dead(){
        Debug.Log("dead " + (IsMy?"my ":"enemy ")  + Model.Type );
        model.Life = 0;
        Status = TroopStatus.STATUS_DEAD;
        // IsDead = true; // 这个应该放在anim完成的回调里做，现在先自己手动延时调用
        Invoke("MarkCanClear", 1);
        spineAnimationState.SetAnimation(0, dieAnimationName, false);
    }

    void MarkCanClear(){
        IsDead = true;
    }

    // 需要处理的碰撞信息，放在被撞的物体身上
    void OnParticleCollision(GameObject other) {
        Debug.Log("collision");     
    }

    void Back(){
        Status = TroopStatus.STATUS_BACK;
        skeleton.FlipX = IsMy;
        spineAnimationState.SetAnimation(0, walkAnimationName, true);
    }

    void Idle(){
        Status = TroopStatus.STATUS_IDLE;
        skeleton.FlipX = !IsMy;
        spineAnimationState.SetAnimation(0, idleAnimationName, true);
        Invoke("DoForwardDelay", (float)Model.AttackCD);
    }

    void DoBackDelay () {
        HandleCommand(TroopCommand.CMD_BACK);
    }

    void DoForwardDelay () {
        HandleCommand(TroopCommand.CMD_FORWARD);
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
            if(skillBuffModel.Type == SkillType.SKILL_LIFE){
                AddSkillLifeBuff(isBuffing);   
            }
            List<int> trickIds = TrickController.GetSkillTrick(ev.Type);
            MyTroopController.DispatchTricks(trickIds, isBuffing);

            Debug.Log("AFTER SKILL&TRICK: " + Model);
        }
    }

    protected BaseModel AddSkillModel(){
        BaseModel resModel = (BaseModel)model.Clone(); // 用Model，就无限递归了。。。，还有，不能用model，因为是引用，直接修改model的值了。。fuck
        resModel.AttackMin = (int)(resModel.AttackMin * (1.0 + skillBuffModel.Attack));
        resModel.AttackMax = (int)(resModel.AttackMax * (1.0 + skillBuffModel.Attack));
        resModel.Defense = (int)(resModel.Defense * (1.0 + skillBuffModel.Defense));
        resModel.HitRate = resModel.HitRate * (1.0 + skillBuffModel.HitRate);
        if(resModel.HitRate > 1.0){
            resModel.HitRate = 1.0;
        }
        // Debug.Log("buff: attack " + resModel.Attack);
        return resModel;
    }

    protected void AddSkillLifeBuff(bool isStart){
        if(isStart){
            if(!IsInvoking("AddSkillLifeTimer")){
                InvokeRepeating("AddSkillLifeTimer", 0, 1.0f);
            }else{
                Debug.Assert(false, "call skill life timer twice");
            }
        }else{
            CancelInvoke("AddSkillLifeTimer");
        }
    }

    protected void AddSkillLifeTimer(){
        Debug.Assert(isBuffing && skillBuffModel != null, "what the fuck life buff ?");
        BaseModel config = characterConfig.GetModel(model.Type);
        int skillLife = skillBuffModel.Life;
        int trickLife = 0;
        foreach(int id in trickingIds){
            TrickModel m = trickConfig.GetModel(id);
            if(m.Property == TrickProperty.PROPERTY_LIFE){
                trickLife += (int)m.Effect;
            }
        }
        int buffLife = skillLife + trickLife;
        // 一开始这里log还选了collapse，所以特么我还说为毛后面部队的timer怎么不跑了。。。orz
        Debug.Log(Model.Type + ": " + model.Life + " skilllife+" + skillBuffModel.Life + ", tricklife+" + trickLife);
        model.Life += buffLife;
        if(model.Life > config.Life){
            model.Life = config.Life;
        }

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
        // 注意这里不能用Model去隐式调用get方法，这样会无限递归了。。。
        BaseModel resModel = (BaseModel)originModel.Clone();
		double effect = trickModel.Effect;
        // effect都是固定值，而不是百分比，直接加
		switch(trickModel.Property){
        case TrickProperty.PROPERTY_ATTACK:
            resModel.AttackMin += (int)effect;
            resModel.AttackMax += (int)effect;
            break;
        case TrickProperty.PROPERTY_DEFENSE:
            resModel.Defense += (int)effect;
            break;
        case TrickProperty.PROPERTY_SPEED:
            resModel.Speed += (int)effect;
            break;
        case TrickProperty.PROPERTY_HIT:
            resModel.HitRate = resModel.HitRate * effect;
            if(resModel.HitRate > 1.0){
                resModel.HitRate = 1.0;
            }        
            break;
        case TrickProperty.PROPERTY_LIFE:
            // 已经在AddTrickLifeTimer里处理了

            // // 固定回复值，注意配置是10.0，这个我估计得特殊处理，每秒回复。。费劲啊，开一个timer去invoke调用，记得timer要清理！！！！
            // resModel.Life += (int)effect;
            // int maxLife = ConfigManager.share().CharacterConfig.GetModel(model.Type).Life;
            // if(resModel.Life > maxLife){
            //     resModel.Life = maxLife;
            // }
            // // 而且注意血量是特么需要重置到自己原本的model的！！！日，不能这样直接复制，会重复叠加了。。。咦？好像可以？
            // model.Life = resModel.Life;
            break;
        case TrickProperty.PROPERTY_ATTACK_CD:
            resModel.AttackCD += (int)effect;
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
            // Debug.Log("can trigger trick " + canTrigger + ": " + IsMy + "," + ev.IsMy + "," + ev.IsSelf);
            if(canTrigger){
                Debug.Assert(ev.Tricks.Length == 1, "error trick length");
                if(TroopValid(ConfigManager.share().TrickConfig.GetModel(ev.Tricks[0]).Target)){
                    AddTrickBuff(ev.Tricks[0]);
                }
            }
        }else{
            RemoveTrickBuff(ev.Tricks);
            // foreach(int id in ev.Tricks){
            //     Debug.Log((IsMy?"my ":"enemy ") + "stop  id" + id + ": " + Model);
            // }
        }
    }

    // 加应该是单个的，一个个处理，而移除却是一群的，而且还有可能在敌方。。。卧槽。。。
    void AddTrickBuff(int trickId){
        trickingIds.Add(trickId);
        Debug.Log((IsMy?"my ":"enemy ")+"base:     " + model);
        Debug.Log((IsMy?"my ":"enemy ")+"start id" + trickId + ": " + Model);

    }

    void AddTrickBuff(int[] trickIds){
        foreach(int id in trickIds){
			AddTrickBuff(id);
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
            // trickingIds.Remove(id);
            RemoveTrickBuff(id);
        }
    }

    void RemoveTrickBuff(int trickId){
		trickingIds.Remove(trickId);
		Debug.Log((IsMy?"my ":"enemy ") + "stop  id" + trickId + ": " + Model);

    }

    public bool IsFlyCategory(){
        return DemoUtil.GetTroopCategory(Model.Type) == TroopCategory.CATEGORY_FLY;
    }

    public bool IsRemoteCategory(){
        return DemoUtil.GetTroopCategory(Model.Type) == TroopCategory.CATEGORY_REMOTE;
    }


    protected void SendExplodeEvent(Vector3 center, double radius, bool isMy, int harm){
        explodeEvent.Center = center;
        explodeEvent.Radius = radius;
        explodeEvent.IsMy = isMy;
        explodeEvent.Harm = harm;
        ExplodeEventHandler(this, explodeEvent);
    }




    // spine  line -----------------------------------------------------------------------------------------------------------

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

    [SpineAnimation]
    public string dieAnimationName;
    #endregion

    SkeletonAnimation skeletonAnimation;

    // Spine.AnimationState and Spine.Skeleton are not Unity-serialized objects. You will not see them as fields in the inspector.
    public Spine.AnimationState spineAnimationState;
    public Spine.Skeleton skeleton;
    [SpineEvent] public string dieEventName = "die"; 


    
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
