using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPlus : MonoBehaviour
{
    public Bullet ThisBullet;
    private Bullet Explosion;
    public bool poisonmaker, electricrang;
    public GameObject ElectricDischarge;

    float electricCharge;
    int electricShocks;

    void Start()
    {
        if (poisonmaker)
        {
            ThisBullet.DoT += 0.8f;
            ThisBullet.pierce++;
            ThisBullet.pierceEfficiency += 0.04f;
        }
        if (electricrang)
        {
            ThisBullet.shatter += 0.7f;
            ThisBullet.pierce += 2;
        }
    }

    void Update()
    {
        electricCharge += Time.deltaTime;
    }

    public void Struck()
    {
        if (poisonmaker)
            ThisBullet.DoT += 0.12f + ThisBullet.DoT * 0.06f;

        if (electricrang)
        {
            electricCharge += 1f;
            if (electricCharge > 1.5f + 0.5f * electricShocks)
                Discharge();
        }
    }

    void Discharge()
    {
        electricCharge -= 1.5f + 0.5f * electricShocks;
        electricShocks++;

        GameObject bullet = Instantiate(ElectricDischarge, transform.position, transform.rotation);
        Explosion = bullet.GetComponent(typeof(Bullet)) as Bullet;
        Explosion.damage = ThisBullet.damage * 0.5f; Explosion.DoT = ThisBullet.DoT; Explosion.shatter = ThisBullet.shatter;
        Explosion.crit = ThisBullet.crit;
    }
}
