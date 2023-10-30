using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gunslinger : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;
    public TMPro.TextMeshProUGUI CurrentChance;

    public float doubleShotChance, chanceBonus, rapidFireCooldown, rapidFireMaxCooldown, rapidFireFireRate, rapidFireMovementSpeed, rapidFireDuration, rapidFireReloadRate, 
        unloadCooldown, unloadMaxCooldown, unloadDuration, unloadGap, unloadRechargeBonus, unloadAccuracy, unloadDamageBonus;
    int unloadCount, unloadFired;

    public bool[] passivePerks, ability1Perks, ability2Perks;

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
            unloadCooldown -= Time.deltaTime * (1f + unloadRechargeBonus);
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
            rapidFireMaxCooldown = 29f / playerStats.cooldownReduction;
            rapidFireCooldown = rapidFireMaxCooldown;

            rapidFireFireRate = 1.21f + 0.018f * playerStats.level;
            if (ability1Perks[2])
            {
                rapidFireFireRate += 0.15f;
                if (ability1Perks[4])
                    rapidFireFireRate += 0.09f;
                rapidFireMovementSpeed = 0.44f + rapidFireFireRate * 0.56f;
            }
            else rapidFireMovementSpeed = 0.54f + rapidFireFireRate * 0.46f;

            playerStats.fireRateBonus *= rapidFireFireRate;
            playerStats.movementSpeed *= rapidFireMovementSpeed;

            if (ability1Perks[0])
                unloadRechargeBonus = 0.45f;

            if (ability1Perks[1])
            {
                doubleShotChance += 0.04f;
                playerStats.additionalCritChance += 0.04f;
                if (ability1Perks[4])
                {
                    doubleShotChance += 0.03f;
                    playerStats.additionalCritChance += 0.03f;
                }
                DisplayChance();
            }

            if (ability1Perks[3])
            {
                rapidFireReloadRate = (0.7f + playerStats.eq.guns[playerStats.eq.equipped].reloadTime) * 10f / (4f + 0.12f + playerStats.eq.guns[playerStats.eq.equipped].MagazineTotalSize());
                for (float i = 0; i < rapidFireDuration; i += rapidFireReloadRate)
                {
                    Invoke("RapidReload", i);
                }
            }

            Invoke("RapidFireEnd", rapidFireDuration);
        }
    }

    void RapidReload()
    {
        if (playerStats.eq.guns[playerStats.eq.equipped].infiniteAmmo)
        {
            if (playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft < playerStats.eq.guns[playerStats.eq.equipped].MagazineTotalSize())
            {
                playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft++;
                if (ability1Perks[4])
                    playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft++;
            }
        }
        else
        {
            if (playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft < playerStats.eq.guns[playerStats.eq.equipped].MagazineTotalSize())
            {
                if (playerStats.eq.guns[playerStats.eq.equipped].ammo > 0)
                {
                    playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft++;
                    playerStats.eq.guns[playerStats.eq.equipped].ammo--;
                }
                if (ability1Perks[4])
                    playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft++;
            }
        }
        playerStats.DisplayAmmo();
    }

    void RapidFireEnd()
    {
        playerStats.fireRateBonus /= rapidFireFireRate;
        playerStats.movementSpeed /= rapidFireMovementSpeed;

        unloadRechargeBonus = 0f;

        if (ability1Perks[1])
        {
            doubleShotChance -= 0.04f;
            playerStats.additionalCritChance -= 0.04f;
            if (ability1Perks[4])
            {
                doubleShotChance -= 0.03f;
                playerStats.additionalCritChance -= 0.03f;
            }
            DisplayChance();
        }
    }

    void Unload()
    {
        if (unloadCooldown <= 0 && playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft > 0)
        {
            if (ability2Perks[1])
                unloadMaxCooldown = 3.36f + 2.52f * playerStats.eq.guns[playerStats.eq.equipped].fireRate;
            else unloadMaxCooldown = 4.2f + 2.8f * playerStats.eq.guns[playerStats.eq.equipped].fireRate;
            unloadMaxCooldown /= playerStats.cooldownReduction;
            unloadCooldown = unloadMaxCooldown;

            if (ability2Perks[0])
            {
                if (ability2Perks[4])
                    unloadGap = 0.035f + 0.09f * playerStats.eq.guns[playerStats.eq.equipped].fireRate / playerStats.SpeedMultiplyer(0.8f);
                else unloadGap = 0.04f + 0.1f * playerStats.eq.guns[playerStats.eq.equipped].fireRate / playerStats.SpeedMultiplyer(0.8f);
            }
            else unloadGap = 0.05f + 0.12f * playerStats.eq.guns[playerStats.eq.equipped].fireRate / playerStats.SpeedMultiplyer(0.67f);
            playerStats.NewTask(0.66f);

            if (ability2Perks[2])
            {
                unloadDamageBonus = 1.06f + 0.002f * playerStats.level;
                playerStats.damageBonus *= unloadDamageBonus;
            }
            if (ability2Perks[3])
                doubleShotChance += 0.078f + 0.001f * playerStats.level;

            unloadCount = 0;
            for (float i = 0; i < unloadDuration; i += unloadGap)
            {
                Invoke("Fire", i);
                unloadCount++;
            }
            unloadFired = 0;
            Invoke("UnloadEnd", 0.66f);
        }
    }

    void Fire()
    {
        if (playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft > 0 || playerStats.eq.guns[playerStats.eq.equipped].infiniteMagazine)
        {
            unloadFired++;
            playerStats.Shoot(unloadAccuracy);
        }
        else
        {
            playerStats.task -= unloadGap;
            unloadCooldown -= unloadMaxCooldown / (unloadCount * 1f + 0.9f);
        }
        if (ability2Perks[4])
            playerStats.Fire(180f);
    }

    void UnloadEnd()
    {
        if (ability2Perks[1])
        {
            playerStats.eq.guns[playerStats.eq.equipped].bulletsLeft += (8 + 3 * unloadCount) / 10;
            playerStats.DisplayAmmo();
        }
        if (ability2Perks[2])
            playerStats.damageBonus /= unloadDamageBonus;
        if (ability2Perks[3])
            doubleShotChance -= 0.078f + 0.001f * playerStats.level;
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
                        doubleShotChance += 0.04f;
                        DisplayChance();
                        break;
                    case 1:
                        // passive - not consuming ammo
                        break;
                    case 2:
                        // passive - gain 5% of base chance for every unsuccessful
                        break;
                    case 3:
                        // passive  - increased damage when triggered
                        break;
                    case 4:
                        // passive - Triple Shot - third shot chance
                        break;
                }
                break;
            case 1:
                ability1Perks[which] = true;
                switch (which)
                {
                    case 0:
                        rapidFireDuration += 2f;
                        // passive - faster unload recharge
                        break;
                    case 1:
                        // passive - increase passive chance & crit chance while active
                        break;
                    case 2:
                        // passive - increase bonuses
                        break;
                    case 3:
                        // passive - auto reload
                        break;
                    case 4:
                        rapidFireDuration += 2f;
                        // passive - Bullet Time - couple bonuses
                        break;
                }
                break;
            case 2:
                ability2Perks[which] = true;
                switch (which)
                {
                    case 0:
                        unloadDuration = 0.67f;
                        // passive - increase fire rate of unload
                        break;
                    case 1:
                        // passive - reduce cooldown & ammo return
                        break;
                    case 2:
                        unloadAccuracy = 0.72f;
                        // passive - increase damage while unloading
                        break;
                    case 3:
                        // passive - increase passive chance while unloading
                        break;
                    case 4:
                        // passive - Bullet Storm - fire in random direction for free
                        break;
                }
                break;
        }
    }
}
