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

    public float clockworkMachine, efficientReloadCooldown, efficientReloadMaxCooldown, reloadedProcentage, overdriveCooldown, overdriveMaxCooldown, overdriveAccuracy, temp, direction;
    public int efficientReloadOverload, requiredParts, spareParts, volleyCount, bulletsCount;

    void Update()
    {
        if (playerStats.task <= 0)
        {
            Action();
        }
        if (Input.GetKeyDown(KeyCode.Q))
            EfficientReload();

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
    }

    public void ClockworkMachine(float amount)
    {
        clockworkMachine += amount;
        if (clockworkMachine >= requiredParts)
            PartGained();
    }

    void PartGained()
    {
        clockworkMachine -= requiredParts;
        requiredParts += 10;
        spareParts++;
        SparePartsCount.text = spareParts.ToString("0");
        playerStats.GainHP(2);
        playerStats.damageBonus += 0.1f;
        playerStats.fireRateBonus += 0.1f;

        switch (playerStats.eq.equipped)
        {
            case 0:
                playerStats.eq.guns[playerStats.eq.equipped].magazineSize++;
                playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft++;
                break;
            case 1:
                playerStats.eq.guns[playerStats.eq.equipped].damage *= 1.007f;
                playerStats.eq.guns[playerStats.eq.equipped].reloadTime *= 0.996f;
                break;
        }
    }

    void EfficientReload()
    {
        if (playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft < playerStats.eq.guns[playerStats.eq.equipped].magazineSize)
        {
            efficientReloadOverload = Mathf.RoundToInt(playerStats.eq.guns[playerStats.eq.equipped].magazineSize * (0.5f + 0.05f * playerStats.level));
            playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft += efficientReloadOverload;
            playerStats.DisplayAmmo();

            efficientReloadMaxCooldown = 0.8f + 6.8f * playerStats.eq.guns[playerStats.eq.equipped].reloadTime;
            efficientReloadMaxCooldown /= playerStats.cooldownReduction;
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
            overdriveMaxCooldown = 0.02f;
            switch (gun)
            {
                case 0:
                    if (playerStats.eq.guns[0].bulletsLeft >= 2 + playerStats.eq.guns[0].bulletSpread)
                    {
                        overdriveMaxCooldown = 3.3f + 44f * playerStats.eq.guns[0].fireRate;

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
            overdriveMaxCooldown /= playerStats.cooldownReduction;
            overdriveCooldown = overdriveMaxCooldown;
        }
    }

    void BulletVolley()
    {
        playerStats.eq.guns[0].bulletsLeft--;
        playerStats.DisplayAmmo();

        bulletsCount = Mathf.FloorToInt((1f + 0.04f * volleyCount + (0.02f + 0.001f * volleyCount) * playerStats.eq.guns[0].magazineSize) / (0.25f + playerStats.eq.guns[0].fireRate) * playerStats.SpeedMultiplyer(0.8f));
        overdriveAccuracy = playerStats.eq.guns[0].accuracy * 0.35f / (0.9f + 0.01f * playerStats.level + 0.11f * bulletsCount);
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
            playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;

            playerStats.SetBullet();
            playerStats.firedBullet.damage *= (1.08f + 0.007f * playerStats.level);
            playerStats.firedBullet.stunChance = playerStats.eq.guns[1].stunChance * 3f + 0.09f + 0.01f * playerStats.level;
            playerStats.firedBullet.stunDuration = playerStats.eq.guns[1].stunDuration + 0.2f;
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
            playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;

            playerStats.SetBullet();
            playerStats.firedBullet.slowDuration += 0.44f;
            playerStats.firedBullet.incendiary = playerStats.eq.guns[2].damage * 0.07f;
        }
    }
}
