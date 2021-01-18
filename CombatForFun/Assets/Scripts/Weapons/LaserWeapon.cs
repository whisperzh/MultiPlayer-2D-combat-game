using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LaserWeapon : Weapons
{
    public LineRenderer LaserLine;

    public GameObject LaserObject;

    public Shader LaserShader;

    public LayerMask laserDetect;

    GameObject temp;

    public LayerMask damageDetect;

    public override void WeaponGenerateBullet(Quaternion BulletRotation, Player_Controller player, Transform SPpoint)
    { 
        Vector3 laserDir = SPpoint.right;
        RaycastHit2D hitInfo = Physics2D.Raycast(SPpoint.position, laserDir,5000, laserDetect);
        RaycastHit2D damageinfo = Physics2D.Raycast(SPpoint.position, laserDir, 5000, damageDetect);
        temp = Instantiate(LaserObject,transform.position,Quaternion.identity);

        LaserLine = temp.GetComponent<LineRenderer>();
        LaserLine.enabled = true;
        LaserLine.material = new Material(LaserShader);
        LaserLine.SetPosition(0, SPpoint.position);
        LaserLine.SetPosition(1, hitInfo.point);
        Debug.DrawLine(SPpoint.position, hitInfo.point, Color.red);
        if(damageinfo.collider!=null)
            if (damageinfo.collider.GetComponent<Player_Controller>() != null)
            {
                Debug.Log("Hit");
                damageinfo.collider.GetComponent<Player_Controller>().Hurt(singleBulletDamage);
            }
        
 
        Invoke("SeizeFire",0.5f);

        //M_Bullet temp = Instantiate(bulletPrefab, SPpoint.position, BulletRotation);
        //Vector2 flyingDir = SPpoint.right;
        //Vector2 v = Vector3.Normalize(new Vector3(flyingDir.y, -flyingDir.x));
        //v.Normalize();
        //if (Shooting_bias != 1)
        //{
        //    float r = UnityEngine.Random.Range(-Shooting_bias, Shooting_bias);
        //    v = v / 100 * r;    
        //    flyingDir += v;
        //}
        ////设置子弹属性(所属 伤害 速度 方向)
        //temp.SetBulletAttribution(player, singleBulletDamage, singleBulletSpeed, flyingDir);
    }

    public override void SeizeFire()
    {
        LaserLine = null;
        Destroy(temp);
    }

    public override void InitiateLaser(LineRenderer l)
    {
       LaserLine = l;
    }

}
