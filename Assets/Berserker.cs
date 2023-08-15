using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Berserker : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;
    public TMPro.TextMeshProUGUI WrathCount;
    public GameObject Wave, Axe, BoomerangAxe;
    public Bullet AxeThrown;
    private ClearProjectals axeClear;

    public float wrath, wrathGain, healthGain, healthRestored, burstCooldown, burstMaxCooldown, swipeCooldown, swipeMaxCooldown, damageTaken;
    public int bonusWaves;
    int wavesCount;
    float waveDirection;
    
    public bool[] passivePerks, ability1Perks, ability2Perks;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            BurstCast();
        if (Input.GetMouseButton(1))
            SwipeCast();

        if (burstCooldown > 0)
        {
            burstCooldown -= Time.deltaTime;
            Ability1.fillAmount = 1 - (burstCooldown / burstMaxCooldown);
        }

        if (swipeCooldown > 0)
        {
            swipeCooldown -= Time.deltaTime;
            Ability2.fillAmount = 1 - (swipeCooldown / swipeMaxCooldown);
        }
    }

    public void GainWrath(float value)
    {
        wrath += value * wrathGain / 100f;
        WrathCount.text = (wrath * 100).ToString("0.0") + "%";
        damageTaken += value;
        if (damageTaken >= 100f + 10f * bonusWaves)
        {
            damageTaken -= 100f + 10f * bonusWaves;
            bonusWaves++;
        }
    }

    void BurstCast()
    {
        if (burstCooldown <= 0)
        {
            burstMaxCooldown = 11f / playerStats.cooldownReduction;
            burstCooldown = burstMaxCooldown;

            wavesCount = 6 + playerStats.level / 3;
            wavesCount += bonusWaves;
            if (ability1Perks[0])
                wavesCount += 2 +  Mathf.FloorToInt(wrath * 20f);
            if (ability1Perks[4])
                wavesCount += 1;
            Burst();
        }
        else if (playerStats.health > 5f)
        {
            playerStats.TakeDamage(5f, true);
            wavesCount = 6 + playerStats.level / 3;
            wavesCount += bonusWaves;
            if (ability1Perks[0])
                wavesCount += 2 +  Mathf.FloorToInt(wrath * 20f);
            if (ability1Perks[4])
                wavesCount += 1;
            Burst();
        }
    }

    void Burst()
    {
        waveDirection = Random.Range(-12f, 12f);
        for (int i = 0; i < wavesCount; i++)
        {
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + waveDirection + i * (360f / wavesCount));
            BurstFire();
        }

        if (ability1Perks[4])
        {
            if (wavesCount > 6)
            {
                wavesCount -= 3;
                Invoke("Burst", 0.32f);
            }
        }
    }

    void BurstFire()
    {
        GameObject bullet = Instantiate(Wave, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 13.1f * playerStats.DamageDealtMultiplyer(0.19f), ForceMode2D.Impulse);
        AxeThrown = bullet.GetComponent(typeof(Bullet)) as Bullet;
        if (ability1Perks[2])
        {
            AxeThrown.damage = (20.8f + 2.1f * playerStats.level) * playerStats.DamageDealtMultiplyer(1f);
            AxeThrown.slowDuration += AxeThrown.damage * 0.0075f;
        }
        else AxeThrown.damage = (17.6f + 1.7f * playerStats.level) * playerStats.DamageDealtMultiplyer(1f);
        if (ability1Perks[1])
        {
            AxeThrown.pierce++;
             AxeThrown.pierceEfficiency += 0.08f;
        }
    }

    void PeriodicBurst()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(0f, 360f));
        BurstFire();

        Invoke("PeriodicBurst", 3.6f / (1f + 2f * wrath));
    }

    /*void Enrage()
    {
        if (enrageCooldown <= 0)
        {
            enrageMaxCooldown = 33f / playerStats.cooldownReduction; ;
            enrageCooldown = enrageMaxCooldown;

            healthSacrifice = playerStats.health * 0.3f;
            playerStats.TakeDamage(healthSacrifice, true);

            enrageFireRateIncrease = 1.108f + 0.004f * playerStats.level + 0.0036f * healthSacrifice;
            enrageDamageIncrease = 0.58f + enrageFireRateIncrease * 0.42f;

            playerStats.fireRateBonus *= enrageFireRateIncrease;
            playerStats.damageBonus *= enrageDamageIncrease;

            Invoke("EnrageEnd", 6f);
        }
    }

    void EnrageEnd()
    {
        playerStats.RestoreHealth(healthSacrifice * 0.31f);

        playerStats.fireRateBonus /= enrageFireRateIncrease;
        playerStats.damageBonus /= enrageDamageIncrease;
    }*/

    void SwipeCast()
    {
        if (swipeCooldown <= 0)
        {
            swipeMaxCooldown = 13.4f / playerStats.cooldownReduction; ;
            swipeCooldown = swipeMaxCooldown;

            Invoke("Swipe", 0.2f);
        }
    }

    void Swipe()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-3f, 3f));
        GameObject bullet = Instantiate(BoomerangAxe, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        AxeThrown = bullet.GetComponent(typeof(Bullet)) as Bullet;
        if (ability2Perks[3])
        {
            bullet_body.AddForce(playerStats.Barrel.up * 15.4f * playerStats.DamageDealtMultiplyer(1.03f), ForceMode2D.Impulse);
            AxeThrown.duration += 0.16f;
            if (ability2Perks[4])
                AxeThrown.duration *= 16.7f;
        }
        else bullet_body.AddForce(playerStats.Barrel.up * 14.4f * playerStats.DamageDealtMultiplyer(0.83f), ForceMode2D.Impulse);
        if (ability2Perks[2])
            AxeThrown.damage = (31.2f + 3f * playerStats.level + 0.05f * playerStats.maxHealth) * playerStats.DamageDealtMultiplyer(1.9f);
        else AxeThrown.damage = (29f + 2f * playerStats.level + 0.04f * playerStats.maxHealth) * playerStats.DamageDealtMultiplyer(1.65f);
        if (ability2Perks[0])
        {
            axeClear = bullet.GetComponent(typeof(ClearProjectals)) as ClearProjectals;
            axeClear.clears = 4 + playerStats.level / 3;
            AxeThrown.DoT += 0.12f;
        }
        if (ability2Perks[2])
        {
            AxeThrown.pierce += 2;
             AxeThrown.pierceEfficiency += 0.07f;
        }
    }

    public void GainPerk(int ability, int which)
    {
        switch (ability)
        {
            case 0:
                passivePerks[which] = true;
                switch (which)
                {
                    case 0:
                        healthRestored += 0.06f;
                        // passive - gain wrath at the start of combat
                        break;
                    case 1:
                        playerStats.GainHP(12f);
                        healthGain += 2.5f;
                        break;
                    case 2:
                        wrathGain += 0.02f;
                        break;
                    case 3:
                        // passive - wrath also increases Fire Rate
                        break;
                    case 4:
                        break;
                }
                break;
            case 1:
                ability1Perks[which] = true;
                switch (which)
                {
                    case 0:
                        // passive - increase number of waves
                        break;
                    case 1:
                        // passive - increase waves Pierce & Pierce Efficiency
                        break;
                    case 2:
                        // passive - increases waves Damage & Slow
                        break;
                    case 3:
                        Invoke("PeriodicBurst", 3.6f / (1f + 2f * wrath));
                        break;
                    case 4:
                        // passive - Endless Rage - increases number of waves, recast while high number
                        break;
                }
                break;
            case 2:
                ability2Perks[which] = true;
                switch (which)
                {
                    case 0:
                        // passive - axe destroys enemy projectals & increases DoT
                        break;
                    case 1:
                        // passive - increase Axe Pierce & Pierce Efficiency
                        break;
                    case 2:
                        // passive - increases Axe Damage
                        break;
                    case 3:
                        // passive - increase Force & Range
                        break;
                    case 4:
                        Axe = BoomerangAxe;
                        // passive - Boomerang Blade - Axe Thrown returns back to Player
                        break;
                }
                break;
        }
    }
}
