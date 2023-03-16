using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;
    // -- Gun Stats
    public float damage, penetration, fireRate, reloadTime, accuracy, force, cameraShake, shakeDuration; //fireRate oznacza czas miêdzy strza³ami w s, a reaload iloœæ s,
    public int magazineSize, bulletsLeft, ammo, bulletSpread, upgrades, upgradeCost, upgradeCostIncrease, upgradeCostIncreaseIncrease, roll;
    public bool infiniteMagazine, infiniteAmmo, individualReload;

    public GameObject bulletPrefab;

    void Start()
    {
        bulletsLeft = magazineSize;
    }

    public void Upgrade()
    {
        upgrades++;
        upgradeCost += upgradeCostIncrease;

        for (int i = 0; i < 3; i++)
        {
            roll = Random.Range(0,3);
            switch (roll)
            {
                case 0:
                    damage *= 1.01f;
                    cameraShake *= 1.004f;
                    break;
                case 1:
                    fireRate *= 0.986f;
                    reloadTime *= 0.996f;
                    break;
                case 2:
                    accuracy *= 0.98f;
                    force *= 1.02f;
                    break;
            }
        }

        if (upgrades % 5 == 0)
        {
            upgradeCostIncrease += upgradeCostIncreaseIncrease;
        }
    }
}
