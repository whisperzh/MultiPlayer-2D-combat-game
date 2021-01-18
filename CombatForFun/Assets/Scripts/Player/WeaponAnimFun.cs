using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimFun : MonoBehaviour
{
    
    [Header("手雷 音效")]
    private Weapon_Controller weapon_Controller;
    // Start is called before the first frame update
    void Start()
    {
        weapon_Controller = GetComponentInParent<Weapon_Controller>();
    }

    public void AwakeGrenadaAudio()
    {
        weapon_Controller.PlayAwakeGrenadaAuido();
    }

    public void ThrowGrenadaAnim()
    {
        weapon_Controller.ThrowGrenada();
    }

    public void FireAnim()
    {
        weapon_Controller.InitFireBullet();
    }
}
