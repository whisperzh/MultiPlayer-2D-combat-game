using InControl;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Weapon_Controller : MonoBehaviour
{
    #region 设备输入
    public InputDevice device;
    #endregion

    [Header("枪械")]
    public Weapons DefWeapon;
    public LayerMask WeaponDetect;
  //  public LineRenderer line;
    public AudioClip AC_PickWeapon;

    private Weapons CurWeapon;
    private Weapons _Weapon_perspective;
    private Weapons _Weapon_discard;

    [Header("手雷")]
    public HandGrenade GrenadePrefab;
    public RuntimeAnimatorController ThrowGrenadeAnim;
    public AudioClip AC_PickGrenada;
    public AudioClip AC_AwakeGrenada;

[Header("组件属性")]
    #region 组件属性
    public Transform BulletSpawningPoint;

    public Slider currentMagzineSlider;
    private SpriteRenderer WeaponSprite;
    private Animator WeaponAnim;
    private Player_Controller player;
    private Transform _bulletDefaultSpawningPoint;
    private AudioSource _weaponAudio;
    //private HandGrenade GrenadeSlot;
    #endregion
    //开火冷却时间
    private float _fireCDTime = 0;
    private float _delayCDTime = 0;

    [Header("粒子效果")]
    public ParticleSystem Par_EmptyAmmo;
    public ParticleSystem Par_BulletShell;
    private ParticleSystem.MainModule parMain_BS;

    #region 工具bool
    
    public bool HasGrenade
    {
        set
        {
            hasGrenade = value;
        }
        get
        {
            return hasGrenade;
        }
    }
    public bool IsDetectWeapon
    {
        get
        {
            return _Weapon_perspective != null ? true : false;
        }
    }
    [HideInInspector] public bool Reloaded = false;
    [HideInInspector] public bool IsFire = false;
    [HideInInspector] public bool _canShoot;
    [HideInInspector] public bool _canCheckDir;
    [HideInInspector] public bool _reloading_Aborted;
    private bool hasGrenade = false;
    private bool _semi_Auto_Trigger;
    #endregion

    public void Start()
    {
        CurWeapon = DefWeapon;
        CurWeapon.CurrentMagzine = CurWeapon.MagzineCapacity;

        WeaponSprite = transform.Find("WeaponAnim").GetComponent<SpriteRenderer>();
        WeaponAnim = transform.Find("WeaponAnim").GetComponent<Animator>();
        WeaponAnim.runtimeAnimatorController = CurWeapon.WeaponAnimController;

        _reloading_Aborted = false;
        _semi_Auto_Trigger = true;
        _canCheckDir = true;

        try
        {
            player = GetComponentInParent<Player_Controller>();
        }
        catch (Exception e) { }

        parMain_BS = Par_BulletShell.main;
        parMain_BS.maxParticles = 0;

        _weaponAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void Update()
    {

        if (device != null)
        {
            currentMagzineSlider.maxValue = CurWeapon.MagzineCapacity;
            UpdateCurrentMagzineText();
            Vector2 inputDir = new Vector2(device.RightStick.X, device.RightStick.Y);
            DirectionInputControl(inputDir);
            DirectionFlipControl();

            //扔出手雷
            if (GrenadeButtonPressed())
            {
                _canShoot = false;
                hasGrenade = false;
                WeaponAnim.runtimeAnimatorController = ThrowGrenadeAnim;
                WeaponAnim.Play("ThrowGrenada");
            }

            if (_fireCDTime >= 0)
                _fireCDTime -= Time.deltaTime;
            else
            {
                IsFire = false;
                if (FirePressed() && !Reloaded)
                {
                    parMain_BS.maxParticles = CurWeapon.MagzineCapacity - CurWeapon.CurrentMagzine;
                    WeaponFire();
                }
                else
                {
                    parMain_BS.maxParticles = 0;
                    
                }
                WeaponAnim.SetBool("IsFire", IsFire);

                if (device.Action3.IsPressed && !Reloaded)
                {
                    parMain_BS.maxParticles = 0;
                    _reloading_Aborted = false;

                    if (CurWeapon.CurrentMagzine != CurWeapon.MagzineCapacity) {
                        PlayReloadAudio();
                        WeaponAnim.SetTrigger("Reload");
                        Invoke("Reload", CurWeapon.ReloadingInterval);
                        Reloaded = true;
                    }        
                }
            }
        }
    }

    private bool FirePressed() {
        bool isPressed = false;
        if (!CurWeapon.Semi_Auto)
        {
            if (device.RightBumper.IsPressed && _canShoot)
                isPressed = true;
        }
        else
        {
            if (device.RightBumper.WasReleased)
                _semi_Auto_Trigger = true;
            if (device.RightBumper.IsPressed && _canShoot && _semi_Auto_Trigger)
            {
                _semi_Auto_Trigger = false;
                isPressed = true;
            }
        }
        return isPressed;
    }

    private bool GrenadeButtonPressed()
    {
        if (hasGrenade&& device.RightTrigger.IsPressed && !player.IsPlayerRolling())
            return true;
        return false;
    }

    //动画事件 -拔手雷
    public void PlayAwakeGrenadaAuido()
    {
        _weaponAudio.PlayOneShot(AC_AwakeGrenada);
    }

    //动画事件 -扔手雷
    public void ThrowGrenada()
    {
        HandGrenade GrenadeSlot = Instantiate(GrenadePrefab, transform.position, Quaternion.identity);
        GrenadeSlot.Throw(transform.right);
        _canShoot = true;
        WeaponAnim.runtimeAnimatorController = CurWeapon.WeaponAnimController;
    }

    /// <summary>
    /// 输入检测
    /// </summary>
    /// <param name="joystick"></param>
    public void DirectionInputControl(Vector2 joystick) {
        //弧度
       float angle = Mathf.Atan2( joystick.y, joystick.x);
        //旋转多少度      
        //Debug.Log("curAngle:" + transform.localEulerAngles.z);
        if (joystick.magnitude > 0.2f)
            transform.localEulerAngles = new Vector3(0, 0, angle * 180 / Mathf.PI);
        else
        {
            var curAngle = transform.localEulerAngles.z;
            var tgtAngle = new Vector3(0, 0, 180);
            if (0 <= curAngle && curAngle < 90)
                tgtAngle = new Vector3(0, 0, 0);
            else if(270 < curAngle && curAngle <= 360)
                tgtAngle = new Vector3(0, 0, 360);
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, tgtAngle, 0.08f);
        }
    }

    public void DirectionFlipControl()
    {
        float angle = transform.localEulerAngles.z;  
        if (_canCheckDir == false)
            return;

        if (angle > 90 && angle <= 270)
        {
            //Debug.Log("不转");
            player.PlayerFilp(false);
            //Par_BulletShell.transform.rotation = Quaternion.Euler(0, 0, 45);
            transform.localScale = new Vector3(1,-1 ,1 );
        }
        else
        {
            //Debug.Log("转"); 
            player.PlayerFilp(true);
            //Par_BulletShell.transform.rotation = Quaternion.Euler(0, 0, 225);
            transform.localScale = new Vector3(1, 1, 1);
        }

        int fixAngle = 0;
        if (0 <= angle && angle < 90)
            fixAngle = (int)angle;
        else if (angle < 270)
            fixAngle = (int)(180 - angle);
        else if (angle < 360)
            fixAngle = (int)(angle - 360);

        player.HeadRollAngle = fixAngle;
        //Debug.Log("fiexAngle = " + fixAngle);
    }

    public void WeaponFire()
    {
        if (CurWeapon.TotalRounds > 0|| CurWeapon.TotalRounds <= -1|| CurWeapon.CurrentMagzine > 0)
        {
            if (CurWeapon.CurrentMagzine > 0)
            {
                StopAllCoroutines();
                //确定开火
                IsFire = true;
                //屏幕抖动
                CameraShake.Instance.ShakeCamera(CurWeapon.CameraSakeIntensity, 0.1f);

                //开火震动 由武器属性决定
                My_Tools.Tools.DeviceViberation(device, CurWeapon.ViberationDuration, CurWeapon.ViberationIntensity);
                //减少当前子弹
                --CurWeapon.CurrentMagzine;
                _fireCDTime = CurWeapon.FireRate;
                //CurWeapon.WeaponGenerateBullet(WeaponSprite.transform.rotation, player, BulletSpawningPoint);
            }
            else
            {
                if (!Reloaded)
                {
                    parMain_BS.maxParticles = 0;
                    _reloading_Aborted = false;
                    if (CurWeapon.CurrentMagzine != CurWeapon.MagzineCapacity)
                    {
                        PlayReloadAudio();
                        WeaponAnim.SetTrigger("Reload");
                        Invoke("Reload", CurWeapon.ReloadingInterval);
                        Reloaded = true;      
                    } 
                }
            }
        }
        else {
            ThrowAwayWeapon();
        }
    }

    public void InitFireBullet()
    {
        //播放开火音效
        PlayFireAudio();
        CurWeapon.WeaponGenerateBullet(WeaponSprite.transform.rotation, player, BulletSpawningPoint);
    }

    public void Reload() {

        if (_reloading_Aborted)//终止换弹
        {
            _reloading_Aborted = false;
            return;
        }
        //Debug.Log("reload");
        if (CurWeapon.CurrentMagzine == CurWeapon.MagzineCapacity) {
            Reloaded = false;
            return;
        }
            
        if (CurWeapon.IsLimitedRounds)
        {
            int neededAmmo = CurWeapon.MagzineCapacity - CurWeapon.CurrentMagzine;
            if (CurWeapon.TotalRounds > neededAmmo)
            {
                CurWeapon.TotalRounds -= neededAmmo;
                CurWeapon.CurrentMagzine += neededAmmo;
            }
            else if( 0 <= CurWeapon.TotalRounds && CurWeapon.TotalRounds <= neededAmmo)
            {
                CurWeapon.CurrentMagzine += CurWeapon.TotalRounds;
                CurWeapon.TotalRounds = 0;
            }
        }
        else
        {
            CurWeapon.CurrentMagzine = CurWeapon.MagzineCapacity;
        }
        UpdateCurrentMagzineText();
        Reloaded = false;
    }

    /// <summary>
    /// 子弹数量更新
    /// </summary>
    public void UpdateCurrentMagzineText() {
        currentMagzineSlider.value = CurWeapon.CurrentMagzine;
    }

    public void SetBulletSpawningTransform(Vector3 tgt)
    {
        Vector3 offset = WeaponSprite.transform.localPosition;
        BulletSpawningPoint.localPosition = offset+tgt;
    }

    public void PickupWeapon()
    {
        CurWeapon = _Weapon_perspective;
        //Debug.Log("捡枪");
        _Weapon_perspective.gameObject.SetActive(false);
        //_Weapon_perspective.SetWeaponSpriteEnable(false);
        _Weapon_perspective.transform.position = this.transform.position;
        _Weapon_perspective.transform.parent = this.transform;
        _Weapon_perspective.readytoDisappear = false;
        //_Weapon_perspective.IsPicked = true;
        _weaponAudio.PlayOneShot(AC_PickWeapon);
        ResetWeapon();
    }

    public void ThrowAwayWeapon()
    {
        //打断 换弹
        _reloading_Aborted = true;
        _weaponAudio.Stop();
        Reloaded = false;
        //Debug.Log("扔枪");
        _Weapon_discard = CurWeapon;
        _Weapon_discard.gameObject.SetActive(true);
        //_Weapon_discard.SetWeaponSpriteEnable(true);
        _Weapon_discard.transform.parent = null;
        _Weapon_discard.readytoDisappear = true;
        //_Weapon_discard.IsPicked = false;
        CurWeapon = DefWeapon;
        ResetWeapon();
    }

    public void ReplaceWeapon()
    {
        ThrowAwayWeapon();
        PickupWeapon();
    }

    private void ResetWeapon()
    {
        WeaponAnim.runtimeAnimatorController = CurWeapon.WeaponAnimController;
        SetBulletSpawningTransform(CurWeapon.BulletSpawningPoint.transform.localPosition);
    }

    public void SeekForPerspectiveWeapon()
    {
        _Weapon_perspective = null;
        Collider2D[] m_cCollider = Physics2D.OverlapCircleAll(transform.position, 1f, WeaponDetect);
        if (m_cCollider == null)
        {
            _Weapon_perspective = null;
            return;
        }//没找到

        float MaxDixt = 100;
        foreach (Collider2D i in m_cCollider)
        {
            if (i == this.GetComponent<Collider2D>())
                continue;
            if (MaxDixt > Vector3.Distance(i.transform.position, transform.position))
            {
                MaxDixt = Vector3.Distance(i.transform.position, transform.position);
                Weapons tempWeapon = i.GetComponent<Weapons>();
                _Weapon_perspective = tempWeapon;
            }
        }
    }

    public void PlayFireAudio()
    {
        _weaponAudio.PlayOneShot(CurWeapon.AC_Fire);
    }

    public void PlayReloadAudio()
    {
        _weaponAudio.PlayOneShot(CurWeapon.AC_Reload);
    }

    public void PlayPickGrenadaAudio()
    {
        _weaponAudio.PlayOneShot(AC_PickGrenada);
    }

}
