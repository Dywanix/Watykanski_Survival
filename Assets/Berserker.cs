using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Berserker : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;
    public TMPro.TextMeshProUGUI WrathCount;
    public GameObject BoomerangAxe;
    public Bullet AxeThrown;

    public float wrath, enrageCooldown, enrageMaxCooldown, healthSacrifice, enrageFireRateIncrease, enrageDamageIncrease, swipeCooldown, swipeMaxCooldown;

    void Start()
    {
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

    public void GainWrath(float value)
    {
        wrath += value * (0.024f + 0.001f * playerStats.level) / 100f;
        WrathCount.text = (wrath * 100).ToString("0.0") + "%";
    }

    void Enrage()
    {
        if (enrageCooldown <= 0)
        {
            enrageMaxCooldown = 33f / playerStats.cooldownReduction; ;
            enrageCooldown = enrageMaxCooldown;

            healthSacrifice = playerStats.health * 0.27f;
            playerStats.TakeDamage(healthSacrifice);

            enrageFireRateIncrease = 1.108f + 0.004f * playerStats.level + 0.0036f * healthSacrifice;
            enrageDamageIncrease = 0.58f + enrageFireRateIncrease * 0.42f;

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
    }

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
        bullet_body.AddForce(playerStats.Barrel.up * 14f * playerStats.DamageDealtMultiplyer(0.8f), ForceMode2D.Impulse);
        AxeThrown = bullet.GetComponent(typeof(Bullet)) as Bullet;
        AxeThrown.damage = (29 + 2f * playerStats.level + 0.04f * playerStats.maxHealth) * playerStats.DamageDealtMultiplyer(1.65f);
    }
}
