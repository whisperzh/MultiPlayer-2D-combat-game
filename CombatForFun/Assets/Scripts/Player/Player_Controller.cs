using InControl;
using UnityEngine;
using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine.UI;

public class Player_Controller : MonoBehaviour
{
    #region 设备输入
    [HideInInspector] public bool deviceisnull = true;
    [HideInInspector] public System.Guid DeviceserialNumber;
    [HideInInspector] public InputDevice memoryDevice;
    [HideInInspector] public InputDevice device { get; set; }
    #endregion
    [Header("组件属性")]
    #region 组件属性
    //public SpriteRenderer cachedRenderer;
    public Weapon_Controller weaponControl;
    private GameObject _kickingTarget;
    private Rigidbody2D _m_Rigidbody2D;
    private Animator _m_Animator;
    public Animator _sheild_Animator;
    private Transform WeaponDefaultPos;
    public RectTransform PlayerUI;
    public Slider _healthSlider;
    //public Slider _armorSlider;
    #endregion

    [Header("玩家属性")]
    #region 玩家属性  
    // 血量 护盾
    public bool GodMode;
    public float health = 100f;
    public float armor = 50f;
    private float _armorRefillTimer;
    private float defaultArmor;
    public float _waitforSecondstoRefillArmor;
    public float armorRecoverRate;

    // 移动
    public float movingSpeed;
    // 踢     
    public LayerMask KickDetect;
    public float kickingForce;
    public float kickCDInterval;
    public float kickingRadius;
    //翻滚
    [Range(1,4)] public float rollSpeed;
    public float rollCD = 0.5f;
    private float rollTime = 0;
    private Vector2 rollDir;
    //移动方向
    private Vector2 moveDir;
    public Vector2 MoveDir
    {
        get
        {
            return moveDir;
        }
    }
    #endregion

    [Header("玩家动画")]
    #region 玩家动画  
    public SpriteRenderer Shield;
    public SpriteRenderer Ps_Body;
    public SpriteRenderer Ps_Head;
    public SpriteRenderer Ps_HeadRoll;
    public SpriteRenderer Ps_Roll;
    public SpriteRenderer Shaodow;
    public GameObject GrenadaPic;

    public Sprite PlayerDieSprite;

    private int _headRollAngle ;
    public int HeadRollAngle
    {
        set
        {
            _headRollAngle = value;
        }
    }
    #endregion

    [Header("玩家材质")]
    #region 玩家材质
    public Shader PlayerShieldShader;
    public Shader PlayerShader;
    [Range(0,40)]
    public float OutlineWidth = 0;
    [Range(0, 1)]
    public float HurtIntensity = 0;
    public bool IsDisplay = true;
    private Material Mat_Shield;
    private Material Mat_Player;
    #endregion

    #region 判断
    private bool _buttonPressed_pick = false;
    private bool _canMove;
    private bool _cankick;
    private bool _canRoll;
    private bool _isBaseWeapon;
    private bool _isDead = false;
    private bool _canbeDamaged;
    private bool _isMove,_isRoll;
    private bool _isFaceRight,_isMoveRight;
    private bool _ifTakeDamage;
    //Triggle
    private bool _rollTriggle;

    //get _isMove 参数
    public bool isMove
    {
        get
        {
            return _isMove;
        }
    }
    public bool isFaceRight
    {
        get
        {
            return _isFaceRight;
        }
    }
    #endregion

    private void DisableAllActivities() {
        _canMove = false;
        _cankick = false;
        _canRoll = false;
        _canbeDamaged = false;
        _isMove = false;
        _isRoll = false;
        _ifTakeDamage = false;
        _isDead=true;
        device = null;
    }

    private void Awake()
    {
        //创建玩家材质
        Mat_Player = new Material(PlayerShader);
        Mat_Shield = new Material(PlayerShieldShader);
        //初始化
        Mat_Player.SetFloat("_OutlineWidth", OutlineWidth);
        Mat_Player.SetFloat("_HurtIntensity", HurtIntensity);
        Mat_Player.SetFloat("_IsDisplay", IsDisplay ? 1 : 0);

        Mat_Shield.SetFloat("_DisIntensity", 1);
        Mat_Shield.SetFloat("_HurtIntensity", 0);
        Mat_Shield.SetFloat("_Strength", 1);
        //赋值材质
        Ps_Head.material = Mat_Player;
        Ps_Body.material = Mat_Player;
        Ps_HeadRoll.material = Mat_Player;
        Ps_Roll.material = Mat_Player;
        Shield.material = Mat_Shield;
    }

