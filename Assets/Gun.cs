using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;
    public Sprite gunSprite;
    // -- Basic Gun Stats
    public float damage, critChance, penetration, fireRate, reloadTime, accuracy, force, range, cameraShake, shakeDuration; //fireRate oznacza czas miêdzy strza³ami w s, a reaload iloœæ s,

    // -- Special Gun Stats
    public float critDamage, armorShred, vulnerableApplied, slowDuration, stunChance, stunDuration, pierceEfficiency, DoT, specialCharge;
    public int magazineSize, overload, bulletsLeft, ammo, ammoFromPack, bulletSpread, pierce, special;
    public int[] MaxSlots, TakenSlots, Costs, Accessories;
    public bool infiniteMagazine, infiniteAmmo, individualReload;
    float temp;
    int tempi;

    public GameObject bulletPrefab;

    void Start()
    {
        bulletsLeft = magazineSize + overload;
    }

    public void Upgrade(int which)
    {
        switch (which)
        {
            case 0:
                damage *= 1.012f;
                cameraShake *= 1.004f;
                break;
            case 1:
                fireRate *= 0.982f;
                break;
            case 2:
                accuracy *= 0.975f;
                range += 0.025f;
                break;
            case 3:
                penetration += 0.012f;
                if (penetration > 1f)
                {
                    temp = penetration - 1f;
                    penetration = 1f;
                    damage *= 1f + temp;
                    cameraShake *= 1f + 0.33f * temp;
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
                damage *= 1.024f;
                cameraShake *= 1.008f;
                fireRate *= 0.955f;
                reloadTime *= 0.985f;
                break;
            case 1:
                critChance += 0.03f;
                if (critChance > 1f)
                {
                    temp = critChance - 1f;
                    critChance = 1f;
                    critDamage *= 1f + temp;
                }
                critDamage += 0.02f * (1.25f + critDamage);
                break;
            case 2:
                temp = 0.26f + 0.18f * magazineSize;
                if (infiniteAmmo)
                {
                    temp *= 1.35f;
                    reloadTime *= 0.965f;
                }

                if (temp < 1f)
                {
                    magazineSize++;
                    reloadTime *= 1.8f - 0.8f * temp;
                }
                else
                {
                    tempi = 0;
                    for (float i = 0.8f; i < temp; i++)
                    {
                        magazineSize++;
                        tempi++;
                    }
                    temp = temp - tempi;
                    if (temp < 0f)
                        reloadTime *= (1f + 0.8f * tempi / tempi);
                    else reloadTime *= 1f - (0.7f * temp / tempi);
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
                temp = 1.002f + 0.08f / (bulletSpread * 1f + 1f);
                accuracy *= temp; cameraShake *= temp; reloadTime *= temp;

                temp = 1.012f - (0.12f / (bulletSpread * 1.5f));
                fireRate *= temp;

                temp = 0.065f - (0.38f / (bulletSpread * 2.25f + 1.2f)) + ((bulletSpread * 1.01f + 0.35f) / (bulletSpread + 1.14f));
                damage *= temp; armorShred *= temp; vulnerableApplied *= temp;

                bulletSpread++;
                break;
            case 7:
                damage *= 0.96f;
                DoT += 0.12f + 0.14f * DoT;
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
                        for (float i = 0.85f; i < temp; i++)
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
                MaxSlots[0]++;
                damage *= 1.038f;
                break;
            case 10:
                MaxSlots[1]++;
                accuracy *= 0.92f;
                range += 0.06f;
                break;
            case 11:
                MaxSlots[2]++;
                reloadTime *= 0.928f;
                break;
            case 12:
                MaxSlots[3]++;
                fireRate *= 0.95f;
                break;
            case 13:
                temp = (2.5f + 1f * pierce) / (3.5f + 1f * pierce);
                pierceEfficiency *= temp;
                pierce++;
                break;
            case 14:
                temp = 1.035f + (0.07f / pierce);
                pierceEfficiency *= temp;
                break;
        }
    }

    public void AmmoPicked()
    {
        ammo += ammoFromPack;
    }
}
