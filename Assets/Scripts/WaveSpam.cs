using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpam : MonoBehaviour
{
    public GameObject Projectal;
    public Transform[] Barrels;

    public float fireRate;

    void Start()
    {
        Invoke("Fire", 6f);
    }

    void Fire()
    {
        for (int i = 0; i < Barrels.Length; i++)
        {
            GameObject bullet = Instantiate(Projectal, Barrels[i].position, Barrels[i].rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrels[i].up * 11f, ForceMode2D.Impulse);
        }

        Invoke("Fire", fireRate);
    }
}
