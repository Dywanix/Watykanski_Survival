using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public PlayerController playerStats;
    public Transform Barrel;
    public GameObject Projectal;
    public Bullet firedBullet;
    public float damage, damageLvl, fireRate;
    float fireTimer;

    void Update()
    {
        if (fireTimer > 0f)
            fireTimer -= Time.deltaTime;
        else Fire();
    }

    void Fire()
    {
        fireTimer += fireRate / playerStats.SpeedMultiplyer(1f);

        GameObject bullet = Instantiate(Projectal, Barrel.position, Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(Barrel.up * Random.Range(17.2f, 18.8f), ForceMode2D.Impulse);
        firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        firedBullet.damage = Damage() *playerStats.DamageDealtMultiplyer(1f);
    }

    float Damage()
    {
        return damage + damageLvl * playerStats.level;
    }
}
