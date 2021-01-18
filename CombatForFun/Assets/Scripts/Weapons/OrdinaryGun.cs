using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdinaryGun : Weapons
{
    public override void WeaponGenerateBullet(Quaternion BulletRotation,Player_Controller player,Transform SPpoint)
    {
        M_Bullet temp = Instantiate(bulletPrefab, SPpoint.position, BulletRotation);

        Vector2 flyingDir = SPpoint.right;
        Vector2 v = Vector3.Normalize(new Vector3(flyingDir.y, -flyingDir.x));
        v.Normalize();
        if ( Shooting_bias != 1)
        {
            float r = UnityEngine.Random.Range(- Shooting_bias,  Shooting_bias);
            v = v / 100 * r;
            flyingDir += v;
        }

        //设置子弹属性(所属 伤害 速度 方向)
        temp.SetBulletAttribution(player, singleBulletDamage, singleBulletSpeed, flyingDir);
    }
}
