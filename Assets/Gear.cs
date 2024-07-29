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

    [Header("Ammos")]
    public int[] AmmoList;
    public int[] MagazineSize, Ammo;

    [Header("Weapon")]
    public int startingWeapon;
    public int[] WeaponList;
    public bool[] BaseWeapon;
    public int[] BaseWeapons, Weapons;

    [Header("Revolver")]
    public GameObject RevolverBullet;
    private float revolverDamage, revolverFireRate, revolverCritDamageBonus, revolverCritChanceBonus, revolverReloadTime, revolverAccuracy;
    private int revolverProjectileCount, revolverMagazineSize, revolverAmmo, revolverAmmoText;
    private bool revolverUpgrade;
    private Bullet revolverFired;

    void Start()
    {
        CollectBaseWeapon(startingWeapon);
    }

    public void CollectBaseWeapon(int which)
    {
        BaseWeapons[which]++;

        switch (which, BaseWeapons[which])
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
                break;
            case (0, 5):
                revolverDamage = 46f;
                revolverFireRate = 0.49f;
                revolverCritDamageBonus = 0.2f;
                break;
            case (0, 6):
                revolverUpgrade = true;
                break;
        }

        if (BaseWeapons[which] == 1)
        {
            WeaponObject[weaponsCollected].SetActive(true);
            WeaponImage[weaponsCollected].sprite = WLibrary.BaseWeapons[which].WeaponSprite;
            WeaponLevel[weaponsCollected].text = "1";
            WeaponList[weaponsCollected] = which;
            BaseWeapon[weaponsCollected] = true;
            if (!WLibrary.BaseWeapons[which].ammoless)
            {
                //AmmoList[weaponsCollected] = which;
                AmmoObject[weaponsCollected].SetActive(true);
                WeaponAmmo[WeaponList[weaponsCollected]].text = MagazineSize[WeaponList[weaponsCollected]].ToString("0") + "/" + Ammo[WeaponList[weaponsCollected]].ToString("0");
            }
            weaponsCollected++;
            //ammosDisplays++;
        }
    }

    void RevolverCast()
    {
        revolverAccuracy = 4f;
        for (int i = 0; i < revolverProjectileCount + playerStats.projectileCountBonus; i++)
        {
            Invoke("RevolverFire", i * 0.04f);
        }
        Invoke("RevolverCast", revolverFireRate / playerStats.SpeedMultiplyer(1f));
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
        }

        revolverAccuracy += 4f;
    }
}
