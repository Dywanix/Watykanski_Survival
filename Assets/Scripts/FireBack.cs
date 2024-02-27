using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBack : MonoBehaviour
{
    public GameObject Bullet;
    public Rigidbody2D Body;
    public Transform Barrel;
    public float delay, delayMultiplyer, accuracy, force;

    void Start()
    {
        delay *= Random.Range(1f, 1f + delayMultiplyer);
    }

    void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0f)
            Fire();
    }

    void Fire()
    {
        Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Body.rotation + 180f + Random.Range(-accuracy, accuracy));
        GameObject bullet = Instantiate(Bullet, Barrel.position, Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(Barrel.up * force * Random.Range(0.975f, 1.025f), ForceMode2D.Impulse);

        Destroy(gameObject);
    }
}