    void Start()
    {
        _armorRefillTimer = _waitforSecondstoRefillArmor;
        defaultArmor = armor;
        _ifTakeDamage = false;
        _healthSlider.maxValue = health;
        _healthSlider.value = health;
        //_armorSlider.maxValue = armor;
        //_armorSlider.value = armor;

        _isBaseWeapon = true;
        _isMove = false;
        _isRoll = false;
        _isFaceRight = false;

        _rollTriggle = false;

        _canMove = true;
        _cankick = true;
        _canRoll = true;
        _canbeDamaged = true;
        _m_Rigidbody2D = GetComponent<Rigidbody2D>();
        _m_Animator = GetComponent<Animator>();
        //_sheild_Animator = Shield.GetComponent<Animator>();

        weaponControl._canShoot = true;
        PlayerFilp(true);
        GrenadaPic.SetActive(false);
        WeaponDefaultPos = weaponControl.transform;
    }

    private void Update()
    {
        PlayerAnim();
        PlayerMatUpdate();
    }

    private void FixedUpdate()
    {
        if(!_isDead)
            ArmorRecover();

        if (device != null)
        {
            GetJoypad();
            //GetKickingTarget();
            GetMoveDir();
        }
        else
        {
            deviceisnull = true;
        }

        Roll();

        //如果可以，把这行功能移到合适的地方  或者不要动
        GrenadaPic.SetActive(!_isRoll && weaponControl.HasGrenade);
    }

    /// <summary>
    /// 输入检测
    /// </summary>
    void InputScan()
    {
        if (_canMove)
        {
            moveDir = new Vector3(device.LeftStick.X, device.LeftStick.Y, 0);
            _isMove = moveDir.magnitude == 0 ? false : true;
            _m_Rigidbody2D.velocity = (moveDir * movingSpeed);
        }
        else
        {
            moveDir = Vector2.zero;
        }

        //if (_cankick && device.Action1.IsPressed && !device.Action1.LastState)
        //    Kick();

        if (_canRoll && device.LeftBumper.IsPressed)
            RollCheck();
        if (device.Action2.IsPressed && !_buttonPressed_pick)
        {
            _buttonPressed_pick = true;
            if (_isBaseWeapon)
            {
                weaponControl.SeekForPerspectiveWeapon();
                if (!weaponControl.IsDetectWeapon)
                    return;
                _isBaseWeapon = false;
                weaponControl.PickupWeapon();
            }
            else
            {
                weaponControl.SeekForPerspectiveWeapon();
                if (!weaponControl.IsDetectWeapon)
                {
                    _isBaseWeapon = true;
                    weaponControl.ThrowAwayWeapon();
                }
                else
                {
                    weaponControl.ReplaceWeapon();
                }       
            }
        }
        if (!device.Action2.IsPressed)
        {
            _buttonPressed_pick = false;
        }
    }

    /// <summary>
    /// 连接上手柄之后做的操作
    /// </summary>
    void GetJoypad() {
        deviceisnull = false;
        //Set object material color based on which action is pressed.
        //cachedRenderer.material.color = Color.white;
        InputScan();
    }

    void GetMoveDir()
    {
        _isMoveRight = device.LeftStick.X > 0 ? true : false;
    }

    //void Kick() {
    //    if (_kickingTarget != null)
    //    {
    //        _kickingTarget.GetComponent<Rigidbody2D>().
    //             AddForce(kickingForce *
    //             (_kickingTarget.transform.position - transform.position).normalized,
    //             ForceMode2D.Impulse);
    //    }
    //    else {
    //        return;
    //    }
    //    _cankick = false;
    //    _canMove = false;
    //    _canRoll = false;
    //    weaponControl._canShoot = false;

