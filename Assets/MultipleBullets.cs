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
        BulletsShards.damage = ThisBullet.damage * damageEfficiency;
        BulletsShards.DoT = ThisBullet.DoT;
        BulletsShards.penetration = ThisBullet.penetration;
        BulletsShards.armorShred = ThisBullet.armorShred * damageEfficiency;
        BulletsShards.vulnerableApplied = ThisBullet.vulnerableApplied * damageEfficiency;
        BulletsShards.slowDuration = ThisBullet.slowDuration * damageEfficiency;
        BulletsShards.stunChance = ThisBullet.stunChance * damageEfficiency;
        BulletsShards.stunDuration = ThisBullet.stunDuration;
        BulletsShards.pierce = ThisBullet.pierce;
        BulletsShards.pierceEfficiency = ThisBullet.pierceEfficiency;
        BulletsShards.crit = ThisBullet.crit;
    }
}
