using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulletExplosion : MonoBehaviour
{
    public Bullet ThisBullet, BulletsShards;
    public GameObject[] BulletShard;
    public Rigidbody2D Dir;
    public Transform Form;
    public float[] bulletForce, damageEfficiency;
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
        BulletsShards.damage = ThisBullet.damage * damageEfficiency[which];
        BulletsShards.DoT = ThisBullet.DoT;
        BulletsShards.penetration = ThisBullet.penetration;
        BulletsShards.armorShred = ThisBullet.armorShred;
        BulletsShards.vulnerableApplied = ThisBullet.vulnerableApplied;
        BulletsShards.slowDuration = ThisBullet.slowDuration;
        BulletsShards.stunChance = ThisBullet.stunChance;
        BulletsShards.stunDuration = ThisBullet.stunDuration;
        if (retainPierce[which])
            BulletsShards.pierce = ThisBullet.pierce + pierceCount[which];
        else BulletsShards.pierce = pierceCount[which];
        BulletsShards.crit = ThisBullet.crit;
    }
}
