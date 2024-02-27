using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : MonoBehaviour
{
    public Bullet ThisBullet;

    public float baseDoT, procDoT;

    void Start()
    {
        //Invoke("PoisonUp", 0.05f);
        PoisonUp();
    }

    void PoisonUp()
    {
        ThisBullet.DoT += baseDoT + ThisBullet.DoT * procDoT;
    }
}
