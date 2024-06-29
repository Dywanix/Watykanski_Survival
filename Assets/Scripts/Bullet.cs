using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject ExplosionRadius;
    private Bullet Explosion;
    public BulletPlus BonusEffects;
    public PulletExplosion ShardExplosion;
    public float duration, falloff, force, mass, damage, penetration, armorShred, vulnerableApplied, slowDuration, stunDuration, pierceEfficiency, DoT, shatter, curse, incendiary, damageGain;
    public int pierce, special;
    public bool crit, AoE, damageLess;
    bool fallen;

    [Header("AreaBullets")]
    public Transform TargetedLocation;
    public GameObject HitArea;
    public FallingObject fall;
    float travelX, travelY;

    void Start()
    {
        if (TargetedLocation)
        {
            Instantiate(HitArea, TargetedLocation.position, TargetedLocation.rotation);
            fall.duration = duration;
            duration += 0.01f;
            travelX = (TargetedLocation.position.x - transform.position.x) / duration;
            travelY = (TargetedLocation.position.y - transform.position.y) / duration;
        }
        Invoke("Fall", falloff);
        Invoke("End", duration);
    }

    void Update()
    {
        //duration -= Time.deltaTime;
        damage *= 1f + damageGain * Time.deltaTime;
        /*if (duration <= 0)
        {
            Explosions();
            Destroy(gameObject);
        }*/
        if (fallen)
            damage /= 1f + 1.2f * Time.deltaTime;

        if (TargetedLocation)
        {
            transform.position = new Vector3(transform.position.x + travelX * Time.deltaTime, transform.position.y + travelY * Time.deltaTime, 0);
        }
    }

    void Fall()
    {
        fallen = true;
        if (TargetedLocation)
        {
            Explosions();
            Destroy(gameObject);
        }
    }

    void End()
    {
        Explosions();
        Destroy(gameObject);
    }

    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Enemy")
        {
            pierce--;
            damage *= pierceDamage;
            if (pierce <= 0)
                Destroy(gameObject);
        }
    }*/

    public void Struck()
    {
        Explosions();

        pierce--;
        damage *= pierceEfficiency;
        armorShred *= 0.2f + 0.8f * pierceEfficiency;
        vulnerableApplied *= 0.2f + 0.8f * pierceEfficiency;
        slowDuration *= 0.4f + 0.6f * pierceEfficiency;
        stunDuration *= 0.4f + 0.6f * pierceEfficiency;

        if (BonusEffects)
            BonusEffects.Struck();

        if (pierce <= 0)
            Destroy(gameObject);
    }

    public void Explosions()
    {
        if (AoE)
        {
            GameObject bullet = Instantiate(ExplosionRadius, transform.position, transform.rotation);
            Explosion = bullet.GetComponent(typeof(Bullet)) as Bullet;
            Explosion.damage = damage; Explosion.DoT = DoT; Explosion.shatter = shatter; Explosion.incendiary = incendiary; Explosion.curse = curse;
            Explosion.penetration = penetration;
            Explosion.armorShred = armorShred;
            Explosion.vulnerableApplied = vulnerableApplied;
            Explosion.slowDuration = slowDuration;
            Explosion.stunDuration = stunDuration;
            Explosion.crit = crit;
        }
        if (ShardExplosion)
            ShardExplosion.Shatter();
    }
}
