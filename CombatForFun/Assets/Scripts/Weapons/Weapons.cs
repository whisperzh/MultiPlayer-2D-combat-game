using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Weapons : MonoBehaviour
{

    [Header("组件属性")]
    #region 组件属性
    public RuntimeAnimatorController WeaponAnimController;
    public M_Bullet bulletPrefab;
    public Transform BulletSpawningPoint;
    private SpriteRenderer _weaponSprite;
    #endregion

    #region 枪械属性
    [Header("类型选择")]
    public bool Semi_Auto;
    public bool IsLaserWeapon;
    public bool IsLimitedRounds;

    [Header("枪械属性")]
    public string Weapon_Name;
    public float FireRate;
    //[Tooltip("开火延迟 一般用于激光枪上 普通武器为0")]
    //public float FireDelay;
    public float ReloadingInterval;

    [Tooltip("当前武器 弹夹 最大容量")]
    public int MagzineCapacity;
    [Tooltip("当前武器 备弹 (不包括 当前弹夹)")]
    public int TotalRounds;
    [Tooltip("当前弹夹 剩余子弹")]
    public int CurrentMagzine;

    public float MaxPercent = 0;

    private int MaxMagzine;

    [Range(1, 100)]
    public float Shooting_bias;

    [Header("子弹属性")]
    public int singleBulletDamage;
    public float singleBulletSpeed;

    [Header("音效")]
    public AudioClip AC_Fire;
    public AudioClip AC_Reload;

    [Header("Shader 部分")]
    public Shader GunShader;
    private Material GunMat;

    [Header("通用")]
    public Vector2 ViberationIntensity;
    public float ViberationDuration;
    public float CameraSakeIntensity;
    public float SecondsToDestroy;
    [HideInInspector] public bool readytoDisappear;
    public bool disappearing;
    [HideInInspector] public bool IsPicked;

    private float _disappearTimer;
    #endregion

    public virtual void WeaponGenerateBullet(Quaternion BulletRotation, Player_Controller player,Transform SPpoint) { }
    public virtual void WeaponGenerateBullet(LineRenderer line,Transform SPpoint) { }
    public virtual void InitiateLaser(LineRenderer l) { }
    public virtual void SeizeFire() { }

    public void Start()
    {
        MaxMagzine = TotalRounds + CurrentMagzine;

        IsPicked = false;
        readytoDisappear = false;
        disappearing = false;

        GunMat = new Material(GunShader);
        GunMat.SetFloat("_DisolveIntensity", 0);
        GunMat.SetFloat("_AmmoColorChange", 0);
        _weaponSprite = GetComponent<SpriteRenderer>();
        _weaponSprite.material = GunMat;

    }
 
    public void Update()
    {
        OutlineChange();

        if(disappearing)
            Disappear();
        else
            WeaponDisappearCheck();
    }

    private void WeaponDisappearCheck()
    {
        if (readytoDisappear && Weapon_Name != "默认武器")
        {
            _disappearTimer -= Time.deltaTime;
            if (_disappearTimer < 0 || CurrentMagzine <= 0 && TotalRounds <= 0)
            {
                disappearing = true;
                IsPicked = true;
                _disappearTimer = 0;
            }

        }
        else
        {
            _disappearTimer = SecondsToDestroy;
        }
    }

    public void Disappear()
    {
        _disappearTimer += Time.deltaTime * 0.5f;
        if (GunMat.GetFloat("_DisolveIntensity") >= 0.99f)
            Destroy(this.gameObject, 0);
        GunMat.SetFloat("_DisolveIntensity", _disappearTimer);
    }

    public void OutlineChange()
    {
        MaxPercent = 1 - ((float)(CurrentMagzine + TotalRounds)/ (float)MaxMagzine);
        GunMat.SetFloat("_AmmoColorChange", MaxPercent);
    }

    ////武器捡起 间隔
    //private void PickCDCountDown()
    //{
    //    if(!CanPick)
    //    {
    //        if(_pickCDTime > 0)
    //        {
    //            _pickCDTime -= Time.deltaTime;
    //            if(_pickCDTime <= 0)
    //            {
    //                CanPick = true;
    //                _pickCDTime = 0.1f;
    //            }
    //        }
    //    }
    //}

    public void SetWeaponSpriteEnable(bool IsActice)
    {
        _weaponSprite.enabled = IsActice;
    }
}
