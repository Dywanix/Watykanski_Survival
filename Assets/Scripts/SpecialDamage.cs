using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialDamage : MonoBehaviour
{
    public Bullet bullet;
    public float damagePerSpecial;

    void Start()
    {
        bullet.damage *= 1f + damagePerSpecial * bullet.special;
    }
}
