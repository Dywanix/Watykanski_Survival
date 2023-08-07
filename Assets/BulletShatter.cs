using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShatter : MonoBehaviour
{
    public Bullet ThisBullet, BulletsShards;
    public GameObject BulletShard;
    public Rigidbody2D Dir;
    public Transform Form;
    public float delay, delayRange, spread, accuracy, bulletForce, forceRange, bonusDuration, damageEfficiency;
    public int bulletsCount, pierceCount;
    public bool remain, retainPierce, countBasedOnGun;

    void Start()
    {
        delay *= Random.Range(1f, delayRange);
        if (countBasedOnGun)
            bulletsCount = ThisBullet.special;
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
            bullet_body.AddForce(Form.up * bulletForce * Random.Range(1f, 1f + forceRange), ForceMode2D.Impulse);
            BulletsShards = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet();
        }
        if (!remain)
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
        if (retainPierce)
            BulletsShards.pierce = ThisBullet.pierce + pierceCount;
        else BulletsShards.pierce = pierceCount;
        BulletsShards.pierceEfficiency = ThisBullet.pierceEfficiency;
        BulletsShards.crit = ThisBullet.crit;
    }
}
