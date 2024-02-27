using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electric : MonoBehaviour
{
    public Bullet ThisBullet;

    public float baseShatter, procShatter, pierceEff;

    void Start()
    {
        //Invoke("PoisonUp", 0.05f);
        Electicize();
    }

    void Electicize()
    {
        ThisBullet.shatter += baseShatter + ThisBullet.DoT * procShatter;
        ThisBullet.pierce++;
        ThisBullet.pierceEfficiency += pierceEff;
    }
}
