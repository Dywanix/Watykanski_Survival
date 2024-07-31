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

    [Header("Rail Spike")]
    public GameObject RailSpikeBullet;
    private float railSpikeDamage, railSpikeDamageRatio, railSpikeFireRate, railSpikePierceDamage, railSpikeReloadTime, railSpikeAim;
    private int railSpikePierce;
    private bool railSpikeUpgrade;
    private Bullet railSpikeFired;

    [Header("Immolation")]
    public GameObject ImmolationObject;
    public GameObject ImmolationAoE;
    private float immolationDamage, immolationHealthRatio;
    public float immolationAreaSize;
    private Bullet immolationTick;

    [Header("Napalm")]
    public GameObject NapalmGrenade;
    public Transform NapalmRotation, NapalmDistance;
    private float napalmDamage, napalmFireRate, napalmAreaSize, napalmDuration, napalmReloadTime;
    private int napalmProjectileCount;
    private Bullet NapalmFired;

    void Start()
    {
        CollectWeapon(startingWeapon);
        //CollectWeapon(4);
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
                revolverCritChanceBonus = 0.15f;
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
                revolverDamage = 48f;
                revolverFireRate = 0.47f;
                revolverCritChanceBonus = 0.18f;
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
                WeaponAmmo[AmmoList[1]].text = Ammo[1].ToString("0") + "/" + MagazineSize[1].ToString("0");
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
                WeaponAmmo[AmmoList[1]].text = Ammo[1].ToString("0") + "/" + MagazineSize[1].ToString("0");
                break;
            case (1, 5):
                shotgunDamage = 46f;
                shotgunFireRate = 1.4f;
                shotgunReloadCount = 2;
                break;
            case (1, 6):
                shotgunDamage = 48f;
                shotgunFireRate = 1.33f;
                shotgunReloadTime = 0.16f;
                shotgunUpgrade = true;
                break;
            case (2, 1):
                railSpikeDamage = 50f;
                railSpikeDamageRatio = 1.04f;
                railSpikeFireRate = 1.11f;
                railSpikePierce = 3;
                railSpikePierceDamage = 0.75f;
                MagazineSize[2] = 3;
                Ammo[2] = 3;
                railSpikeReloadTime = 0.79f;
                Invoke("RailSpikeCast", railSpikeFireRate / playerStats.SpeedMultiplyer(1f));
                break;
            case (2, 2):
                railSpikeDamageRatio = 1.06f;
                railSpikeFireRate = 0.98f;
                railSpikePierceDamage = 0.8f;
                break;
            case (2, 3):
                railSpikeDamage = 69f;
                railSpikePierce = 4;
                break;
            case (2, 4):
                railSpikeDamageRatio = 1.08f;
                railSpikePierceDamage = 0.85f;
                MagazineSize[2] = 5;
                Ammo[2] += 2;
                WeaponAmmo[AmmoList[2]].text = Ammo[2].ToString("0") + "/" + MagazineSize[2].ToString("0");
                break;
            case (2, 5):
                railSpikeDamage = 86f;
                railSpikeFireRate = 0.91f;
                railSpikePierceDamage = 0.9f;
                break;
            case (2, 6):
                railSpikeDamage = 90f;
                railSpikeFireRate = 0.86f;
                railSpikePierce = 5;
                railSpikeReloadTime = 0.74f;
                railSpikeUpgrade = true;
                break;
            case (3, 1):
                ImmolationObject.SetActive(true);
                immolationDamage = 1.5f;
                immolationHealthRatio = 0.04f;
                immolationAreaSize = 0.8f;
                ImmolationObject.transform.localScale = new Vector3(immolationAreaSize * playerStats.areaSizeBonus, immolationAreaSize * playerStats.areaSizeBonus, 1f);
                Invoke("ImmolateCast", 0.4f);
                break;
            case (3, 2):
                immolationDamage = 3f;
                immolationAreaSize = 0.9f;
                ImmolationObject.transform.localScale = new Vector3(immolationAreaSize * playerStats.areaSizeBonus, immolationAreaSize * playerStats.areaSizeBonus, 1f);
                break;
            case (3, 3):
                immolationDamage = 4f;
                immolationHealthRatio = 0.05f;
                break;
            case (3, 4):
                immolationDamage = 6f;
                immolationAreaSize = 1f;
                ImmolationObject.transform.localScale = new Vector3(immolationAreaSize * playerStats.areaSizeBonus, immolationAreaSize * playerStats.areaSizeBonus, 1f);
                break;
            case (3, 5):
                immolationHealthRatio = 0.06f;
                immolationAreaSize = 1.15f;
                ImmolationObject.transform.localScale = new Vector3(immolationAreaSize * playerStats.areaSizeBonus, immolationAreaSize * playerStats.areaSizeBonus, 1f);
                break;
            case (3, 6):
                immolationDamage = 8f;
                immolationAreaSize = 1.25f;
                ImmolationObject.transform.localScale = new Vector3(immolationAreaSize * playerStats.areaSizeBonus, immolationAreaSize * playerStats.areaSizeBonus, 1f);
                break;
            case (4, 1):
                napalmDamage = 12f;
                napalmFireRate = 3.2f;
                napalmProjectileCount = 2;
                napalmAreaSize = 0.5f;
                napalmDuration = 3.5f;
                napalmReloadTime = 4.2f;
                MagazineSize[4] = 6;
                Ammo[4] = 6;
                Invoke("NapalmCast", napalmFireRate / playerStats.SpeedMultiplyer(1f));
                break;
            case (4, 2):
                napalmDamage = 14f;
                napalmProjectileCount = 3;
                napalmReloadTime = 4f;
                break;
            case (4, 3):
                napalmFireRate = 2.75f;
                napalmAreaSize = 0.6f;
                break;
            case (4, 4):
                napalmDamage = 17f;
                napalmProjectileCount = 4;
                napalmDuration = 3.8f;
                break;
            case (4, 5):
                napalmDamage = 20f;
                napalmFireRate = 2.55f;
                napalmAreaSize = 0.7f;
                break;
            case (4, 6):
                napalmDamage = 21f;
                napalmFireRate = 2.45f;
                napalmAreaSize = 0.75f;
                napalmDuration = 4f;
                napalmReloadTime = 3.8f;
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
                WeaponAmmo[weaponsCollected].text = MagazineSize[WeaponList[weaponsCollected]].ToString("0") + "/" + Ammo[WeaponList[weaponsCollected]].ToString("0");
            }
            weaponsCollected++;
            //ammosDisplays++;
        }
        else
            WeaponLevel[WeaponList[which]].text = Weapons[which].ToString("0");
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
            shotgunFired.damage *= 1f + 0.08f * playerStats.projectileCountBonus;

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

    void RailSpikeCast()
    {
        if (Ammo[2] > 0)
        {
            railSpikeAim = Random.Range(0f, 360f);
            RailSpikeFire();
            Ammo[2]--;
            WeaponAmmo[AmmoList[2]].text = Ammo[2].ToString("0") + "/" + MagazineSize[2].ToString("0");
            Invoke("RailSpikeCast", railSpikeFireRate / playerStats.SpeedMultiplyer(1f));
        }
        else
            Invoke("RailSpikeReload", railSpikeReloadTime);
    }

    void RailSpikeFire()
    {
        tempi = 1 + playerStats.projectileCountBonus;
        for (int i = 0; i < tempi; i++)
        {
            NapalmRotation.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            GameObject bullet = Instantiate(NapalmGrenade, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * 15f * Random.Range(1f, 1.02f), ForceMode2D.Impulse);

            railSpikeFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
            railSpikeFired.duration = 1.75f * playerStats.durationBonus;
            railSpikeFired.damage = railSpikeDamage * playerStats.DamageDealtMultiplyer(railSpikeDamageRatio);
            railSpikeFired.pierce = railSpikePierce;
            railSpikeFired.pierceEfficiency = railSpikePierceDamage;

            if (playerStats.CritChance > Random.Range(0f, 1f))
            {
                railSpikeFired.damage *= playerStats.CritDamage;
                railSpikeFired.crit = true;
            }
        }
    }

    void RailSpikeReload()
    {
        Ammo[2] = MagazineSize[2];
        WeaponAmmo[AmmoList[2]].text = Ammo[2].ToString("0") + "/" + MagazineSize[2].ToString("0");
        RailSpikeCast();
    }

    void ImmolateCast()
    {
        GameObject tick = Instantiate(ImmolationAoE, transform.position, transform.rotation);

        immolationTick = tick.GetComponent(typeof(Bullet)) as Bullet;
        immolationTick.damage = (immolationDamage + immolationHealthRatio * playerStats.maxHealth) * playerStats.DamageDealtMultiplyer(1f);
        tick.transform.localScale = new Vector3(immolationAreaSize * playerStats.areaSizeBonus, immolationAreaSize * playerStats.areaSizeBonus, 1f);

        Invoke("ImmolateCast", 0.4f);
    }

    void NapalmCast()
    {
        if (Ammo[4] > 0)
        {
            //revolverAccuracy = 4f;
            for (int i = 0; i < napalmProjectileCount + playerStats.projectileCountBonus; i++)
            {
                Invoke("NapalmFire", i * 0.16f);
            }
            Ammo[4]--;
            WeaponAmmo[AmmoList[4]].text = Ammo[4].ToString("0") + "/" + MagazineSize[4].ToString("0");
            Invoke("NapalmCast", napalmFireRate / playerStats.SpeedMultiplyer(1f));
        }
        else
            Invoke("NapalmReload", napalmReloadTime);
    }

    void NapalmFire()
    {
        NapalmDistance.position = new Vector2(0f + NapalmRotation.position.x, Random.Range(4f, 7f + playerStats.areaSizeBonus) + NapalmRotation.position.y);
        NapalmRotation.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        GameObject bullet = Instantiate(NapalmGrenade, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();

        NapalmFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
        NapalmFired.TargetedLocation = NapalmDistance;
        NapalmFired.damage = napalmDamage;
        NapalmFired.areaSize = napalmAreaSize * playerStats.areaSizeBonus;
        NapalmFired.durationValue = napalmDuration * playerStats.durationBonus;

        if (playerStats.CritChance > Random.Range(0f, 1f))
        {
            NapalmFired.damage *= playerStats.CritDamage;
            revolverFired.crit = true;
        }

        revolverAccuracy += 4f;
    }

    void NapalmReload()
    {
        Ammo[4] = MagazineSize[4];
        WeaponAmmo[AmmoList[4]].text = Ammo[4].ToString("0") + "/" + MagazineSize[4].ToString("0");
        NapalmCast();
    }
}
