using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Engineer : MonoBehaviour
{
    public PlayerController playerStats;
    public GameObject TurretBullet, TurretRocket, EMPGrenade;
    public Image Ability1, Ability2, extraToolBar, extraShieldBar;
    public Transform TurretBarrel;
    public Bullet bulletFired;

    public float scrapCollected, scrapRequired, scrapStored, turretCooldown, turretMaxCooldown, turretBaseCooldown, turretDuration, turretFireRate, grenadeCooldown, grenadeMaxCooldown;
    public int toolsCollected, totalUpgrades;
    public bool turretActive;
    float timeToFire;
    int turretCount, rocketCount, nextRocket;
        
    public bool[] passivePerks, ability1Perks, ability2Perks;

    public void ConstructScrap(float value)
    {
        scrapCollected += value;
        while (scrapCollected >= scrapRequired)
        {
            scrapCollected -= scrapRequired;
            scrapStored += 18f;
            playerStats.GainTools(1);
        }
        extraToolBar.fillAmount = scrapCollected / scrapRequired;
    }

    public void ConstructTools(int value)
    {
        toolsCollected += value;
        while (toolsCollected >= 8)
        {
            toolsCollected -= 8;
            Upgrade();
        }
        extraShieldBar.fillAmount = toolsCollected / 8f;
    }

    void Upgrade()
    {
        totalUpgrades++;
        if (passivePerks[2])
        {
            playerStats.maxShield += 3;
            playerStats.GainShield(8);
        }
        else
        {
            playerStats.maxShield += 2;
            playerStats.GainShield(5);
        }
        if (passivePerks[4])
        {
            playerStats.damageBonus += 0.008f;
            playerStats.fireRateBonus += 0.01f;
            playerStats.movementSpeed += 0.09f;
            playerStats.cooldownReduction += 0.02f;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ActivateTurret();
        if (Input.GetMouseButton(1))
            ThrowGrenade();

        if (turretCooldown > 0)
        {
            turretCooldown -= Time.deltaTime;
            Ability1.fillAmount = 1 - (turretCooldown / turretMaxCooldown);
        }

        if (grenadeCooldown > 0)
        {
            grenadeCooldown -= Time.deltaTime;
            Ability2.fillAmount = 1 - (grenadeCooldown / grenadeMaxCooldown);
        }

        if (turretActive)
        {
            timeToFire -= Time.deltaTime;
            if (timeToFire <= 0f)
            {
                turretCount++;
                if (ability1Perks[4])
                {
                    if (turretCount == nextRocket)
                        TurretLaunch();
                    else TurretFire();
                }
                else TurretFire();
            }
        }
    }

    void ActivateTurret()
    {
        if (turretCooldown <= 0f)
        {
            turretMaxCooldown = turretBaseCooldown / playerStats.cooldownReduction; ;
            turretCooldown = turretMaxCooldown;

            turretCount = 0;
            rocketCount = 0;
            nextRocket = 5;

            turretFireRate = 0.5f / (1 + 0.02f * playerStats.level);
            timeToFire += turretFireRate;

            Invoke("DeactivateTurret", turretDuration);

            turretActive = true;
        }
    }

    void DeactivateTurret()
    {
        TurretBarrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, 0f);
        turretActive = false;
    }

    void TurretFire()
    {
        timeToFire += turretFireRate;

        TurretBarrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-8f, 8f));
        GameObject bullet = Instantiate(TurretBullet, TurretBarrel.position, TurretBarrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 18f * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
        bulletFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
        if (ability1Perks[3])
        {
            bulletFired.damage = (13.8f + 1.2f * playerStats.level + 0.06f * (playerStats.maxShield - 100)) * playerStats.DamageDealtMultiplyer(1f);
            turretFireRate /= 1f + 0.07f * (playerStats.maxShield - 100);
        }
        else bulletFired.damage = (12.6f + 0.9f * playerStats.level) * playerStats.DamageDealtMultiplyer(1f);

        if (ability1Perks[1])
        {
            bulletFired.damage *= 1f + 0.025f * turretCount;
            turretFireRate /= 1.033f;
        }
        if (ability1Perks[2])
        {
            bulletFired.penetration += 0.15f;
            bulletFired.armorShred += 0.012f;
        }
    }

    void TurretLaunch()
    {
        timeToFire += turretFireRate;

        TurretBarrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-8f, 8f));
        GameObject bullet = Instantiate(TurretRocket, TurretBarrel.position, TurretBarrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 18f * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
        bulletFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
        bulletFired.damage = (13.8f + 1.2f * playerStats.level + 0.06f * (playerStats.maxShield - 100)) * playerStats.DamageDealtMultiplyer(1f);
        turretFireRate /= 1f + 0.07f * (playerStats.maxShield - 100);
        bulletFired.damage *= 1.037f + 0.005f * playerStats.level;
        bulletFired.damage *= 1f + 0.025f * turretCount;
        turretFireRate /= 1.033f;

        rocketCount++;
        nextRocket += 5 + rocketCount;
    }

    void ThrowGrenade()
    {
        if (grenadeCooldown <= 0f)
        {
            grenadeMaxCooldown = 32f / playerStats.cooldownReduction; ;
            grenadeCooldown = grenadeMaxCooldown;

            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation);
            GameObject bullet = Instantiate(EMPGrenade, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * 12f, ForceMode2D.Impulse);
            bulletFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
            bulletFired.damage = (25f + 2f * playerStats.level) * playerStats.DamageDealtMultiplyer(1f);
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
                        // passive - increases tools & Scrap gained every round
                        break;
                    case 1:
                        scrapRequired -= 18f;
                        ConstructScrap(scrapStored);
                        break;
                    case 2:
                        playerStats.maxShield += totalUpgrades;
                        playerStats.GainShield(3 * totalUpgrades);
                        // passive - increases Shield Capacity & Shield gained from Tools
                        break;
                    case 3:
                        // passive - gives extra ammo per Tools
                        break;
                    case 4:
                        // passive - Upgrade! - collecting Tools also increases stats
                        playerStats.damageBonus += 0.008f * totalUpgrades;
                        playerStats.fireRateBonus += 0.01f * totalUpgrades;
                        playerStats.movementSpeed += 0.09f * totalUpgrades;
                        playerStats.cooldownReduction += 0.02f * totalUpgrades;
                        break;
                }
                break;
            case 1:
                ability1Perks[which] = true;
                switch (which)
                {
                    case 0:
                        turretDuration += 1.5f;
                        turretBaseCooldown -= 3f;
                        break;
                    case 1:
                        // passive - turret gains increased damage & fire rate with duration
                        break;
                    case 2:
                        // passive - turret gains penetration & armor Shred
                        break;
                    case 3:
                        // passive - increases turret damage & fire rate
                        break;
                    case 4:
                        // passive - Rocket Barrage - every so often fire an explosive rocket instead
                        break;
                }
                break;
            case 2:
                ability2Perks[which] = true;
                switch (which)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                }
                break;
        }
    }
}
