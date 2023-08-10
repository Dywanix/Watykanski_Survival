using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public string gunName;
    public Sprite gunSprite;

    [Header("Basic Staty")]
    public float damage;
    public float fireRate; //fireRate oznacza czas miêdzy strza³ami w s, a reaload iloœæ s,
    public float accuracy;
    public float penetration;
    public float critChance;
    public float reloadTime;
    public float range;
    public float force;
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
    public int burst;
    public float burstDelay;

    [Header("Inne Staty")]
    public float LevelCostCharge;
    public float cameraShake, shakeDuration;
    public int bulletsLeft, bonusAmmo, spreadMultiplyer, special;
    public int MaxSlots, TakenSlots, LevelCost, StoredCostReduction;
    public int[] Costs, Accessories;
    public float[] Values, LevelUpBonuses;
    public string[] UpgradeType, UpgradeInfo;
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
        switch (UpgradeType[which])
        {
            case "damage":
                damage += Values[which];
                break;
            case "fireRate":
                fireRate *= Values[which];
                break;
            case "accuracy":
                accuracy *= Values[which];
                break;
            case "penetration":
                penetration += Values[which];
                break;
            case "critChance":
                critChance += Values[which];
                break;
            case "reloadTime":
                reloadTime *= Values[which];
                break;
            case "range":
                range += Values[which];
                break;
            case "magazineSize":
                magazineSize += Mathf.RoundToInt(Values[which]);
                break;
            case "maxAmmo":
                ammo += Mathf.RoundToInt(Values[which]);
                maxAmmo += Mathf.RoundToInt(Values[which]);
                break;
            case "critDamage":
                critDamage += Values[which];
                break;
            case "bulletSpread":
                bulletSpread++;
                break;
            case "pierce":
                pierce++;
                break;
            case "DoT":
                DoT += Values[which];
                break;
            case "overload":
                overload += Mathf.RoundToInt(Values[which]);
                break;
            case "special":
                special++;
                break;
        }
        /*switch (which)
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
        }*/

        ExperienceGain(Costs[which] * 0.25f);
        //GainSpecialCharge(0.12f + Costs[which] * 0.00015f);
    }

    public void LevelUp()
    {
        damage += LevelUpBonuses[0];
        fireRate *= LevelUpBonuses[1];
        accuracy *= LevelUpBonuses[2];
        penetration += LevelUpBonuses[3];
        critChance += LevelUpBonuses[4];
        reloadTime *= LevelUpBonuses[5];
        range += LevelUpBonuses[6];
        force += LevelUpBonuses[7];
        magazineSize += Mathf.RoundToInt(LevelUpBonuses[8]);
        maxAmmo += Mathf.RoundToInt(LevelUpBonuses[9]);
        critDamage += LevelUpBonuses[10];
        bulletSpread += Mathf.RoundToInt(LevelUpBonuses[11]);
        pierce += Mathf.RoundToInt(LevelUpBonuses[12]);
        pierceEfficiency += LevelUpBonuses[13];
        DoT += LevelUpBonuses[14];
        overload += Mathf.RoundToInt(LevelUpBonuses[15]);
        special += Mathf.RoundToInt(LevelUpBonuses[16]);

        MaxSlots++;

        LevelCost = 75 - StoredCostReduction;
        StoredCostReduction = 0;
        if (LevelCost < 0)
        {
            StoredCostReduction = LevelCost * -1;
            LevelCost = 0;
        }
    }

    public void ExperienceGain(float value)
    {
        LevelCostCharge += value;
        while (LevelCostCharge >= 1f)
        {
            LevelCostCharge -= 1f;
            if (LevelCost > 0)
                LevelCost--;
            else StoredCostReduction++;
        }
    }

    /*public void GainSpecialCharge(float amount)
    {
        specialCharge += amount;
        if (specialCharge >= 1f)
        {
            specialCharge -= 1f;
            special++;
        }
    }*/

    /*public void SpecialUpgrade(int which)
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
    }*/

    public void AmmoPicked()
    {
        ammo += maxAmmo / 6;
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
