using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gunslinger : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;
    public TMPro.TextMeshProUGUI CurrentChance;

    public float doubleShotChance, chanceBonus, rapidFireCooldown, rapidFireFireRate, rapidFireMovementSpeed, unloadCooldown, unloadMaxCooldown, unloadGap;
    int unloadCount;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            BulletTime();

        if (playerStats.task <= 0)
        {
            Action();
        }

        if (rapidFireCooldown > 0)
        {
            rapidFireCooldown -= Time.deltaTime;
            Ability1.fillAmount = 1 - (rapidFireCooldown / 32f);
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

    public void DisplayChance()
    {
        CurrentChance.text = ((doubleShotChance + chanceBonus) * 100f).ToString("0.0") + "%";
    }

    void BulletTime()
    {
        if (rapidFireCooldown <= 0)
        {
            rapidFireCooldown = 32f;

            rapidFireFireRate = 1.22f + 0.006f * playerStats.level;
            rapidFireMovementSpeed = 0.5f + rapidFireFireRate * 0.5f;

            playerStats.fireRateBonus *= rapidFireFireRate;
            playerStats.movementSpeed *= rapidFireMovementSpeed;

            Invoke("BulletTimeEnd", 5f);
        }
    }

    void BulletTimeEnd()
    {
        playerStats.fireRateBonus /= rapidFireFireRate;
        playerStats.movementSpeed /= rapidFireMovementSpeed;
    }

    void Unload()
    {
        if (unloadCooldown <= 0 && playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft > 0)
        {
            unloadCooldown = 4.25f + 2.8f * playerStats.eq.guns[playerStats.eq.equipped].fireRate;
            unloadMaxCooldown = unloadCooldown;

            unloadGap = 0.06f + 0.12f * playerStats.eq.guns[playerStats.eq.equipped].fireRate / playerStats.SpeedMultiplyer(0.55f);
            playerStats.task = 0.6f;

            unloadCount = 0;
            for (float i = 0; i < 0.5f; i += unloadGap)
            {
                Invoke("Fire", i);
                unloadCount++;
            }
        }
    }

    void Fire()
    {
        if (playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft > 0 || playerStats.eq.guns[playerStats.eq.equipped].infiniteMagazine)
            playerStats.Shoot(6f);
        else
        {
            playerStats.task -= unloadGap;
            unloadCooldown -= unloadMaxCooldown / (unloadCount * 1f + 1f);
        }
    }
}
