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
    public int weaponsCollected;

    [Header("Stats")]
    public int tempi;
    public float temp;

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
    private Bullet revolverFired;

    [Header("Shotgun")]
    public GameObject ShotgunBullet;
    private float shotgunDamage, shotgunFireRate, shotgunReloadTime, shotgunAim;
    private int shotgunProjectileCount, shotgunReloadCount;
    private Bullet shotgunFired;

    [Header("Rail Spike")]
    public GameObject RailSpikeBullet;
    private float railSpikeDamage, railSpikeDamageRatio, railSpikeFireRate, railSpikePierceDamage, railSpikeReloadTime, railSpikeAim;
    private int railSpikePierce;
    private Bullet railSpikeFired;
    private BounceBack railSpikeBounce;

    [Header("Immolation")]
    public GameObject ImmolationObject;
    public GameObject ImmolationAoE;
    private float immolationDamage, immolationHealthRatio, immolationTickRate;
    public float immolationAreaSize;
    private Bullet immolationTick;

    [Header("Napalm")]
    public GameObject NapalmGrenade;
    public Transform NapalmRotation, NapalmDistance;
    private float napalmDamage, napalmFireRate, napalmAreaSize, napalmDuration, napalmReloadTime;
    private int napalmProjectileCount;
    private Bullet NapalmFired;

    [Header("Poison Nova")]
    public GameObject PoisonNovaAoE;
    private float poisonNovaDoT, poisonNovaDoTDuration, poisonNovaFireRate, poisonNovaAreaSize, poisonNovaDuration;
    private Bullet poisonNovaFired;
    private Grow poisonNovaGrow;

    [Header("Flamethrower")]
    public GameObject FlamethrowerBullet;
    private float flamethrowerDamage, flamethrowerFireRate, flamethrowerReloadTime;
    private int flamethrowerProjectileCount, flamethrowerBurn;
    private Bullet flamethrowerFired;

    [Header("Boomerang")]
    public GameObject BoomerangBullet;
    public GameObject BoomerangBulletv2;
    private float boomerangDamage, boomerangFireRate, boomerangDuration, boomerangPierceDamage, boomerangAim;
    private int boomerangProjectileCount, boomerangPierce;
    private Bullet boomerangFired;

    [Header("Singularity")]
    public GameObject SingularityBullet;
    public GameObject EmpoweredSingularityBullet;
    private float singularityDamage, singularityFireRate, singularityAreaSize, singularityDuration;
    private Bullet singularityFired;

    void Start()
    {
        CollectWeapon(startingWeapon);
        //CollectWeapon(8);
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
                break;
            case (2, 1):
                railSpikeDamage = 52f;
                railSpikeDamageRatio = 1.04f;
                railSpikeFireRate = 1.1f;
                railSpikePierce = 3;
                railSpikePierceDamage = 0.7f;
                MagazineSize[2] = 3;
                Ammo[2] = 3;
                railSpikeReloadTime = 0.77f;
                Invoke("RailSpikeCast", railSpikeFireRate / playerStats.SpeedMultiplyer(1f));
                break;
            case (2, 2):
                railSpikeDamageRatio = 1.06f;
                railSpikeFireRate = 0.97f;
                railSpikePierceDamage = 0.75f;
                break;
            case (2, 3):
                railSpikeDamage = 72f;
                railSpikePierce = 4;
                break;
            case (2, 4):
                railSpikeDamageRatio = 1.08f;
                railSpikePierceDamage = 0.8f;
                MagazineSize[2] = 5;
                Ammo[2] += 2;
                WeaponAmmo[AmmoList[2]].text = Ammo[2].ToString("0") + "/" + MagazineSize[2].ToString("0");
                break;
            case (2, 5):
                railSpikeDamage = 90f;
                railSpikeFireRate = 0.9f;
                railSpikePierceDamage = 0.85f;
                break;
            case (2, 6):
                railSpikeDamage = 95f;
                railSpikeFireRate = 0.85f;
                railSpikePierce = 5;
                railSpikePierceDamage = 0.9f;
                railSpikeReloadTime = 0.72f;
                //railSpikeUpgrade = true;
                break;
            case (3, 1):
                ImmolationObject.SetActive(true);
                immolationDamage = 1.5f;
                immolationHealthRatio = 0.04f;
                immolationTickRate = 0.4f;
                immolationAreaSize = 0.8f;
                ImmolationObject.transform.localScale = new Vector3(immolationAreaSize * playerStats.areaSizeBonus, immolationAreaSize * playerStats.areaSizeBonus, 1f);
                Invoke("ImmolateCast", immolationTickRate);
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
                immolationTickRate = 0.38f;
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
            case (5, 1):
                poisonNovaDoT = 20f;
                poisonNovaDoTDuration = 1.7f;
                poisonNovaFireRate = 4f;
                poisonNovaAreaSize = 0.34f;
                poisonNovaDuration = 1.83f;
                Invoke("PoisonNovaCast", poisonNovaFireRate / playerStats.SpeedMultiplyer(1f));
                break;
            case (5, 2):
                poisonNovaDoT = 25f;
                poisonNovaAreaSize = 0.38f;
                break;
            case (5, 3):
                poisonNovaDoT = 30f;
                poisonNovaDoTDuration = 1.85f;
                break;
            case (5, 4):
                poisonNovaDoT = 35f;
                poisonNovaDuration = 2f;
                break;
            case (5, 5):
                poisonNovaDoT = 40f;
                poisonNovaDoTDuration = 1.9f;
                poisonNovaAreaSize = 0.42f;
                break;
            case (5, 6):
                poisonNovaDoT = 42f;
                poisonNovaDoTDuration = 2f;
                poisonNovaAreaSize = 0.45f;
                poisonNovaDuration = 2.15f;
                break;
            case (6, 1):
                flamethrowerDamage = 9f;
                flamethrowerFireRate = 0.37f;
                flamethrowerProjectileCount = 2;
                flamethrowerBurn = 1;
                flamethrowerReloadTime = 6.6f;
                MagazineSize[6] = 80;
                Ammo[6] = 100;
                Invoke("FlamethrowerCast", flamethrowerFireRate / playerStats.SpeedMultiplyer(1f));
                break;
            case (6, 2):
                flamethrowerDamage = 11f;
                flamethrowerFireRate = 0.345f;
                break;
            case (6, 3):
                flamethrowerDamage = 12f;
                flamethrowerProjectileCount = 3;
                MagazineSize[6] = 100;
                Ammo[6] += 20;
                WeaponAmmo[AmmoList[6]].text = Ammo[6].ToString("0") + "/" + MagazineSize[6].ToString("0");
                break;
            case (6, 4):
                flamethrowerDamage = 14f;
                flamethrowerBurn = 2;
                break;
            case (6, 5):
                flamethrowerDamage = 16f;
                flamethrowerFireRate = 0.315f;
                break;
            case (6, 6):
                flamethrowerDamage = 17f;
                flamethrowerFireRate = 0.295f;
                flamethrowerReloadTime = 6.3f;
                MagazineSize[6] = 110;
                Ammo[6] += 10;
                WeaponAmmo[AmmoList[6]].text = Ammo[6].ToString("0") + "/" + MagazineSize[6].ToString("0");
                break;
            case (7, 1):
                boomerangDamage = 32f;
                boomerangFireRate = 2.5f;
                boomerangProjectileCount = 2;
                boomerangDuration = 4f;
                boomerangPierce = 4;
                boomerangPierceDamage = 0.8f;
                Invoke("BoomerangCast", boomerangFireRate / playerStats.SpeedMultiplyer(1f));
                break;
            case (7, 2):
                boomerangDamage = 38f;
                boomerangProjectileCount = 3;
                break;
            case (7, 3):
                boomerangFireRate = 2.2f;
                boomerangPierce = 5;
                boomerangPierceDamage = 0.85f;
                break;
            case (7, 4):
                boomerangDamage = 44f;
                boomerangProjectileCount = 4;
                break;
            case (7, 5):
                boomerangDamage = 50f;
                boomerangFireRate = 2f;
                boomerangPierce = 6;
                break;
            case (7, 6):
                boomerangDamage = 53f;
                boomerangFireRate = 1.9f;
                boomerangDuration = 6.1f;
                boomerangPierceDamage = 0.9f;
                BoomerangBullet = BoomerangBulletv2;
                break;
            case (8, 1):
                singularityDamage = 13f;
                singularityFireRate = 5.2f;
                singularityAreaSize = 0.68f;
                singularityDuration = 4f;
                Invoke("SingularityCast", singularityFireRate / playerStats.SpeedMultiplyer(1f));
                break;
            case (8, 2):
                singularityDamage = 18f;
                singularityAreaSize = 0.72f;
                break;
            case (8, 3):
                singularityFireRate = 4.95f;
                singularityDuration = 4.2f;
                break;
            case (8, 4):
                singularityDamage = 22f;
                singularityAreaSize = 0.76f;
                break;
            case (8, 5):
                singularityDamage = 26f;
                singularityFireRate = 4.75f;
                singularityDuration = 4.4f;
                break;
            case (8, 6):
                singularityDamage = 28f;
                singularityFireRate = 4.5f;
                singularityAreaSize = 0.87f;
                SingularityBullet = EmpoweredSingularityBullet;
                break;
        }

        if (Weapons[which] == 1)
        {
            WeaponObject[weaponsCollected].SetActive(true);
            WeaponImage[weaponsCollected].sprite = WLibrary.Weapons[which].WeaponSprite;
            WeaponLevel[weaponsCollected].text = "1";
            WeaponList[which] = weaponsCollected;
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
            if (Weapons[0] > 5)
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
            if (Weapons[1] > 5)
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
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Body.rotation + shotgunAim - (tempi - 1) * 6f + i * 12f);
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
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Body.rotation + shotgunAim - (tempi - 1) * 6f + i * 12f);
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
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + railSpikeAim + i * (360f / tempi));
            GameObject bullet = Instantiate(RailSpikeBullet, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * 15f * Random.Range(1f, 1.02f), ForceMode2D.Impulse);

            railSpikeFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
            railSpikeFired.duration = 1.75f * playerStats.durationBonus;
            railSpikeFired.damage = railSpikeDamage * playerStats.DamageDealtMultiplyer(railSpikeDamageRatio);
            railSpikeFired.pierce = railSpikePierce;
            railSpikeFired.pierceEfficiency = railSpikePierceDamage;

            if (Weapons[2] > 5)
            {
                railSpikeFired.duration *= 2f;
                railSpikeBounce = bullet.GetComponent(typeof(BounceBack)) as BounceBack;
                railSpikeBounce.bounceTimer = 1.75f;
            }

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
        if (Weapons[3] > 5)
            immolationTick.burn = 1;
        Invoke("ImmolateCast", immolationTickRate);
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
        NapalmFired.damage = napalmDamage * playerStats.DamageDealtMultiplyer(1f);
        NapalmFired.areaSize = napalmAreaSize * playerStats.areaSizeBonus;
        if (Weapons[4] > 5)
        {
            NapalmFired.durationValue = napalmDuration * (playerStats.durationBonus * 1.1f - 0.1f);
            NapalmFired.damageGain = 0.18f * NapalmFired.damage;
        }
        else NapalmFired.durationValue = napalmDuration * playerStats.durationBonus;

        if (playerStats.CritChance > Random.Range(0f, 1f))
        {
            NapalmFired.damage *= playerStats.CritDamage;
            NapalmFired.crit = true;
        }
    }

    void NapalmReload()
    {
        Ammo[4] = MagazineSize[4];
        WeaponAmmo[AmmoList[4]].text = Ammo[4].ToString("0") + "/" + MagazineSize[4].ToString("0");
        NapalmCast();
    }

    void PoisonNovaCast()
    {
        GameObject nova = Instantiate(PoisonNovaAoE, transform.position, transform.rotation);

        poisonNovaFired = nova.GetComponent(typeof(Bullet)) as Bullet;
        poisonNovaGrow = nova.GetComponent(typeof(Grow)) as Grow;

        poisonNovaFired.DoT = poisonNovaDoT * playerStats.DamageDealtMultiplyer(1f);
        poisonNovaFired.durationValue = poisonNovaDoTDuration * playerStats.durationBonus;
        nova.transform.localScale = new Vector3(poisonNovaAreaSize * playerStats.areaSizeBonus, poisonNovaAreaSize * playerStats.areaSizeBonus, 1f);
        poisonNovaFired.duration = poisonNovaDuration * playerStats.durationBonus;
        temp = 0.1f + 0.7f * poisonNovaAreaSize * playerStats.areaSizeBonus;
        poisonNovaGrow.scaleChange.x = temp;
        poisonNovaGrow.scaleChange.y = temp;

        if (Weapons[5] > 5)
            poisonNovaFired.novaDetonation = true;

        Invoke("PoisonNovaCast", poisonNovaFireRate / playerStats.SpeedMultiplyer(1f));
    }

    void FlamethrowerCast()
    {
        if (Ammo[6] > 0)
        {
            FlamethrowerFire();
            Ammo[6]--;
            WeaponAmmo[AmmoList[6]].text = Ammo[6].ToString("0") + "/" + MagazineSize[6].ToString("0");
            Invoke("FlamethrowerCast", temp / playerStats.SpeedMultiplyer(1f));
        }
        else
            Invoke("FlamethrowerReload", flamethrowerReloadTime);
    }

    void FlamethrowerFire()
    {
        temp = flamethrowerFireRate / (1f + 0.48f * (flamethrowerProjectileCount + playerStats.projectileCountBonus));
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-12f - 2f / temp, 12f + 2f / temp));
        GameObject bullet = Instantiate(FlamethrowerBullet, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 11.6f * Random.Range(1f, 1.06f), ForceMode2D.Impulse);

        flamethrowerFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
        flamethrowerFired.duration = 1.1f * playerStats.durationBonus;
        flamethrowerFired.damage = flamethrowerDamage * playerStats.DamageDealtMultiplyer(1f);
        flamethrowerFired.burn = flamethrowerBurn;

        if (Weapons[6] > 5)
            flamethrowerFired.pierce++;

        if (playerStats.CritChance > Random.Range(0f, 1f))
        {
            flamethrowerFired.damage *= playerStats.CritDamage;
            flamethrowerFired.crit = true;
            if (Weapons[6] > 3)
                flamethrowerFired.burn++;
        }
        flamethrowerFired.burn += (flamethrowerFired.burn * (flamethrowerProjectileCount + playerStats.projectileCountBonus)) / 5;
    }

    void FlamethrowerReload()
    {
        Ammo[6] = MagazineSize[6];
        WeaponAmmo[AmmoList[6]].text = Ammo[6].ToString("0") + "/" + MagazineSize[6].ToString("0");
        FlamethrowerCast();
    }

    void BoomerangCast()
    {
        boomerangAim = Random.Range(0f, 360f);
        for (int i = 0; i < boomerangProjectileCount + playerStats.projectileCountBonus; i++)
        {
            Invoke("BoomerangFire", i * 0.09f);
        }
        Invoke("BoomerangCast", boomerangFireRate / playerStats.SpeedMultiplyer(1f));
    }

    void BoomerangFire()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Body.rotation + boomerangAim);
        GameObject bullet = Instantiate(BoomerangBullet, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 14.8f * Random.Range(1f, 1.03f), ForceMode2D.Impulse);

        boomerangFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
        boomerangFired.duration = boomerangDuration * playerStats.durationBonus;
        boomerangFired.damage = boomerangDamage * playerStats.DamageDealtMultiplyer(1f);
        boomerangFired.pierce = boomerangPierce;
        boomerangFired.pierceEfficiency = boomerangPierceDamage;

        if (playerStats.CritChance > Random.Range(0f, 1f))
        {
            boomerangFired.damage *= playerStats.CritDamage;
            boomerangFired.crit = true;
        }

        boomerangAim -= 15.3f;
    }

    void SingularityCast()
    {
        SingularityFire();
        Invoke("SingularityCast", singularityFireRate / playerStats.SpeedMultiplyer(1f));
    }

    void SingularityFire()
    {
        //playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation);
        GameObject bullet = Instantiate(SingularityBullet, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 11.9f * Random.Range(1f, 1.02f), ForceMode2D.Impulse);

        singularityFired = bullet.GetComponent(typeof(Bullet)) as Bullet;
        singularityFired.damage = singularityDamage * playerStats.DamageDealtMultiplyer(1f);
        singularityFired.areaSize = singularityAreaSize * playerStats.areaSizeBonus;
        singularityFired.durationValue = singularityDuration * playerStats.durationBonus;
        singularityFired.playerAreaBonus = playerStats.areaSizeBonus;
    }
}
