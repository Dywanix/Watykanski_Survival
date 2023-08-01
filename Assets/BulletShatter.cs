using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShatter : MonoBehaviour
{
    public Bullet ThisBullet, BulletsShards;
    public GameObject BulletShard;
    public Rigidbody2D Dir;
    public Transform Form;
    public float delay, delayRange, spread, accuracy, bulletForce, bonusDuration, damageEfficiency;
    public int bulletsCount, pierceCount;

    void Start()
    {
        delay *= Random.Range(1f, delayRange);
    }

    void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0)
            Shatter();
    }

    void Shatter()
    {
        for (int i = 0; i < bulletsCount; i++)
        {
            Form.rotation = Quaternion.Euler(Form.rotation.x, Form.rotation.y, Dir.rotation + Random.Range(-accuracy, accuracy) + (i * 2 - bulletsCount + 1) * spread / 2);
            GameObject bullet = Instantiate(BulletShard, Form.position, Form.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Form.up * bulletForce, ForceMode2D.Impulse);
            BulletsShards = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet();
        }
        Destroy(gameObject);
    }

    void SetBullet()
    {
        BulletsShards.duration = ThisBullet.duration + bonusDuration;
        BulletsShards.damage = ThisBullet.damage * damageEfficiency;
        BulletsShards.DoT = ThisBullet.DoT;
        BulletsShards.penetration = ThisBullet.penetration;
        BulletsShards.armorShred = ThisBullet.armorShred;
        BulletsShards.vulnerableApplied = ThisBullet.vulnerableApplied;
        BulletsShards.slowDuration = ThisBullet.slowDuration;
        BulletsShards.stunChance = ThisBullet.stunChance;
        BulletsShards.stunDuration = ThisBullet.stunDuration;
        BulletsShards.pierce = pierceCount;
        BulletsShards.pierceEfficiency = ThisBullet.pierceEfficiency;
        BulletsShards.crit = ThisBullet.crit;
    }
}
