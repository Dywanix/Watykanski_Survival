using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plasma : MonoBehaviour
{
    public Bullet ThisBullet, BulletsShards;
    public GameObject BulletShard;
    public Rigidbody2D Dir;
    public Transform Form;
    public float damage, delay, damageEfficiency;

    public bool explosive;

    void Start()
    {
        ThisBullet.damage *= damage;
    }

    void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0)
            Shatter();
    }

    void Shatter()
    {
        if (explosive)
        {
            ThisBullet.Explosions();

            for (int i = 0; i < 6; i++)
            {
                Form.rotation = Quaternion.Euler(Form.rotation.x, Form.rotation.y, Dir.rotation + (-112.5f + 45f * i));
                GameObject bullet = Instantiate(BulletShard, Form.position, Form.rotation);
                Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
                bullet_body.AddForce(Form.up * (3 + ThisBullet.force * 0.6f) * Random.Range(0.95f, 1.06f), ForceMode2D.Impulse);
                BulletsShards = bullet.GetComponent(typeof(Bullet)) as Bullet;
                SetBullet();
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                Form.rotation = Quaternion.Euler(Form.rotation.x, Form.rotation.y, Dir.rotation + (-90f + 60f * i));
                GameObject bullet = Instantiate(BulletShard, Form.position, Form.rotation);
                Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
                bullet_body.AddForce(Form.up * (3 + ThisBullet.force * 0.6f) * Random.Range(0.95f, 1.06f), ForceMode2D.Impulse);
                BulletsShards = bullet.GetComponent(typeof(Bullet)) as Bullet;
                SetBullet();
            }
        }
        Destroy(gameObject);
    }

    void SetBullet()
    {
        BulletsShards.duration = ThisBullet.duration + 0.25f;
        BulletsShards.falloff = ThisBullet.falloff + 0.25f;
        BulletsShards.force = ThisBullet.force;
        BulletsShards.mass = ThisBullet.mass;
        BulletsShards.damage = ThisBullet.damage * damageEfficiency;
        BulletsShards.DoT = ThisBullet.DoT;
        BulletsShards.shatter = ThisBullet.shatter;
        BulletsShards.incendiary = ThisBullet.incendiary;
        BulletsShards.curse = ThisBullet.curse;
        BulletsShards.damageGain = ThisBullet.damageGain;
        BulletsShards.penetration = ThisBullet.penetration;
        BulletsShards.armorShred = ThisBullet.armorShred * damageEfficiency;
        BulletsShards.vulnerableApplied = ThisBullet.vulnerableApplied * damageEfficiency;
        BulletsShards.slowDuration = ThisBullet.slowDuration * damageEfficiency;
        BulletsShards.stunDuration = ThisBullet.stunDuration;
        BulletsShards.pierce = ThisBullet.pierce;
        BulletsShards.pierceEfficiency = ThisBullet.pierceEfficiency;
        BulletsShards.crit = ThisBullet.crit;
    }
}
