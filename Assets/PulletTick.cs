using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulletTick : MonoBehaviour
{
    public Bullet ThisBullet, BulletsShards;
    public GameObject BulletTick;
    public Rigidbody2D Dir;
    public Transform Form;
    public float frequency, delay, frequencyRange, damageEfficiency;
    public int bulletsCount;

    void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0)
            Tick();
    }

    void Tick()
    {
        for (int i = 0; i < bulletsCount; i++)
        {
            Form.rotation = Quaternion.Euler(Form.rotation.x, Form.rotation.y, Dir.rotation + Random.Range(0f, 360f));
            GameObject bullet = Instantiate(BulletTick, Form.position, Form.rotation);
            BulletsShards = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet();
        }
        delay += frequency * Random.Range(1f, 1f + frequencyRange);
    }

    void SetBullet()
    {
        BulletsShards.damage = ThisBullet.damage * damageEfficiency;
        BulletsShards.DoT = ThisBullet.DoT;
        BulletsShards.shatter = ThisBullet.shatter;
        BulletsShards.incendiary = ThisBullet.incendiary;
        BulletsShards.curse = ThisBullet.curse;
        BulletsShards.damageGain = ThisBullet.damageGain;
        BulletsShards.penetration = ThisBullet.penetration;
        BulletsShards.armorShred = ThisBullet.armorShred;
        BulletsShards.vulnerableApplied = ThisBullet.vulnerableApplied;
        BulletsShards.slowDuration = ThisBullet.slowDuration;
        BulletsShards.stunDuration = ThisBullet.stunDuration;
        BulletsShards.pierceEfficiency = ThisBullet.pierceEfficiency;
        BulletsShards.crit = ThisBullet.crit;
    }
}
