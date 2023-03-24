using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;
    // -- Basic Gun Stats
    public float damage, critChance, penetration, fireRate, reloadTime, accuracy, force, range, cameraShake, shakeDuration; //fireRate oznacza czas miêdzy strza³ami w s, a reaload iloœæ s,

    // -- Special Gun Stats
    public float critDamage, armorShred, vulnerableApplied, pierceDamage, DoT, specialCharge;
    public int magazineSize, overload, bulletsLeft, ammo, ammoFromPack, bulletSpread, pierce, special;
    public int[] Slots, Costs;
    public bool infiniteMagazine, infiniteAmmo, individualReload;
    float temp;
    int tempi;

    public GameObject bulletPrefab;

    void Start()
    {
        bulletsLeft = magazineSize;
    }

    public void Upgrade(int which)
    {
        switch (which)
        {
            case 0:
                damage *= 1.01f;
                cameraShake *= 1.004f;
                break;
            case 1:
                fireRate *= 0.984f;
                break;
            case 2:
                accuracy *= 0.98f;
                range += 0.02f;
                break;
            case 3:
                penetration += 0.01f;
                if (penetration > 1f)
                {
                    temp = penetration - 1f;
                    penetration = 1f;
                    damage *= 1f + temp;
                    cameraShake *= 1f + 0.4f * temp;
                }
                break;
        }

        GainSpecialCharge(0.06f + Costs[which] * 0.00008f);

        Costs[which] += 4;
    }

    public void GainSpecialCharge(float amount)
    {
        specialCharge += amount;
        if (specialCharge >= 1f)
        {
            specialCharge -= 1f;
            special++;
        }
    }

    public void SpecialUpgrade(int which)
    {
        switch (which)
        {
            case 0:
                damage *= 1.02f;
                cameraShake *= 1.008f;
                fireRate *= 0.96f;
                reloadTime *= 0.99f;
                break;
            case 1:
                critChance += 0.025f;
                if (critChance > 1f)
                {
                    temp = critChance - 1f;
                    critChance = 1f;
                    critDamage *= 1f + temp;
                }
                critDamage += 0.02f * (1 + critDamage);
                break;
            case 2:
                temp = 0.25f + 0.15f * magazineSize;
                if (temp < 1f)
                {
                    magazineSize++;
                    reloadTime *= 1.9f - 0.9f * temp;
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
                temp = 0.03f * fireRate * (0.1f + 0.9f * bulletSpread);
                temp *= 1 + 1.2f * penetration;
                armorShred += temp;
                fireRate *= 0.975f;
                break;
            case 4:
                vulnerableApplied += 0.02f * fireRate * (0.1f + 0.9f * bulletSpread);
                accuracy *= 0.96f;
                break;
            case 5:
                tempi = 2 + ammoFromPack / 5;
                ammoFromPack += tempi;
                ammo += 3 + 2 * ammoFromPack;
                break;
            case 6:
                temp = 1.02f + 0.1f / (bulletSpread + 1f);
                accuracy *= temp; cameraShake *= temp; reloadTime *= temp;

                temp = 0.04f + ((1f * bulletSpread + 0.4f) / (1f * bulletSpread + 1.4f));
                damage *= temp; armorShred *= temp; vulnerableApplied *= temp;

                bulletSpread++;
                break;
            case 7:
                damage *= 0.96f;
                DoT += 0.1f + 0.1f * DoT;
                break;
            case 8:
                if (infiniteAmmo)
                {
                    temp = (0.3f + reloadTime) / (0.2f + 1.1f * fireRate);
                    if (temp < 1f)
                    {
                        overload++;
                        reloadTime *= 1.4f - (0.4f * temp);
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
                        reloadTime *= 1f - (1.25f * temp / tempi);
                    }
                }
                else
                {
                    temp = (0.2f + reloadTime) / (0.3f + 1.2f * fireRate);
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
                }
                break;
            case 9:
                Slots[0]++;
                damage *= 1.032f;
                break;
            case 10:
                Slots[1]++;
                accuracy *= 0.93f;
                range += 0.05f;
                break;
            case 11:
                Slots[2]++;
                reloadTime *= 0.936f;
                break;
            case 12:
                Slots[3]++;
                fireRate *= 0.956f;
                break;
            case 13:
                temp = (3f + 1f * pierce) / (4f + 1f * pierce);
                pierceDamage *= temp;
                armorShred *= temp;
                vulnerableApplied *= temp;
                pierce++;
                break;
            case 14:
                temp = 1.02f + (0.08f / pierce);
                pierceDamage *= temp;
                break;
        }
    }

    public void AmmoPicked()
    {
        ammo += ammoFromPack;
    }
}
