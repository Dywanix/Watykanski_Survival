using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Berserker : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;
    public TMPro.TextMeshProUGUI ChargesCount;
    public GameObject BoomerangAxe;
    public Bullet Axe, AxeThrown;

    public float axeDamage, axeCharging, damageTaken, enrageCooldown, enrageMaxCooldown, healthSacrifice, enrageFireRateIncrease, enrageDamageIncrease, swipeCooldown, swipeMaxCooldown;
    public int axeCharges, axeMaxCharges;

    void Start()
    {
        UpdateAxeDamage();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Enrage();

        if (playerStats.task <= 0)
        {
            Action();
        }

        if (axeCharges < axeMaxCharges)
        {
            axeCharging += (0.83f + 0.11f * playerStats.level) * Time.deltaTime;
            if (axeCharging >= 1f)
            {
                axeCharging -= 1f;
                AxeGainCharge();
            }
        }

        if (enrageCooldown > 0)
        {
            enrageCooldown -= Time.deltaTime;
            Ability1.fillAmount = 1 - (enrageCooldown / enrageMaxCooldown);
        }

        if (swipeCooldown > 0)
        {
            swipeCooldown -= Time.deltaTime;
            Ability2.fillAmount = 1 - (swipeCooldown / swipeMaxCooldown);
        }
    }

    void AxeGainCharge()
    {
        axeCharges++;
        ChargesCount.text = axeCharges.ToString("0");
        UpdateAxeDamage();
    }

    public void AxeStuck()
    {
        if (axeCharges > 0)
        {
            if (axeCharges >= 8)
                axeCharges -= 2;
            else axeCharges--;
        }
        ChargesCount.text = axeCharges.ToString("0");
        UpdateAxeDamage();
    }

    void Action()
    {
        if (Input.GetMouseButton(1))
            SwipeCast();
    }

    public void AxeDamageIncrease(float amount)
    {
        damageTaken += amount;
        UpdateAxeDamage();
    }

    public void UpdateAxeDamage()
    {
        axeDamage = 15.02f + 0.574f * playerStats.level + damageTaken / 420f;
        axeDamage *= playerStats.DamageDealtMultiplyer(1f);
        if (axeCharges > 0)
        {
            axeDamage *= 1.92f;
            if (axeCharges >= 8)
                axeDamage *= 1.23f;
        }
        Axe.damage = axeDamage;
    }


    void Enrage()
    {
        if (enrageCooldown <= 0)
        {
            enrageMaxCooldown = 34f;
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[3 + playerStats.accessoriesPerType] > 0)
            {
                for (int i = 0; i < playerStats.eq.guns[playerStats.eq.equipped].Accessories[3 + playerStats.accessoriesPerType]; i++)
                {
                    enrageMaxCooldown *= 0.91f;
                }
            }
            enrageCooldown = enrageMaxCooldown;

            healthSacrifice = playerStats.health * 0.27f;
            playerStats.TakeDamage(healthSacrifice);

            enrageFireRateIncrease = 1.108f + 0.004f * playerStats.level + 0.0036f * healthSacrifice;
            enrageDamageIncrease = 0.58f + enrageFireRateIncrease * 0.42f;
            UpdateAxeDamage();

            playerStats.fireRateBonus *= enrageFireRateIncrease;
            playerStats.damageBonus *= enrageDamageIncrease;

            Invoke("EnrageEnd", 6f);
        }
    }

    void EnrageEnd()
    {
        playerStats.RestoreHealth(healthSacrifice * 0.27f);

        playerStats.fireRateBonus /= enrageFireRateIncrease;
        playerStats.damageBonus /= enrageDamageIncrease;
        UpdateAxeDamage();
    }

    void SwipeCast()
    {
        if (swipeCooldown <= 0)
        {
            swipeMaxCooldown = 12f;
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[3 + playerStats.accessoriesPerType] > 0)
            {
                for (int i = 0; i < playerStats.eq.guns[playerStats.eq.equipped].Accessories[3 + playerStats.accessoriesPerType]; i++)
                {
                    swipeMaxCooldown *= 0.91f;
                }
            }
            swipeCooldown = swipeMaxCooldown;

            Invoke("Swipe", 0.2f);

            playerStats.NewTask(0.7f);
        }
    }

    void Swipe()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-3f, 3f));
        GameObject bullet = Instantiate(BoomerangAxe, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 14f * playerStats.DamageDealtMultiplyer(0.8f), ForceMode2D.Impulse);
        AxeThrown = bullet.GetComponent(typeof(Bullet)) as Bullet;
        AxeThrown.damage = (33 + 3f * playerStats.level + 0.06f * playerStats.maxHealth) * playerStats.DamageDealtMultiplyer(1.65f);
    }
}
