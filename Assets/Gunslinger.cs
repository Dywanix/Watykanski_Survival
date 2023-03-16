using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gunslinger : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;

    public float doubleShotChance, bulletTimeCooldown, bulletTimeFireRate, bulletTimeMovementSpeed, unloadCooldown, unloadMaxCooldown;

    void Update()
    {
        if (playerStats.task <= 0)
        {
            Action();
        }
        if (bulletTimeCooldown > 0)
        {
            bulletTimeCooldown -= Time.deltaTime;
            Ability1.fillAmount = 1 - (bulletTimeCooldown / 32f);
        }
        if (unloadCooldown > 0)
        {
            unloadCooldown -= Time.deltaTime;
            Ability2.fillAmount = 1 - (unloadCooldown / unloadMaxCooldown);
        }
    }

    void Action()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            BulletTime();
        else if (Input.GetMouseButton(1))
            Unload();
    }

    void BulletTime()
    {
        if (bulletTimeCooldown <= 0)
        {
            bulletTimeCooldown = 32f;

            bulletTimeFireRate = 1.22f + 0.005f * playerStats.level;
            bulletTimeMovementSpeed = 0.5f + bulletTimeFireRate * 0.5f;

            playerStats.fireRateBonus *= bulletTimeFireRate;
            playerStats.movementSpeed *= bulletTimeMovementSpeed;

            Invoke("BulletTimeEnd", 5f);
        }
    }

    void BulletTimeEnd()
    {
        playerStats.fireRateBonus /= bulletTimeFireRate;
        playerStats.movementSpeed /= bulletTimeMovementSpeed;
    }

    void Unload()
    {
        if (unloadCooldown <= 0)
        {
            unloadCooldown = 3f + 8 * playerStats.eq.guns[playerStats.eq.equipped].fireRate;
            unloadMaxCooldown = unloadCooldown;

            playerStats.task = 0.8f;

            for (int i = 0; i < 4; i++)
            {
                Invoke("Fire", 0.16f + i * 0.16f);
            }
        }
    }

    void Fire()
    {
        if (playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft > 0 || playerStats.eq.guns[playerStats.eq.equipped].infiniteMagazine)
            playerStats.Fire(8f);
        else
        {
            playerStats.task -= 0.16f;
            unloadCooldown -= 2 * playerStats.eq.guns[playerStats.eq.equipped].fireRate;
        }
    }
}
