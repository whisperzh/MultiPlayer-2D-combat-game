using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : Weapons
{
    public override void WeaponGenerateBullet(Quaternion BulletRotation, Player_Controller player, Transform SPpoint) {
        for (int i = 0; i < 6; i++)
        {
            M_Bullet temp = Instantiate( bulletPrefab, SPpoint.position, BulletRotation);

            Vector2 flyingDir = SPpoint.right;
            Vector2 v = new Vector3(flyingDir.y, -flyingDir.x);
            v.Normalize();
            float interval = 2 *  Shooting_bias / 5 / 100;
            flyingDir += v *  Shooting_bias / 100;
            flyingDir -= v * interval * i;

            //设置子弹属性(所属 伤害 速度 方向)
            temp.SetBulletAttribution(player, singleBulletDamage, singleBulletSpeed, flyingDir);
        }
    }
}
