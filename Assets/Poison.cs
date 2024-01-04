using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : MonoBehaviour
{
    public Bullet ThisBullet;

    void Start()
    {
        //Invoke("PoisonUp", 0.05f);
        PoisonUp();
    }

    void PoisonUp()
    {
        ThisBullet.DoT += 1.2f + ThisBullet.DoT * 0.2f;
    }
}
