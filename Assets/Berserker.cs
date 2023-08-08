using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Berserker : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;
    public TMPro.TextMeshProUGUI WrathCount;
    public GameObject Wave, BoomerangAxe;
    public Bullet AxeThrown;

    public float wrath, burstCooldown, burstMaxCooldown, swipeCooldown, swipeMaxCooldown;
    int wavesCount;
    float waveDirection;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            BurstCast();

        if (playerStats.task <= 0)
        {
            Action();
        }

        if (burstCooldown > 0)
        {
            burstCooldown -= Time.deltaTime;
            Ability1.fillAmount = 1 - (burstCooldown / burstMaxCooldown);
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

    public void GainWrath(float value)
    {
        wrath += value * 0.06f / 100f;
        WrathCount.text = (wrath * 100).ToString("0.0") + "%";
    }

    void BurstCast()
    {
        if (burstCooldown <= 0)
        {
            burstMaxCooldown = 11f / playerStats.cooldownReduction;
            burstCooldown = burstMaxCooldown;

            Burst();
        }
        else if (playerStats.health > 5f)
        {
            playerStats.TakeDamage(5f, true);
            Burst();
        }
    }

    void Burst()
    {
        GainWrath(2f);
        wavesCount = 5 + playerStats.level / 2;

        waveDirection = Random.Range(-12f, 12f);
        for (int i = 0; i < wavesCount; i++)
        {
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + waveDirection + i * (360f / wavesCount));
            GameObject bullet = Instantiate(Wave, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * 12.4f * playerStats.DamageDealtMultiplyer(0.18f), ForceMode2D.Impulse);
            AxeThrown = bullet.GetComponent(typeof(Bullet)) as Bullet;
            AxeThrown.damage = (17.6f + 1.6f * playerStats.level) * playerStats.DamageDealtMultiplyer(1f);
        }
    }

    /*void Enrage()
    {
        if (enrageCooldown <= 0)
        {
            enrageMaxCooldown = 33f / playerStats.cooldownReduction; ;
            enrageCooldown = enrageMaxCooldown;

            healthSacrifice = playerStats.health * 0.3f;
            playerStats.TakeDamage(healthSacrifice, true);

            enrageFireRateIncrease = 1.108f + 0.004f * playerStats.level + 0.0036f * healthSacrifice;
            enrageDamageIncrease = 0.58f + enrageFireRateIncrease * 0.42f;

            playerStats.fireRateBonus *= enrageFireRateIncrease;
            playerStats.damageBonus *= enrageDamageIncrease;

            Invoke("EnrageEnd", 6f);
        }
    }

    void EnrageEnd()
    {
        playerStats.RestoreHealth(healthSacrifice * 0.31f);

        playerStats.fireRateBonus /= enrageFireRateIncrease;
        playerStats.damageBonus /= enrageDamageIncrease;
    }*/

    void SwipeCast()
    {
        if (swipeCooldown <= 0)
        {
            swipeMaxCooldown = 12f / playerStats.cooldownReduction; ;
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
        bullet_body.AddForce(playerStats.Barrel.up * 14.4f * playerStats.DamageDealtMultiplyer(0.83f), ForceMode2D.Impulse);
        AxeThrown = bullet.GetComponent(typeof(Bullet)) as Bullet;
        AxeThrown.damage = (29 + 2f * playerStats.level + 0.045f * playerStats.maxHealth) * playerStats.DamageDealtMultiplyer(1.65f);
    }
}
