using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulletExplosion : MonoBehaviour
{
    public Bullet ThisBullet, BulletsShards;
    public GameObject[] BulletShard;
    public Rigidbody2D Dir;
    public Transform Form;
    public float[] bulletForce, damageEfficiency, bonusDuration;
    public float delay, delayRange, accuracy;
    public int[] bulletsCount, pierceCount;
    public bool[] retainPierce;

    void Start()
    {
        delay *= Random.Range(1f, delayRange);
    }

    void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0)
        {
            Shatter();
            Destroy(gameObject);
        }
    }

    public void Shatter()
    {
        for (int j = 0; j < BulletShard.Length; j++)
        {
            for (int i = 0; i < bulletsCount[j]; i++)
            {
                Form.rotation = Quaternion.Euler(Form.rotation.x, Form.rotation.y, Dir.rotation + Random.Range(-accuracy, accuracy) + i * 360f / bulletsCount[j]);
                GameObject bullet = Instantiate(BulletShard[j], Form.position, Form.rotation);
                Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
                bullet_body.AddForce(Form.up * bulletForce[j], ForceMode2D.Impulse);
                BulletsShards = bullet.GetComponent(typeof(Bullet)) as Bullet;
                SetBullet(j);
            }
        }
        //Destroy(gameObject);
    }

    void SetBullet(int which)
    {
        BulletsShards.duration = ThisBullet.duration + bonusDuration[which];
        BulletsShards.falloff = ThisBullet.falloff + 0.25f;
        BulletsShards.force = ThisBullet.force;
        BulletsShards.damage = ThisBullet.damage * damageEfficiency[which];
        BulletsShards.DoT = ThisBullet.DoT;
        BulletsShards.shatter = ThisBullet.shatter;
        BulletsShards.burn = ThisBullet.burn;
        BulletsShards.curse = ThisBullet.curse;
        BulletsShards.damageGain = ThisBullet.damageGain;
        BulletsShards.vulnerableApplied = ThisBullet.vulnerableApplied;
        BulletsShards.slowDuration = ThisBullet.slowDuration;
        BulletsShards.stunDuration = ThisBullet.stunDuration;
        if (retainPierce[which])
            BulletsShards.pierce = ThisBullet.pierce + pierceCount[which];
        else BulletsShards.pierce = pierceCount[which];
        BulletsShards.crit = ThisBullet.crit;
    }
}
