using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Berserker : MonoBehaviour
{
    public PlayerController playerStats;
    public Image Ability1, Ability2;
    public GameObject SwipeWave;
    private Bullet SwipeBullet;

    public float enrageCooldown, healthSacrifice, enrageFireRateIncrease, enrageDamageIncrease, swipeCooldown;

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
            Ability1.fillAmount = 1 - (enrageCooldown / 35f);
        }

        if (swipeCooldown > 0)
        {
            swipeCooldown -= Time.deltaTime;
            Ability2.fillAmount = 1 - (swipeCooldown / 12f);
        }
    }

    void Action()
    {
        if (Input.GetMouseButton(1))
            Swipe();
    }

    void Enrage()
    {
        if (enrageCooldown <= 0)
        {
            enrageCooldown = 35f;

            healthSacrifice = playerStats.health * 0.25f;
            playerStats.TakeDamage(healthSacrifice);

            enrageFireRateIncrease = 1.1f + 0.004f * playerStats.level + 0.0035f * healthSacrifice;
            enrageDamageIncrease = 0.6f + enrageFireRateIncrease * 0.4f;

            playerStats.fireRateBonus *= enrageFireRateIncrease;
            playerStats.damageIncrease *= enrageDamageIncrease;

            Invoke("EnrageEnd", 6f);
        }
    }

    void EnrageEnd()
    {
        playerStats.fireRateBonus /= enrageFireRateIncrease;
        playerStats.damageIncrease /= enrageDamageIncrease;
    }

    void Swipe()
    {
        if (swipeCooldown <= 0)
        {
            swipeCooldown = 12f;

            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-3f, 3f));
            GameObject bullet = Instantiate(SwipeWave, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * 12f * playerStats.DamageDealtMultiplyer(0.4f), ForceMode2D.Impulse);
            SwipeBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SwipeBullet.damage = (32 + 3 * playerStats.level) * playerStats.DamageDealtMultiplyer(1.6f);

            playerStats.task = 0.8f;
        }
    }
}