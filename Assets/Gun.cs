using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;
    // -- Basic Gun Stats
    public float damage, critChance, penetration, fireRate, reloadTime, accuracy, force, cameraShake, shakeDuration; //fireRate oznacza czas miêdzy strza³ami w s, a reaload iloœæ s,

    // -- Special Gun Stats
    public float critDamage;
    public int magazineSize, bulletsLeft, ammo, ammoFromPack, bulletSpread, upgrades, upgradeCost, upgradeCostIncrease, upgradeCostIncreaseIncrease, roll;
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
            roll = Random.Range(0,4);
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
                case 3:
                    if (penetration < 1f)
                        penetration += 0.01f;
                    else
                    {
                        damage *= 1.01f;
                        cameraShake *= 1.004f;
                    }
                    break;
            }
        }

        if (upgrades % 5 == 0)
        {
            upgradeCostIncrease += upgradeCostIncreaseIncrease;
        }
    }

    public void AmmoPicked()
    {
        ammo += ammoFromPack;
    }
}
