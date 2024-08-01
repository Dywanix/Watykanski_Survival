using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleBullets : MonoBehaviour
{
    public Bullet ThisBullet, BulletsShards;
    public GameObject BulletShard;
    public Transform[] Barrels;

    public float bulletForce, damageEfficiency;

    void Start()
    {
        for (int i = 0; i < Barrels.Length; i++)
        {
            GameObject bullet = Instantiate(BulletShard, Barrels[i].position, Barrels[i].rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrels[i].up * bulletForce, ForceMode2D.Impulse);
            BulletsShards = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet();
        }

        Destroy(gameObject);
    }
    
    void SetBullet()
    {
        BulletsShards.duration = ThisBullet.duration;
        BulletsShards.force = ThisBullet.force;
        BulletsShards.mass = ThisBullet.mass;
        BulletsShards.damage = ThisBullet.damage * damageEfficiency;
        BulletsShards.DoT = ThisBullet.DoT;
        BulletsShards.shatter = ThisBullet.shatter;
        BulletsShards.burn = ThisBullet.burn;
        BulletsShards.curse = ThisBullet.curse;
        BulletsShards.damageGain = ThisBullet.damageGain;
        BulletsShards.vulnerableApplied = ThisBullet.vulnerableApplied * damageEfficiency;
        BulletsShards.slowDuration = ThisBullet.slowDuration * damageEfficiency;
        BulletsShards.stunDuration = ThisBullet.stunDuration;
        BulletsShards.pierce = ThisBullet.pierce;
        BulletsShards.pierceEfficiency = ThisBullet.pierceEfficiency;
        BulletsShards.crit = ThisBullet.crit;
        BulletsShards.special = ThisBullet.special;
    }
}
