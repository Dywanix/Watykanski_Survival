using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;
    public Sprite gunSprite;

    [Header("Basic Staty")]
    public float damage;
    public float fireRate;
    public float accuracy;
    public float penetration;
    public float critChance;
    public float reloadTime;
    public float range;
    public float force; //fireRate oznacza czas miêdzy strza³ami w s, a reaload iloœæ s,
    public int magazineSize;
    public int ammo;
    public int maxAmmo;

    [Header("Special Staty")]
    public float critDamage;
    public float armorShred;
    public float vulnerableApplied;
    public int bulletSpread;
    public int pierce;
    public float pierceEfficiency;
    public float DoT;
    public int overload;
    public float slowDuration;
    public float stunChance;
    public float stunDuration;

    [Header("Inne Staty")]
    public float specialCharge;
    public float cameraShake, shakeDuration;
    public int bulletsLeft, ammoFromPack, spreadMultiplyer, special;
    public int MaxSlots, TakenSlots;
    public int[] Costs, Accessories;
    public bool infiniteMagazine, infiniteAmmo, individualReload;
    float temp;
    int tempi;

    [Header("Multiplikatory Stat")]
    public float damageMultiplier;
    public int magazineMultiplier = 1;
    //public float fireRateMultiplier;

    [Header("Graficzne Staty")]
    public GameObject[] bulletPrefab;
    public GameObject flashPrefab;
    public int flashCount;
    public float flashSpread;

    void Start()
    {
        bulletsLeft = magazineSize + overload;
    }

    public void Upgrade(int which)
    {
        switch (which)
        {
            case 0:
                damage *= 1.014f;
                cameraShake *= 1.0042f;
                break;
            case 1:
                fireRate *= 0.979f;
                break;
            case 2:
                accuracy *= 0.973f;
                range += 0.027f;
                break;
            case 3:
                penetration += 0.014f;
                if (penetration > 1f)
                {
                    temp = penetration - 1f;
                    penetration = 1f;
                    damage *= 1f + temp;
                    cameraShake *= 1f + 0.3f * temp;
                }
                armorShred *= 1.07f;
                break;
        }

        GainSpecialCharge(0.12f + Costs[which] * 0.00015f);

        Costs[which] += 2;
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
                damage *= 1.06f;
                cameraShake *= 1.02f;
                break;
            case 1:
                fireRate *= 0.91f;
                break;
            case 2:
                accuracy *= 0.87f;
                range += 0.13f;
                break;
            case 3:
                penetration += 0.06f;
                if (penetration > 1f)
                {
                    temp = penetration - 1f;
                    penetration = 1f;
                    damage *= 1f + temp;
                    cameraShake *= 1f + 0.33f * temp;
                }
                armorShred *= 1.3f;
                break;
            case 4:
                critChance += 0.04f;
                if (critChance > 1f)
                {
                    temp = critChance - 1f;
                    critChance = 1f;
                    critDamage *= 1f + temp;
                }
                critDamage += 0.03f + 0.025f * critDamage;
                break;
            case 5:
                temp = 0.3f + 0.2f * MagazineTotalSize();
                if (infiniteAmmo)
                {
                    temp *= 1.35f;
                    reloadTime *= 0.965f;
                }

                if (temp < 1f)
                {
                    magazineSize++;
                    reloadTime *= 1.75f - 0.75f * temp;
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
                        reloadTime *= (1f + 0.75f * tempi / tempi);
                    else reloadTime *= 1f - (0.65f * temp / tempi);
                }
                break;
            case 6:
                temp = 0.05f * fireRate * (0.2f + 0.8f * bulletSpread);
                temp *= 1 + 1.2f * penetration;
                armorShred += temp;
                break;
            case 7:
                vulnerableApplied += 0.035f * fireRate * (0.2f + 0.8f * bulletSpread);
                break;
            case 8:
                temp = 1.002f + 0.08f / (bulletSpread * 1f + 1f);
                accuracy *= temp; cameraShake *= temp; reloadTime *= temp;

                temp = 1.012f - (0.12f / (bulletSpread * 1.5f));
                fireRate *= temp;

                temp = 0.065f - (0.38f / (bulletSpread * 2.25f + 1.2f)) + ((bulletSpread * 1.01f + 0.35f) / (bulletSpread + 1.14f));
                damage *= temp; armorShred *= temp; vulnerableApplied *= temp;

                bulletSpread++;
                break;
            case 9:
                damage *= 0.964f;
                DoT += 0.135f + 0.18f * DoT;
                break;
            case 10:
                temp = (2f + 1f * pierce) / (2.8f + 1f * pierce);
                pierceEfficiency *= temp;
                pierce++;
                break;
            case 11:
                temp = 1.04f + (0.08f / pierce);
                pierceEfficiency *= temp;
                break;
        }
    }

    public void AmmoPicked()
    {
        ammo += ammoFromPack;
    }

    public int BulletsFired()
    {
        return bulletSpread * spreadMultiplyer;
    }

    public float Damage()
    {
        return damage * damageMultiplier;
    }

    public int MagazineTotalSize()
    {
        return magazineSize * magazineMultiplier;
    }
}
