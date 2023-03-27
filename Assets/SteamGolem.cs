using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SteamGolem : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;
    public GameObject ElectricProjectal, IncendiaryProjectal;
    private Bullet firedBullet;

    public float overdriveCooldown, overdriveMaxCooldown, overdriveAccuracy, temp, direction;
    public int volleyCount, bulletsCount;

    void Update()
    {
        if (playerStats.task <= 0)
        {
            Action();
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

    void Overdrive(int gun)
    {
        if (overdriveCooldown <= 0)
        {
            switch (gun)
            {
                case 0:
                    if (playerStats.eq.guns[0].bulletsLeft >= 2 + playerStats.eq.guns[0].bulletSpread)
                    {
                        overdriveMaxCooldown = 3.7f + 50f * playerStats.eq.guns[0].fireRate;
                        overdriveCooldown = overdriveMaxCooldown;

                        for (int i = 0; i < 2 + playerStats.eq.guns[0].bulletSpread; i++)
                        {
                            Invoke("BulletVolley", 0.1f + i * (0.5f / (2f + playerStats.eq.guns[0].bulletSpread)));
                        }
                        playerStats.task = 1f;
                    }
                    break;
                case 1:
                    if (playerStats.eq.guns[1].bulletsLeft > 0)
                    {
                        overdriveMaxCooldown = 2.4f + 10f * playerStats.eq.guns[1].fireRate;
                        overdriveCooldown = overdriveMaxCooldown;
                        ElectricGrenade();
                        playerStats.task = playerStats.eq.guns[1].fireRate;
                    }
                    break;
                case 2:
                    if (playerStats.eq.guns[2].bulletsLeft > 0)
                    {
                        overdriveMaxCooldown = 10f * playerStats.eq.guns[2].fireRate;
                        overdriveCooldown = overdriveMaxCooldown;
                        IncendiaryGrenade();
                        playerStats.task = playerStats.eq.guns[2].fireRate;
                    }
                    break;
            }
        }
    }

    void BulletVolley()
    {
        playerStats.eq.guns[0].bulletsLeft--;
        playerStats.DisplayAmmo();

        bulletsCount = Mathf.FloorToInt((1f + 0.04f * volleyCount + (0.028f + 0.002f * volleyCount) * playerStats.eq.guns[0].magazineSize) / (0.25f + playerStats.eq.guns[0].fireRate));
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
        playerStats.eq.guns[1].bulletsLeft--;
        playerStats.DisplayAmmo();

        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-playerStats.eq.guns[1].accuracy, playerStats.eq.guns[1].accuracy));
        GameObject bullet = Instantiate(ElectricProjectal, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * playerStats.eq.guns[1].force * Random.Range(0.74f, 0.87f), ForceMode2D.Impulse);
        firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;

        SetBullet(1);
        firedBullet.damage *= (1.12f + 0.01f * playerStats.level);
        firedBullet.stunChance = playerStats.eq.guns[1].stunChance * 3f + 0.08f + 0.01f * playerStats.level;
        firedBullet.stunDuration = playerStats.eq.guns[1].stunDuration + 0.2f;
    }

    void IncendiaryGrenade()
    {
        playerStats.eq.guns[2].bulletsLeft--;
        playerStats.DisplayAmmo();

        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-playerStats.eq.guns[1].accuracy, playerStats.eq.guns[1].accuracy));
        GameObject bullet = Instantiate(IncendiaryProjectal, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * playerStats.eq.guns[1].force * Random.Range(0.95f, 1.11f), ForceMode2D.Impulse);
        firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;

        SetBullet(2);
        firedBullet.incendiary = playerStats.eq.guns[2].damage * 0.11f;
    }

    void SetBullet(int which)
    {
        firedBullet.duration = playerStats.eq.guns[which].range;
        firedBullet.damage = playerStats.eq.guns[which].damage * playerStats.DamageDealtMultiplyer(1f);
        firedBullet.DoT = playerStats.eq.guns[which].DoT;
        firedBullet.penetration = playerStats.eq.guns[which].penetration;
        firedBullet.armorShred = playerStats.eq.guns[which].armorShred;
        firedBullet.vulnerableApplied = playerStats.eq.guns[which].vulnerableApplied;
        firedBullet.stunChance = playerStats.eq.guns[which].stunChance;
        firedBullet.stunDuration = playerStats.eq.guns[which].stunDuration;
        firedBullet.pierce = playerStats.eq.guns[which].pierce;
        firedBullet.pierceDamage = playerStats.eq.guns[which].pierceDamage;
        if (playerStats.eq.guns[which].critChance + playerStats.additionalCritChance >= Random.Range(0f, 1f))
        {
            firedBullet.damage *= playerStats.eq.guns[1].critDamage;
            firedBullet.armorShred *= 0.6f + playerStats.eq.guns[which].critDamage * 0.4f; firedBullet.vulnerableApplied *= 0.6f + playerStats.eq.guns[which].critDamage * 0.4f;
            firedBullet.stunChance *= 0.4f + playerStats.eq.guns[which].critDamage * 0.6f; firedBullet.stunDuration *= 0.7f + playerStats.eq.guns[which].stunDuration * 0.3f;
            firedBullet.crit = true;
        }
    }
}