    //    My_Tools.Tools.SetTimeOut(1000*0.02,delegate 
    //    { _canMove = true;
    //        _canRoll = true;
    //        weaponControl._canShoot = true;
    //    });//限制行动回复

    //    My_Tools.Tools.SetTimeOut(1000 * kickCDInterval, delegate
    //    { _cankick = true; });//踢腿冷却

    //    _kickingTarget = null;
    //}

    ///// <summary>
    ///// 得到要踢的物体
    ///// </summary>
    //void GetKickingTarget() {
    //    Collider2D[] m_cCollider = Physics2D.OverlapCircleAll(transform.position,
    //        kickingRadius, KickDetect);
    //    if (m_cCollider == null)
    //    {
    //        _kickingTarget = null;
    //        return;
    //    }
    //    float MaxDixt = 100;
    //    foreach (Collider2D i in m_cCollider)
    //    {
    //        if (i == this.GetComponent<Collider2D>())
    //            continue;

    //        if (MaxDixt > Vector3.Distance(i.transform.position, transform.position))
    //        {
    //            MaxDixt = Vector3.Distance(i.transform.position, transform.position);
    //            _kickingTarget = i.gameObject;
    //        }
    //    }
    //}

    /// <summary>
    /// 翻滚 检测
    /// </summary>
    void RollCheck()
    {
        rollDir = new Vector2(device.LeftStick.X, device.LeftStick.Y);

        //如果玩家没有位移 则不能翻滚
        if (rollDir.magnitude < 0.2f)
            return;

        //开始翻滚
        _isRoll = true;
        PlayerBehaviorControl(false);
        _rollTriggle = true;

        Action a1 = delegate {
            PlayerBehaviorControl(true);
            _isRoll = false;
            _canRoll = false;
        };//限制条件解除
        My_Tools.Tools.SetTimeOut(600, a1);

        Action a2 = delegate {
            _canRoll = true;
        };//限制条件解除
        My_Tools.Tools.SetTimeOut(600 + rollCD * 1000, a2);
    }

    /// <summary>
    /// 翻滚
    /// </summary>
    void Roll()
    {
        if (_isRoll)
        {
            rollDir.Normalize();
            rollTime = rollTime + Time.deltaTime;
            float par = My_Tools.Tools.RollParameterForeachFrame(rollTime, rollSpeed, 0.3f/3, 0.6f/3, 0.8f);
            _m_Rigidbody2D.velocity = rollDir * par * 4;
        }
        else
        {
            rollTime = 0;
        }
    }

    /// <summary>
    /// 玩家行动限制
    /// </summary>
    /// <param name="IsActive"></param>
    private void PlayerBehaviorControl(bool IsActive)
    {
        weaponControl._canCheckDir = IsActive;
        weaponControl._canShoot = IsActive;
        _canMove = IsActive;
        _canRoll = IsActive;
        _cankick = IsActive;
        _canbeDamaged = IsActive;
    }

    public void Hurt(float damage)
    {
        if (_canbeDamaged && !GodMode)
        {
            My_Tools.Tools.DeviceViberation(device, 0.3, new Vector2(3, 3));
            Debug.Log("玩家" + gameObject.name + "受伤");
            _armorRefillTimer = _waitforSecondstoRefillArmor;
            if (armor - damage <= 0)
            {
                if(armor > 0)
                {
                    _sheild_Animator.SetTrigger("Broken");
                }
                float remainingDamage = armor - damage;
                armor = 0;
                //_armorSlider.value = armor;
                health += remainingDamage;
                _m_Animator.SetTrigger("Hurt");
                _healthSlider.value = health;
            }
            else//伤害不够打破护盾
            {
                _sheild_Animator.SetTrigger("Attack");
                armor -= damage;
                //_armorSlider.value = armor;
            }
            if (health <= 0)
            {
                Die();
                //health = 100;
            }      
        }
    }

