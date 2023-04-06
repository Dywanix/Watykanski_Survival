using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject Bullet;
    public Transform Barrel;
    public Rigidbody2D Body;

    public float duration, fireRate, accuracy;
    float task = 3f;

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
            Destroy(gameObject);

        task -= Time.deltaTime;
        if (task <= 0)
            Fire();
    }

    void Fire()
    {
        task += fireRate * Random.Range(0.9f, 1.1f);

        Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Body.rotation + Random.Range(-accuracy, accuracy));
        GameObject bullet = Instantiate(Bullet, Barrel.position, Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(Barrel.up * 20f * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
    }
}
