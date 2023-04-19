using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Berserker : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;
    public TMPro.TextMeshProUGUI AxeDamage;
    public GameObject BoomerangAxe;
    public Bullet Axe, AxeThrown;

    public float axeDamage, damageTaken, enrageCooldown, enrageMaxCooldown, healthSacrifice, enrageFireRateIncrease, enrageDamageIncrease, swipeCooldown, swipeMaxCooldown;

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
        axeDamage = 22 + playerStats.level + damageTaken / 360f;
        axeDamage *= playerStats.DamageDealtMultiplyer(1f);
        Axe.damage = axeDamage;
        AxeDamage.text = axeDamage.ToString("0");
    }


    void Enrage()
    {
        if (enrageCooldown <= 0)
        {
            enrageMaxCooldown = 34f;
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[15] > 0)
            {
                for (int i = 0; i < playerStats.eq.guns[playerStats.eq.equipped].Accessories[15]; i++)
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
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[15] > 0)
            {
                for (int i = 0; i < playerStats.eq.guns[playerStats.eq.equipped].Accessories[15]; i++)
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
