using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulletFire : MonoBehaviour
{
    public Bullet ThisBullet, BulletsShards;
    public GameObject BulletFire;
    public Rigidbody2D Dir;
    public Transform Form;
    public float frequency, delay, frequencyRange, spread, accuracy, bulletForce, forceRange, bulletDuration, damageEfficiency;
    public int bulletsCount, pierceCount;
    public bool retainPierce;

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
            GameObject bullet = Instantiate(BulletFire, Form.position, Form.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Form.up * bulletForce * Random.Range(1f, 1f + forceRange), ForceMode2D.Impulse);
            BulletsShards = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet();
        }
        delay += frequency * Random.Range(1f, 1f + frequencyRange);
    }

    void SetBullet()
    {
        BulletsShards.duration = bulletDuration;
        BulletsShards.force = ThisBullet.force;
        BulletsShards.mass = ThisBullet.mass;
        BulletsShards.damage = ThisBullet.damage * damageEfficiency;
        BulletsShards.DoT = ThisBullet.DoT;
        BulletsShards.shatter = ThisBullet.shatter;
        BulletsShards.incendiary = ThisBullet.incendiary;
        BulletsShards.curse = ThisBullet.curse;
        BulletsShards.damageGain = ThisBullet.damageGain;
        BulletsShards.vulnerableApplied = ThisBullet.vulnerableApplied;
        BulletsShards.slowDuration = ThisBullet.slowDuration;
        BulletsShards.stunDuration = ThisBullet.stunDuration;
        if (retainPierce)
            BulletsShards.pierce = ThisBullet.pierce + pierceCount;
        else BulletsShards.pierce = pierceCount;
        BulletsShards.pierceEfficiency = ThisBullet.pierceEfficiency;
        BulletsShards.crit = ThisBullet.crit;
    }
}
