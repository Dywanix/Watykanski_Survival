using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gunslinger : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;

    public float doubleShotChance, bulletTimeCooldown, bulletTimeFireRate, bulletTimeMovementSpeed, unloadCooldown, unloadMaxCooldown, unloadGap;
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            BulletTime();

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
        if (Input.GetMouseButton(1))
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
        if (unloadCooldown <= 0 && playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft > 0)
        {
            unloadCooldown = 6f + 4 * playerStats.eq.guns[playerStats.eq.equipped].fireRate;
            unloadMaxCooldown = unloadCooldown;

            unloadGap = 0.06f + 0.12f * playerStats.eq.guns[playerStats.eq.equipped].fireRate;
            playerStats.task = 0.8f;

            for (float i = 0; i < 0.8f; i += unloadGap)
            {
                Invoke("Fire", i);
            }
        }
    }

    void Fire()
    {
        if (playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft > 0 || playerStats.eq.guns[playerStats.eq.equipped].infiniteMagazine)
            playerStats.Shoot(7f);
        else
        {
            playerStats.task -= unloadGap;
            unloadCooldown -= unloadGap * 10f;
        }
    }
}
