using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Berserker : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;
    public TMPro.TextMeshProUGUI SurvivorCount;
    public GameObject SwipeWave;
    private Bullet SwipeBullet;

    public float survivorCharge, requiredCharge, survivorCount, enrageCooldown, enrageMaxCooldown, healthSacrifice, enrageFireRateIncrease, enrageDamageIncrease, swipeCooldown, swipeMaxCooldown;

    void Start()
    {
        requiredCharge = 240f;
        survivorCharge = 0;
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

    public void GainCharge(float amount)
    {
        survivorCharge += amount;
        if (survivorCharge >= requiredCharge)
        {
            playerStats.maxHealth += 1.2f;
            playerStats.health += 1.2f;
            playerStats.damageBonus += 0.0002f;
            playerStats.healthBar.fillAmount = playerStats.health / playerStats.maxHealth;
            survivorCharge -= requiredCharge;
            requiredCharge += 5;
            survivorCount++;
            SurvivorCount.text = survivorCount.ToString("0");
        }
    }
    
    void Enrage()
    {
        if (enrageCooldown <= 0)
        {
            enrageMaxCooldown = 35f;
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[15] > 0)
            {
                for (int i = 0; i < playerStats.eq.guns[playerStats.eq.equipped].Accessories[15]; i++)
                {
                    enrageMaxCooldown *= 0.925f;
                }
            }
            enrageCooldown = enrageMaxCooldown;

            healthSacrifice = playerStats.health * 0.27f;
            playerStats.TakeDamage(healthSacrifice);

            enrageFireRateIncrease = 1.102f + 0.004f * playerStats.level + 0.0035f * healthSacrifice;
            enrageDamageIncrease = 0.6f + enrageFireRateIncrease * 0.4f;

            playerStats.fireRateBonus *= enrageFireRateIncrease;
            playerStats.damageBonus *= enrageDamageIncrease;

            Invoke("EnrageEnd", 6f);
        }
    }

    void EnrageEnd()
    {
        playerStats.RestoreHealth(healthSacrifice * 0.26f);

        playerStats.fireRateBonus /= enrageFireRateIncrease;
        playerStats.damageBonus /= enrageDamageIncrease;
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
                    swipeMaxCooldown *= 0.925f;
                }
            }
            swipeCooldown = swipeMaxCooldown;

            Invoke("Swipe", 0.4f);

            playerStats.NewTask(0.8f);
        }
    }

    void Swipe()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-3f, 3f));
        GameObject bullet = Instantiate(SwipeWave, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 16f * playerStats.DamageDealtMultiplyer(0.8f), ForceMode2D.Impulse);
        SwipeBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        SwipeBullet.damage = (30 + 3f * playerStats.level + 0.06f * playerStats.maxHealth) * playerStats.DamageDealtMultiplyer(1.65f);
    }
}
