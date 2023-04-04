using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SteamGolem : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;
    public TMPro.TextMeshProUGUI SparePartsCount;
    public GameObject ElectricProjectal, IncendiaryProjectal;
    private Bullet firedBullet;

    public float efficientReloadCooldown, efficientReloadMaxCooldown, reloadedProcentage, overdriveCooldown, overdriveMaxCooldown, overdriveAccuracy, temp, direction;
    public int gatlingGunCharges, railGunCharges, clockworkMachine, spareParts, volleyCount, bulletsCount;

    void Update()
    {
        if (playerStats.task <= 0)
        {
            Action();
        }

        if (efficientReloadCooldown > 0)
        {
            efficientReloadCooldown -= Time.deltaTime;
            Ability1.fillAmount = 1 - (efficientReloadCooldown / efficientReloadMaxCooldown);
        }

        if (overdriveCooldown > 0)
        {
            overdriveCooldown -= Time.deltaTime;
            Ability2.fillAmount = 1 - (overdriveCooldown / overdriveMaxCooldown);
        }
    }

    void Action()
    {
        if (Input.GetMouseButton(1))
            Overdrive(playerStats.eq.equipped);
        else if (Input.GetKeyDown(KeyCode.Q))
            EfficientReload();
    }

    public void PackedUp()
    {
        gatlingGunCharges++;
        if (gatlingGunCharges >= 2)
        {
            gatlingGunCharges -= 2;
            playerStats.eq.guns[0].magazineSize++;
            playerStats.eq.guns[0].fireRate *= 0.992f;
            playerStats.eq.guns[0].reloadTime *= 0.992f;
        }
    }

    public void ChargedUp()
    {
        railGunCharges++;
        if (railGunCharges >= 3)
        {
            railGunCharges -= 3;
            playerStats.eq.guns[1].overload++;
            playerStats.eq.guns[1].reloadTime *= 0.988f;
        }
    }

    public void ClockworkMachine(int amount)
    {
        clockworkMachine += amount;
        if (clockworkMachine >= 240)
            PartGained();
    }

    void PartGained()
    {
        clockworkMachine -= 240;
        spareParts++;
        SparePartsCount.text = spareParts.ToString("0");
        playerStats.maxHealth += 2;
        playerStats.health += 2;
        playerStats.damageBonus += 0.1f;
        playerStats.fireRateBonus += 0.1f;
    }

    void EfficientReload()
    {
        if (playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft < playerStats.eq.guns[playerStats.eq.equipped].magazineSize)
        {
            if (playerStats.eq.guns[playerStats.eq.equipped].infiniteAmmo || playerStats.eq.guns[playerStats.eq.equipped].ammo > 0)
            {
                if (playerStats.eq.equipped == 2)
                {
                    playerStats.eq.guns[playerStats.eq.equipped].individualReload = false;
                    playerStats.reloading = true;
                    playerStats.NewTask(0.075f + 0.125f * playerStats.eq.guns[playerStats.eq.equipped].reloadTime * (playerStats.eq.guns[playerStats.eq.equipped].magazineSize - playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft));
                    efficientReloadMaxCooldown = 0.75f + 5f * playerStats.eq.guns[0].reloadTime * (playerStats.eq.guns[playerStats.eq.equipped].magazineSize - playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft);
                    Invoke("SwapReload", 0.15f + 0.125f * playerStats.eq.guns[playerStats.eq.equipped].reloadTime * (playerStats.eq.guns[playerStats.eq.equipped].magazineSize - playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft));
                }
                else
                {
                    playerStats.reloading = true;
                    reloadedProcentage = 1f * (playerStats.eq.guns[playerStats.eq.equipped].magazineSize - playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft) / (1f * playerStats.eq.guns[playerStats.eq.equipped].magazineSize);
                    playerStats.NewTask(0.075f + 0.125f * playerStats.eq.guns[playerStats.eq.equipped].reloadTime * reloadedProcentage);
                    efficientReloadMaxCooldown = 0.75f + 5f * playerStats.eq.guns[0].reloadTime * reloadedProcentage;
                }
            }
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[15] > 0)
            {
                for (int i = 0; i < playerStats.eq.guns[playerStats.eq.equipped].Accessories[15]; i++)
                {
                    efficientReloadMaxCooldown *= 0.925f;
                }
            }
            efficientReloadCooldown = efficientReloadMaxCooldown;
        }
    }

    void SwapReload()
    {
        playerStats.eq.guns[playerStats.eq.equipped].individualReload = true;
    }

    void Overdrive(int gun)
    {
        if (overdriveCooldown <= 0)
        {
            switch (gun)
            {
                case 0:
                    if (playerStats.eq.guns[0].bulletsLeft >= 2 + playerStats.eq.guns[0].bulletSpread)
                    {
                        overdriveMaxCooldown = 3.7f + 50f * playerStats.eq.guns[0].fireRate / (1f + 0.012f * spareParts);

                        for (int i = 0; i < 2 + playerStats.eq.guns[0].bulletSpread; i++)
                        {
                            Invoke("BulletVolley", 0.1f + i * (0.5f / (2f + playerStats.eq.guns[0].bulletSpread)));
                        }
                        playerStats.NewTask(1f);
                    }
                    break;
                case 1:
                    if (playerStats.eq.guns[1].bulletsLeft > 0)
                    {
                        overdriveMaxCooldown = 2.4f + 10f * playerStats.eq.guns[1].fireRate;

                        ElectricGrenade();
                        playerStats.NewTask(playerStats.eq.guns[1].fireRate);
                    }
                    break;
                case 2:
                    if (playerStats.eq.guns[2].bulletsLeft > 0)
                    {
                        overdriveMaxCooldown = 10f * playerStats.eq.guns[2].fireRate;

                        IncendiaryGrenade();
                        playerStats.NewTask(playerStats.eq.guns[2].fireRate);
                    }
                    break;
            }
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[15] > 0)
            {
                for (int i = 0; i < playerStats.eq.guns[playerStats.eq.equipped].Accessories[15]; i++)
                {
                    overdriveMaxCooldown *= 0.925f;
                }
            }
            overdriveCooldown = overdriveMaxCooldown;
        }
    }

    void BulletVolley()
    {
        playerStats.eq.guns[0].bulletsLeft--;
        playerStats.DisplayAmmo();

        bulletsCount = Mathf.FloorToInt((1f + 0.04f * volleyCount + (0.02f + 0.001f * volleyCount) * playerStats.eq.guns[0].magazineSize) / (0.25f + playerStats.eq.guns[0].fireRate) * playerStats.SpeedMultiplyer(0.8f));
        overdriveAccuracy = playerStats.eq.guns[0].accuracy * 0.35f / (0.9f + 0.1f * bulletsCount);
        direction = (-overdriveAccuracy) * (bulletsCount - 1);
        for (int i = 0; i < bulletsCount; i++)
        {
            playerStats.FireDirection(direction + overdriveAccuracy * 2 * i);
        }

        volleyCount++;
        if (volleyCount == 2 + playerStats.eq.guns[0].bulletSpread)
            volleyCount = 0;
    }

    void ElectricGrenade()
    {
        for (int i = 0; i < playerStats.eq.guns[1].bulletSpread; i++)
        {
            playerStats.eq.guns[1].bulletsLeft--;
            playerStats.DisplayAmmo();

            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-playerStats.eq.guns[1].accuracy, playerStats.eq.guns[1].accuracy));
            GameObject bullet = Instantiate(ElectricProjectal, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * playerStats.eq.guns[1].force * Random.Range(0.74f, 0.87f), ForceMode2D.Impulse);
            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;

            SetBullet(1);
            firedBullet.damage *= (1.06f + 0.005f * playerStats.level + 0.007f * spareParts);
            firedBullet.stunChance = playerStats.eq.guns[1].stunChance * 3f + 0.09f + 0.01f * playerStats.level;
            firedBullet.stunDuration = playerStats.eq.guns[1].stunDuration + 0.2f;
        }
    }

    void IncendiaryGrenade()
    {
        for (int i = 0; i < playerStats.eq.guns[2].bulletSpread; i++)
        {
            playerStats.eq.guns[2].bulletsLeft--;
            playerStats.DisplayAmmo();

            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-playerStats.eq.guns[1].accuracy, playerStats.eq.guns[1].accuracy));
            GameObject bullet = Instantiate(IncendiaryProjectal, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * playerStats.eq.guns[1].force * Random.Range(0.95f, 1.11f), ForceMode2D.Impulse);
            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;

            SetBullet(2);
            firedBullet.slowDuration += 0.36f + 0.09f * spareParts;
            firedBullet.incendiary = playerStats.eq.guns[2].damage * 0.07f;
        }
    }

    void SetBullet(int which)
    {
        firedBullet.duration = playerStats.eq.guns[which].range;
        firedBullet.damage = playerStats.eq.guns[which].damage * playerStats.DamageDealtMultiplyer(1f);
        firedBullet.DoT = playerStats.eq.guns[which].DoT;
        firedBullet.penetration = playerStats.eq.guns[which].penetration;
        firedBullet.armorShred = playerStats.eq.guns[which].armorShred;
        firedBullet.vulnerableApplied = playerStats.eq.guns[which].vulnerableApplied;
        firedBullet.slowDuration = playerStats.eq.guns[which].slowDuration;
        firedBullet.stunChance = playerStats.eq.guns[which].stunChance;
        firedBullet.stunDuration = playerStats.eq.guns[which].stunDuration;
        firedBullet.pierce = playerStats.eq.guns[which].pierce;
        firedBullet.pierceEfficiency = playerStats.eq.guns[which].pierceEfficiency;
        if (playerStats.eq.guns[which].critChance + playerStats.additionalCritChance >= Random.Range(0f, 1f))
        {
            firedBullet.damage *= playerStats.eq.guns[1].critDamage;
            firedBullet.armorShred *= 0.6f + playerStats.eq.guns[which].critDamage * 0.4f;
            firedBullet.vulnerableApplied *= 0.6f + playerStats.eq.guns[which].critDamage * 0.4f;
            firedBullet.slowDuration = 0.7f + playerStats.eq.guns[which].critDamage * 0.3f;
            firedBullet.stunChance *= 0.4f + playerStats.eq.guns[which].critDamage * 0.6f;
            firedBullet.stunDuration *= 0.7f + playerStats.eq.guns[which].stunDuration * 0.3f;
            firedBullet.pierceEfficiency *= 1.1f;
            firedBullet.crit = true;
        }
    }
}
