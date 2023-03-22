using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;
    // -- Basic Gun Stats
    public float damage, critChance, penetration, fireRate, reloadTime, accuracy, force, cameraShake, shakeDuration; //fireRate oznacza czas miêdzy strza³ami w s, a reaload iloœæ s,

    // -- Special Gun Stats
    public float critDamage, armorShred, vulnerableApplied, pierceDamage, DoT;
    public int magazineSize, overload, bulletsLeft, ammo, ammoFromPack, bulletSpread, pierce, upgrades, upgradeCost, upgradeCostIncrease, upgradeCostIncreaseIncrease, specialUpgrades, roll;
    public int[] Slots;
    public bool infiniteMagazine, infiniteAmmo, individualReload;
    public float temp;
    private int tempi;

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
            roll = Random.Range(0, 9);
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
                case 4:
                    if (critChance < 1f)
                        critChance += 0.01f;
                    else
                        critDamage += 0.01f;
                    break;
                case 5:
                    DoT += 0.01f + 0.05f * DoT;
                    break;
                case 6:
                    roll = Random.Range(0, 4);
                    Slots[roll]++;
                    break;
                case 7:
                    damage *= 1.01f;
                    cameraShake *= 1.004f;
                    break;
                case 8:
                    fireRate *= 0.986f;
                    reloadTime *= 0.996f;
                    break;
            }
        }

        if (upgrades % upgradeCostIncreaseIncrease == 0)
            upgradeCostIncrease++;

        if (upgrades % 5 == 0)
            specialUpgrades++;
    }

    public void SpecialUpgrade(int which)
    {
        switch (which)
        {
            case 0:
                damage *= 1.02f;
                cameraShake *= 1.008f;
                fireRate *= 0.97f;
                reloadTime *= 0.99f;
                break;
            case 1:
                for (int i = 0; i < 2; i++)
                {
                    if (critChance < 1f)
                        critChance += 0.01f;
                    else
                        critDamage += 0.01f;
                }
                critDamage += 0.02f * (1 + critDamage);
                break;
            case 2:
                temp = 0.25f + 0.15f * magazineSize;
                if (temp < 1f)
                {
                    magazineSize++;
                    reloadTime *= 2f - temp;
                }
                else
                {
                    tempi = 0;
                    for (int i = 0; i < temp; i++)
                    {
                        magazineSize++;
                        tempi++;
                    }
                    temp = temp - tempi;
                    reloadTime *= 1f - (0.8f * temp / tempi);
                }
                break;
            case 3:
                armorShred += 0.03f * fireRate * (0.1f + 0.9f * bulletSpread);
                fireRate *= 0.98f;
                break;
            case 4:
                vulnerableApplied += 0.02f * fireRate * (0.1f + 0.9f * bulletSpread);
                accuracy *= 0.96f;
                break;
            case 5:
                tempi = 2 + ammoFromPack / 5;
                ammoFromPack += tempi;
                ammo += 2 * ammoFromPack;
                break;
            case 6:
                temp = 1.01f + 0.08f / bulletSpread;
                accuracy *= temp; cameraShake *= temp; reloadTime *= temp;

                temp = 0.08f + (1f * bulletSpread / (1f * bulletSpread + 1f));
                damage *= temp; armorShred *= temp; vulnerableApplied *= temp;

                bulletSpread++;
                break;
            case 7:
                damage *= 0.96f;
                DoT += 0.07f + 0.2f * DoT;
                break;
            case 8:
                temp = (0.1f + reloadTime) / (0.2f + 1.2f * fireRate);
                if (temp < 1f)
                {
                    overload++;
                    reloadTime *= 1.5f - (0.5f * temp);
                }
                else
                {
                    tempi = 0;
                    for (int i = 0; i < temp; i++)
                    {
                        overload++;
                        tempi++;
                    }
                    temp = temp - tempi;
                    reloadTime *= 1f - (1.1f * temp / tempi);
                }
                break;
            case 9:
                Slots[0]++;
                damage *= 1.03f;
                break;
            case 10:
                Slots[1]++;
                accuracy *= 0.93f;
                break;
            case 11:
                Slots[2]++;
                reloadTime *= 0.94f;
                break;
            case 12:
                Slots[3]++;
                fireRate *= 0.96f;
                break;
        }
    }

    public void AmmoPicked()
    {
        ammo += ammoFromPack;
    }
}
