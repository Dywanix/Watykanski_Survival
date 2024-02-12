using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public GameObject Projectal;
    public Transform[] Barrels;

    void Start()
    {
        Fire();
    }

    void Fire()
    {
        for (int i = 0; i < Barrels.Length; i++)
        {
            GameObject bullet = Instantiate(Projectal, Barrels[i].position, Barrels[i].rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrels[i].up * 10f, ForceMode2D.Impulse);
        }
    }
}
