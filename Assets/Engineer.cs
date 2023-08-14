using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Engineer : MonoBehaviour
{
    public PlayerController playerStats;
    public GameObject TurretBullet, EMPGrenade;
    public Image Ability1, Ability2, extraToolBar, extraShieldBar;
    public Transform TurretBarrel;
    public Bullet bulletFired;

    public float scrapCollected, scrapRequired, turretCooldown, turretMaxCooldown, turretFireRate, grenadeCooldown, grenadeMaxCooldown;
    public int toolsCollected, totalUpgrades;
    public bool turretActive;
    float timeToFire;

    public void ConstructScrap(float value)
    {
        scrapCollected += value;
        while (scrapCollected >= scrapRequired)
        {
            scrapCollected -= scrapRequired;
            playerStats.GainTools(1);
        }
        extraToolBar.fillAmount = scrapCollected / scrapRequired;
    }

    public void ConstructTools(int value)
    {
        toolsCollected += value;
        while (toolsCollected >= 8)
        {
            toolsCollected -= 8;
            Upgrade();
        }
        extraShieldBar.fillAmount = toolsCollected / 8f;
    }

    void Upgrade()
    {
        totalUpgrades++;
        playerStats.maxShield += 2;
        playerStats.GainShield(5);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ActivateTurret();
        if (Input.GetMouseButton(1))
            ThrowGrenade();

        if (turretCooldown > 0)
        {
            turretCooldown -= Time.deltaTime;
            Ability1.fillAmount = 1 - (turretCooldown / turretMaxCooldown);
        }

        if (grenadeCooldown > 0)
        {
            grenadeCooldown -= Time.deltaTime;
            Ability2.fillAmount = 1 - (grenadeCooldown / grenadeMaxCooldown);
        }

        if (turretActive)
        {
            timeToFire -= Time.deltaTime;
            if (timeToFire <= 0f)
                TurretFire();
        }
    }

    void ActivateTurret()
    {
        if (turretCooldown <= 0f)
        {
            turretMaxCooldown = 38f / playerStats.cooldownReduction; ;
            turretCooldown = turretMaxCooldown;

            turretFireRate = 0.5f / (1 + 0.02f * playerStats.level);
            timeToFire += turretFireRate;

            Invoke("DeactivateTurret", 6f);

            turretActive = true;
        }
    }

    void DeactivateTurret()
    {
        TurretBarrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, 0f);
        turretActive = false;
    }

    void TurretFire()
    {
        timeToFire += turretFireRate;

        TurretBarrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-8f, 8f));
        GameObject bullet = Instantiate(TurretBullet, TurretBarrel.position, TurretBarrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 18f * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
        bulletFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
        bulletFired.damage = (12.6f + 0.9f * playerStats.level) * playerStats.DamageDealtMultiplyer(1f);
    }

    void ThrowGrenade()
    {
        if (grenadeCooldown <= 0f)
        {
            grenadeMaxCooldown = 32f / playerStats.cooldownReduction; ;
            grenadeCooldown = grenadeMaxCooldown;

            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation);
            GameObject bullet = Instantiate(EMPGrenade, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * 12f, ForceMode2D.Impulse);
            bulletFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
            bulletFired.damage = (25f + 2f * playerStats.level) * playerStats.DamageDealtMultiplyer(1f);
        }
    }
}
