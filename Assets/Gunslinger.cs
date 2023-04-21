using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gunslinger : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;
    public TMPro.TextMeshProUGUI CurrentChance;

    public float doubleShotChance, chanceBonus, rapidFireCooldown, rapidFireMaxCooldown, rapidFireFireRate, rapidFireMovementSpeed, unloadCooldown, unloadMaxCooldown, unloadGap;
    int unloadCount;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            RapidFire();

        if (playerStats.task <= 0)
        {
            Action();
        }

        if (rapidFireCooldown > 0)
        {
            rapidFireCooldown -= Time.deltaTime;
            Ability1.fillAmount = 1 - (rapidFireCooldown / rapidFireMaxCooldown);
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

    void RapidFire()
    {
        if (rapidFireCooldown <= 0)
        {
            rapidFireMaxCooldown = 30f;
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[3 + playerStats.accessoriesPerType] > 0)
            {
                for (int i = 0; i < playerStats.eq.guns[playerStats.eq.equipped].Accessories[3 + playerStats.accessoriesPerType]; i++)
                {
                    rapidFireMaxCooldown *= 0.91f;
                }
            }
            rapidFireCooldown = rapidFireMaxCooldown;

            rapidFireFireRate = 1.22f + 0.01f * playerStats.level;
            rapidFireMovementSpeed = 0.55f + rapidFireFireRate * 0.45f;

            playerStats.fireRateBonus *= rapidFireFireRate;
            playerStats.movementSpeed *= rapidFireMovementSpeed;

            Invoke("RapidFireEnd", 5f);
        }
    }

    void RapidFireEnd()
    {
        playerStats.fireRateBonus /= rapidFireFireRate;
        playerStats.movementSpeed /= rapidFireMovementSpeed;
    }

    void Unload()
    {
        if (unloadCooldown <= 0 && playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft > 0)
        {
            unloadMaxCooldown = 4.2f + 2.8f * playerStats.eq.guns[playerStats.eq.equipped].fireRate;
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[3 + playerStats.accessoriesPerType] > 0)
            {
                for (int i = 0; i < playerStats.eq.guns[playerStats.eq.equipped].Accessories[3 + playerStats.accessoriesPerType]; i++)
                {
                    unloadMaxCooldown *= 0.91f;
                }
            }
            unloadCooldown = unloadMaxCooldown;

            unloadGap = 0.05f + 0.12f * playerStats.eq.guns[playerStats.eq.equipped].fireRate / playerStats.SpeedMultiplyer(0.6f);
            playerStats.NewTask(0.7f);

            unloadCount = 0;
            for (float i = 0; i < 0.55f; i += unloadGap)
            {
                Invoke("Fire", i);
                unloadCount++;
            }
        }
    }

    void Fire()
    {
        if (playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft > 0 || playerStats.eq.guns[playerStats.eq.equipped].infiniteMagazine)
            playerStats.Shoot(5f);
        else
        {
            playerStats.task -= unloadGap;
            unloadCooldown -= unloadMaxCooldown / (unloadCount * 1f + 1f);
        }
    }
}
