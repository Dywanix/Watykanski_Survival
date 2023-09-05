using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialDamage : MonoBehaviour
{
    public Bullet bullet;

    void Start()
    {
        bullet.damage *= 1f + 0.34f * bullet.special;
    }
}
