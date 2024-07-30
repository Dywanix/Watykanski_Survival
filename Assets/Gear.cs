using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gear : MonoBehaviour
{
    [Header("Scripts")]
    public PlayerController playerStats;
    public WeaponsLibrary WLibrary;

    [Header("UI")]
    public GameObject[] WeaponObject;
    public GameObject[] AmmoObject;
    public Image[] WeaponImage;
    public TMPro.TextMeshProUGUI[] WeaponLevel, WeaponAmmo;
    private int weaponsCollected; //ammosDisplays;

    [Header("Stats")]
    public int tempi;

    [Header("Ammos")]
    public int[] AmmoList;
    public int[] MagazineSize, Ammo;

    [Header("Weapon")]
    public int startingWeapon;
    public int[] WeaponList;
    public int[] Weapons;

    [Header("Revolver")]
    public GameObject RevolverBullet;
    private float revolverDamage, revolverFireRate, revolverCritDamageBonus, revolverCritChanceBonus, revolverReloadTime, revolverAccuracy;
    private int revolverProjectileCount;
    private bool revolverUpgrade;
    private Bullet revolverFired;

    [Header("Shotgun")]
    public GameObject ShotgunBullet;
    private float shotgunDamage, shotgunFireRate, shotgunReloadTime, shotgunAim;
    private int shotgunProjectileCount, shotgunReloadCount;
    private bool shotgunUpgrade;
    private Bullet shotgunFired;

    void Start()
    {
        CollectWeapon(startingWeapon);
        //CollectWeapon(1);
    }

    public void CollectWeapon(int which)
    {
        Weapons[which]++;

        switch (which, Weapons[which])
        {
            case (0, 1):
                revolverDamage = 29f;
                revolverFireRate = 0.72f;
                revolverCritDamageBonus = 0.1f;
                revolverCritChanceBonus = 0.1f;
                revolverReloadTime = 0.5f;
                revolverProjectileCount = 1;
                MagazineSize[0] = 6;
                Ammo[0] = 6;
                Invoke("RevolverCast", revolverFireRate / playerStats.SpeedMultiplyer(1f));
                break;
            case (0, 2):
                revolverDamage = 36f;
                revolverCritDamageBonus = 0.15f;
                break;
            case (0, 3):
                revolverFireRate = 0.54f;
                revolverCritDamageBonus = 0.15f;
                break;
            case (0, 4):
                revolverProjectileCount = 2;
                MagazineSize[0] = 7;
                Ammo[0]++;
                WeaponAmmo[AmmoList[0]].text = Ammo[0].ToString("0") + "/" + MagazineSize[0].ToString("0");
                break;
            case (0, 5):
                revolverDamage = 46f;
                revolverFireRate = 0.49f;
                revolverCritDamageBonus = 0.2f;
                break;
            case (0, 6):
                revolverUpgrade = true;
                break;
            case (1, 1):
                shotgunDamage = 32f;
                shotgunFireRate = 2.65f;
                shotgunReloadTime = 0.22f;
                shotgunProjectileCount = 3;
                shotgunReloadCount = 1;
                MagazineSize[1] = 12;
                Ammo[1] = 12;
                Invoke("ShotgunCast", shotgunFireRate / playerStats.SpeedMultiplyer(1f));
                break;
            case (1, 2):
                shotgunFireRate = 2.2f;
                shotgunProjectileCount = 4;
                MagazineSize[1] = 13;
                Ammo[1]++;
                WeaponAmmo[AmmoList[1]].text = Ammo[1].ToString("0") + "/" + MagazineSize[0].ToString("0");
                break;
            case (1, 3):
                shotgunDamage = 39f;
                shotgunFireRate = 2f;
                shotgunReloadTime = 0.18f;
                break;
            case (1, 4):
                shotgunDamage = 44f;
                shotgunProjectileCount = 5;
                MagazineSize[1] = 14;
                Ammo[1]++;
                WeaponAmmo[AmmoList[1]].text = Ammo[1].ToString("0") + "/" + MagazineSize[0].ToString("0");
                break;
            case (1, 5):
                shotgunDamage = 46f;
                shotgunFireRate = 1.4f;
                shotgunReloadCount = 2;
                break;
            case (1, 6):
                shotgunUpgrade = true;
                break;
        }

        if (Weapons[which] == 1)
        {
            WeaponObject[weaponsCollected].SetActive(true);
            WeaponImage[weaponsCollected].sprite = WLibrary.Weapons[which].WeaponSprite;
            WeaponLevel[weaponsCollected].text = "1";
            WeaponList[weaponsCollected] = which;
            if (!WLibrary.Weapons[which].ammoless)
            {
                AmmoList[which] = weaponsCollected;
                AmmoObject[weaponsCollected].SetActive(true);
                WeaponAmmo[WeaponList[weaponsCollected]].text = MagazineSize[WeaponList[weaponsCollected]].ToString("0") + "/" + Ammo[WeaponList[weaponsCollected]].ToString("0");
            }
            weaponsCollected++;
            //ammosDisplays++;
        }
    }

    void RevolverCast()
    {
        if (Ammo[0] > 0)
        {
            revolverAccuracy = 4f;
            for (int i = 0; i < revolverProjectileCount + playerStats.projectileCountBonus; i++)
            {
                Invoke("RevolverFire", i * 0.04f);
            }
            Ammo[0]--;
            WeaponAmmo[AmmoList[0]].text = Ammo[0].ToString("0") + "/" + MagazineSize[0].ToString("0");
            Invoke("RevolverCast", revolverFireRate / playerStats.SpeedMultiplyer(1f));
        }
        else
            Invoke("RevolverReload", revolverReloadTime);
    }

    void RevolverFire()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-revolverAccuracy, revolverAccuracy));
        GameObject bullet = Instantiate(RevolverBullet, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 18f * Random.Range(1f, 1.04f), ForceMode2D.Impulse);

        revolverFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
        revolverFired.duration = 1.2f * playerStats.durationBonus;
        revolverFired.damage = revolverDamage * playerStats.DamageDealtMultiplyer(1f);

        if (playerStats.CritChance + revolverCritChanceBonus > Random.Range(0f, 1f))
        {
            revolverFired.damage *= playerStats.CritDamage + revolverCritDamageBonus;
            revolverFired.crit = true;
            if (revolverUpgrade)
            {
                revolverFired.pierce += 2;
                revolverFired.pierceEfficiency = 0.66f;
            }
        }

        revolverAccuracy += 4f;
    }

    void RevolverReload()
    {
        Ammo[0] = MagazineSize[0];
        WeaponAmmo[AmmoList[0]].text = Ammo[0].ToString("0") + "/" + MagazineSize[0].ToString("0");
        RevolverCast();
    }

    void ShotgunCast()
    {
        if (Ammo[1] > 0)
        {
            shotgunAim = Random.Range(0f, 360f);
            ShotgunFire();
            if (shotgunUpgrade)
                Invoke("Shotgun2ndFire", 0.12f);
            Ammo[1]--;
            WeaponAmmo[AmmoList[1]].text = Ammo[1].ToString("0") + "/" + MagazineSize[1].ToString("0");
            Invoke("ShotgunCast", shotgunFireRate / playerStats.SpeedMultiplyer(1f));
        }
        else
            Invoke("ShotgunReload", shotgunReloadTime);
    }

    void ShotgunFire()
    {
        tempi = shotgunProjectileCount + playerStats.projectileCountBonus;
        for (int i = 0; i < tempi; i++)
        {
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + shotgunAim - (tempi - 1) * 6f + i * 12f);
            GameObject bullet = Instantiate(ShotgunBullet, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * 16f * Random.Range(1f, 1.02f), ForceMode2D.Impulse);

            shotgunFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
            shotgunFired.duration = 1.4f * playerStats.durationBonus;
            shotgunFired.damage = shotgunDamage * playerStats.DamageDealtMultiplyer(1f);

            if (playerStats.CritChance > Random.Range(0f, 1f))
            {
                shotgunFired.damage *= playerStats.CritDamage;
                shotgunFired.crit = true;
            }
        }
    }

    void Shotgun2ndFire()
    {
        tempi = shotgunProjectileCount + playerStats.projectileCountBonus - 1;
        for (int i = 0; i < tempi; i++)
        {
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + shotgunAim - (tempi - 1) * 6f + i * 12f);
            GameObject bullet = Instantiate(ShotgunBullet, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * 16f * Random.Range(1f, 1.02f), ForceMode2D.Impulse);

            shotgunFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
            shotgunFired.duration = 1.4f * playerStats.durationBonus;
            shotgunFired.damage = shotgunDamage * playerStats.DamageDealtMultiplyer(1f);

            if (playerStats.CritChance > Random.Range(0f, 1f))
            {
                shotgunFired.damage *= playerStats.CritDamage;
                shotgunFired.crit = true;
            }
        }
    }

    void ShotgunReload()
    {
        Ammo[1] += shotgunReloadCount;
        if (Ammo[1] > MagazineSize[1])
            Ammo[1] = MagazineSize[1];
        WeaponAmmo[AmmoList[1]].text = Ammo[1].ToString("0") + "/" + MagazineSize[1].ToString("0");

        if (Ammo[1] < MagazineSize[1])
            Invoke("ShotgunReload", shotgunReloadTime);
        else ShotgunCast();
    }
}