    /// <summary>
    /// 人物翻转 
    /// </summary>
    /// <param name="isFaceLeft">是否朝左</param>
    public void PlayerFilp(bool isFaceRight)
    {
        //Debug.Log("反转");
        Ps_Body.flipX = isFaceRight;
        Ps_Head.flipX = isFaceRight;
        Ps_HeadRoll.flipX = isFaceRight;
        Shaodow.flipX = isFaceRight;
        GrenadaPic.transform.localScale = new Vector2(1 * (isFaceRight ? 1 : -1), 1);
        PlayerUI.localScale = new Vector2(1 * (isFaceRight ? 1 : -1), 1);
        _isFaceRight = isFaceRight;
    }

    /// <summary>
    /// 角色动画
    /// </summary>
    private void PlayerAnim()
    {
        //移动
        _m_Animator.SetBool("isMove", _isMove);

        //正走 倒走
        int moveDir = (_isFaceRight ? 1 : -1) * (_isMoveRight ? 1 : -1);
        _m_Animator.SetFloat("MoveDir",moveDir);

        //护盾
        Mat_Shield.SetFloat("_Strength", armor / defaultArmor);

        //翻滚
        if(_rollTriggle)
        {
            //Debug.Log("翻滚动画! ");
            _m_Animator.SetTrigger("Roll");
            Ps_Roll.flipX = device.LeftStick.X > 0 ? true : false;
            _rollTriggle = false;
        }

        //武器能够旋转的时候才能旋转头部
        if (!(-20 < _headRollAngle && _headRollAngle < 20) && weaponControl._canCheckDir)
        {
            _m_Animator.SetFloat("HeadAngle", _headRollAngle);
            Ps_Head.gameObject.SetActive(false);
            Ps_HeadRoll.gameObject.SetActive(true);
        }
        else
        {
            _m_Animator.SetFloat("HeadAngle", 0);
            Ps_Head.gameObject.SetActive(true);
            Ps_HeadRoll.gameObject.SetActive(false);
        }
    }

    private void PlayerMatUpdate()
    {
        Mat_Player.SetFloat("_OutlineWidth", OutlineWidth);
        Mat_Player.SetFloat("_HurtIntensity", HurtIntensity);
        Mat_Player.SetFloat("_IsDisplay", IsDisplay ? 1 : 0);
    }

    /// <summary>
    /// 手柄震动
    /// </summary>
    public void DeviceViberate_Roll()
    {
        My_Tools.Tools.DeviceViberation(device, 0.1, new Vector2(0.6f, 0.6f));
    }

    private void ArmorRecover()
    {
        if (_armorRefillTimer <= 0)
        {
            if (armor < defaultArmor)
            {
                armor += Time.deltaTime * armorRecoverRate;
                //_armorSlider.value = armor;
            }
            else
            {
                armor = defaultArmor;
            }
        }
        else
        {
            _armorRefillTimer -= Time.deltaTime;
        }
    }

    public bool CollectGrenade() {
        if (!weaponControl.HasGrenade)
        {
            weaponControl.PlayPickGrenadaAudio();
            weaponControl.HasGrenade = true;
            GrenadaPic.SetActive(true);
            return true;
        }
        return false;
    }

    public void TemporarilyCannotMove(float WaitforSecondsToMove) {
        PlayerBehaviorControl(false);
        _canbeDamaged = true;
        My_Tools.Tools.SetTimeOut(WaitforSecondsToMove*1000, delegate {
            PlayerBehaviorControl(true);
        });//限制条件解除)
    }

    public bool IsPlayerRolling() { return _isRoll; }

    private void Die() {
        GameObject tempDie = new GameObject();
        tempDie.name = "玩家遗骸";
        tempDie.AddComponent<SpriteRenderer>().sprite = PlayerDieSprite;
        tempDie.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        tempDie.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f, 1);
        tempDie.transform.position = transform.position;

        GameObject.Find("PlayerDistributionManager").GetComponent<Player_Input>().PlayerDeclaredDearth(device);
        DisableAllActivities();
        weaponControl.gameObject.SetActive(false);
    }

    public void ResetPlayerSprite()
    {
        Ps_Head.sprite = null;
        Ps_Body.sprite = null;
        Ps_HeadRoll.sprite = null;
        Ps_Roll.sprite = null;
    }
}

    
