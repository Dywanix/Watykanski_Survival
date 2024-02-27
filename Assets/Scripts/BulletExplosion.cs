using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExplosion : MonoBehaviour
{
    public GameObject Bullet;
    public Rigidbody2D Dir;
    public Transform form;
    public float delay, delayMultiplyer, accuracy, bulletForce;
    public int bulletsCount;

    void Start()
    {
        delay = delay * Random.Range(1f, 1f + delayMultiplyer);
    }

    void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0)
            Explode();
    }

    void Explode()
    {
        for (int i = 0; i < bulletsCount; i++)
        {
            form.rotation = Quaternion.Euler(form.rotation.x, form.rotation.y, Dir.rotation + Random.Range(-accuracy, accuracy) + i * (360f / bulletsCount));
            GameObject bullet = Instantiate(Bullet, form.position, form.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(form.up * bulletForce, ForceMode2D.Impulse);
        }
        Destroy(gameObject);
    }
}
