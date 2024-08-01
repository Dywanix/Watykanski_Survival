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
    public float bonusShatter;
    public bool straight, scaleWithAreaSize;

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
            if (!straight) //Form.rotation = Quaternion.Euler(Form.rotation.x, Form.rotation.y, Dir.rotation);
                Form.rotation = Quaternion.Euler(Form.rotation.x, Form.rotation.y, Dir.rotation + Random.Range(0f, 360f));
            GameObject bullet = Instantiate(BulletTick, Form.position, Form.rotation);
            BulletsShards = bullet.GetComponent(typeof(Bullet)) as Bullet;
            if (scaleWithAreaSize)
                bullet.transform.localScale = new Vector3(ThisBullet.areaSize, ThisBullet.areaSize, 1f);
            SetBullet();
        }
        delay += frequency * Random.Range(1f, 1f + frequencyRange);
    }

    void SetBullet()
    {
        BulletsShards.damage = ThisBullet.damage * damageEfficiency;
        BulletsShards.DoT = ThisBullet.DoT;
        BulletsShards.shatter = ThisBullet.shatter + bonusShatter;
        BulletsShards.burn = ThisBullet.burn;
        BulletsShards.curse = ThisBullet.curse;
        BulletsShards.damageGain = ThisBullet.damageGain;
        BulletsShards.vulnerableApplied = ThisBullet.vulnerableApplied;
        BulletsShards.slowDuration = ThisBullet.slowDuration;
        BulletsShards.stunDuration = ThisBullet.stunDuration;
        BulletsShards.pierceEfficiency = ThisBullet.pierceEfficiency;
        BulletsShards.crit = ThisBullet.crit;
    }
}
