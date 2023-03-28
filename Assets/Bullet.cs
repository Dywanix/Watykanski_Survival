using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject ExplosionRadius;
    private Bullet Explosion;
    public float duration, damage, penetration, armorShred, vulnerableApplied, slowDuration, stunChance, stunDuration, pierceDamage, DoT, incendiary, crateBonus;
    public int pierce;
    public bool crit, AoE;

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
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
        if (AoE)
        {
            GameObject bullet = Instantiate(ExplosionRadius, transform.position, transform.rotation);
            Explosion = bullet.GetComponent(typeof(Bullet)) as Bullet;
            Explosion.damage = damage; Explosion.DoT = DoT; Explosion.penetration = penetration;
            Explosion.armorShred = armorShred; Explosion.vulnerableApplied = vulnerableApplied;
            Explosion.stunChance = stunChance; Explosion.stunDuration = stunDuration;
            Explosion.incendiary = incendiary;
        }
        pierce--;
        damage *= pierceDamage;
        if (pierce <= 0)
            Destroy(gameObject);
    }
}
