using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    public PlayerController playerStats;
    public bool gambler;
    public GunsLibrary Library;
    public ItemsLibrary ILibrary;
    public Transform Barrel;
    public Gun[] guns;
    public int[] Accessories;
    public Image itemImage;
    public SpriteRenderer equippedGun;
    private Bullet firedBullet;

    public bool[] slotFilled;
    public int equipped, item;
    float temp;
    int tempi, roll;

    // On Hit
    public GameObject Peacemaker, Boomerange, Wave, Orb;
    public float[] freeBulletCharges, peacemakerCharges, boomerangCharges, waveCharges, orbCharges;
    public MultipleBullets waveBullet;

    // -- items
    public bool[] Items;
    public int[] ItemList;
    public int itemsCollected;
    public GameObject DeflectProjectal;
    public GameObject[] Drones;
    public TMPro.TextMeshProUGUI Tooltip;
    //public GameObject Caltrop, Knife, Cleaver;
    //public float itemsActivationRate = 1f;

    // -- special bullets
    public GameObject Saw, Laser;
    //public float sawCharges, laserCharges;

    void Start()
    {
        if (gambler)
        {
            guns[0] = Library.baseGuns[Random.Range(0, Library.baseGuns.Length)];
            playerStats.gunImage.sprite = guns[equipped].gunSprite;
            equippedGun.sprite = guns[equipped].holdingSprite;
            guns[equipped].parts = playerStats.toolsStored;
            playerStats.DisplayAmmo();
            tempi = 3;
            while (tempi > 0)
            {
                roll = Random.Range(0, ILibrary.ItemSprite.Length);
                if (!Items[roll])
                {
                    PickUpItem(roll);
                    tempi--;
                }
            }
        }
        //Invoke("AutoReload", 3f);
        //Invoke("ThrowCaltrops", 8f);
        //Invoke("KnifeThrow", 2.85f);
        //Invoke("ThrowSaw", 3.5f);
        //Invoke("ThrowCleaver", 4f);

        /*for (int i = 0; i < 24; i++)
        {
            PickUpItem(i);
        }*/
    }

    public void PickUpItem(int which)
    {
        Items[which] = true;
        ItemList[itemsCollected] = which;
        itemsCollected++;

        ShowTooltip(which);
        switch (which)
        {
            case 3:
                playerStats.maxShield += 20;
                playerStats.GainShield(20);
                break;
            case 4:
                playerStats.dashBaseCooldown /= 1.18f;
                break;
            case 5:
                playerStats.cooldownReduction += 0.46f;
                break;
            case 6:
                playerStats.GainHP(10);
                break;
            case 7:
                playerStats.GainDMG((playerStats.maxHealth - 80) * 0.0012f);
                playerStats.GainHP(10);
                break;
            case 8:
                playerStats.GainTools(6);
                for (int i = 0; i < 3; i++)
                {
                    if (slotFilled[i])
                    {
                        guns[i].MaxSlots++;
                    }
                }
                break;
            case 9:
                playerStats.GainMS(45f);
                break;
            case 10:
                playerStats.GainDMG(0.06f);
                playerStats.GainGold(32);
                break;
            case 11:
                playerStats.maxShield += 20;
                playerStats.GainShield(20);
                break;
            case 13:
                playerStats.dashBaseCooldown /= 1.15f;
                break;
            case 14:
                playerStats.GainFR(0.072f);
                playerStats.GainKeys(2);
                break;
            case 15:
                playerStats.grenadeBaseCooldown /= 1.1f;
                break;
            case 16:
                playerStats.additionalCritChance += 0.08f;
                playerStats.luck += 2;
                playerStats.map.luck += 2;
                break;
            case 19:
                playerStats.GainDMG(0.35f);
                break;
            case 21:
                playerStats.GainDMG(0.06f);
                playerStats.forceIncrease += 0.25f;
                break;
            case 23:
                playerStats.GainFR(0.072f);
                break;
            case 25:
                playerStats.GainHP(10);
                break;
            case 26:
                playerStats.GainTools(2);
                break;
            case 27:
                playerStats.grenadeBaseCooldown /= 1.16f;
                playerStats.grenadeMaxCharges++;
                break;
            case 28:
                playerStats.grenadeMaxCharges++;
                break;
            case 29:
                // poison cloud potem
                playerStats.grenadeMaxCharges++;
                break;
            case 31:
                playerStats.GainDMG(0.06f);
                break;
            case 32:
                playerStats.cooldownReduction += (playerStats.fireRateBonus - 1f) / 3f;
                playerStats.GainFR(0.072f);
                break;
            case 33:
                Accessories[Random.Range(0, Accessories.Length)]++;
                Accessories[Random.Range(0, Accessories.Length)]++;
                playerStats.GainTools(6);
                break;
            case 34:
                playerStats.maxShield += 20;
                playerStats.GainShield(20);
                break;
        }
    }

    void ShowTooltip(int which)
    {
        //Tooltip.text = ILibrary.ItemTooltip[ItemList[which]];

        Invoke("HideTooltip", 1f);
    }

    void HideTooltip()
    {
        Tooltip.text = "";
    }

    public void OnHit(float efficiency)
    {
        //Flash();

        freeBulletCharges[equipped] += efficiency * guns[equipped].Accessories[20] * (1f + 0.2f * guns[equipped].Accessories[26]);
        if (freeBulletCharges[equipped] >= 6f)
        {
            playerStats.FireDirection(0f, 0f);
            freeBulletCharges[equipped] -= 6f;
        }

        peacemakerCharges[equipped] += efficiency * (1f + 0.2f * guns[equipped].fireRate) * guns[equipped].Accessories[23] * guns[equipped].BulletsFired() * (1f + 0.2f * guns[equipped].Accessories[26]);
        if (peacemakerCharges[equipped] >= 12f)
        {
            FirePeacemaker();
            peacemakerCharges[equipped] -= 12f;
        }

        boomerangCharges[equipped] += efficiency * (1f + 0.1f * guns[equipped].fireRate) * guns[equipped].Accessories[24] * (1f + 0.2f * guns[equipped].Accessories[26]);
        if (boomerangCharges[equipped] >= 11f)
        {
            FireBoomerang();
            boomerangCharges[equipped] -= 11f;
        }

        waveCharges[equipped] += efficiency * guns[equipped].Accessories[25] * guns[equipped].BulletsFired() * (1f + 0.2f * guns[equipped].Accessories[26]);
        if (waveCharges[equipped] >= 15f)
        {
            FireWave();
            waveCharges[equipped] -= 15f;
        }

        orbCharges[equipped] += efficiency * (1f + 0.06f * guns[equipped].fireRate) * guns[equipped].Accessories[29] * (1f + 0.2f * guns[equipped].Accessories[26]);
        if (orbCharges[equipped] >= 6f)
        {
            FireOrb();
            orbCharges[equipped] -= 6f;
        }
    }

    void FirePeacemaker()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-0.7f * guns[equipped].accuracy, 0.7f * guns[equipped].accuracy));
        GameObject bullet = Instantiate(Peacemaker, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 20f * Random.Range(0.95f, 1.05f), ForceMode2D.Impulse);

        playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        playerStats.SetBullet(1f);
        playerStats.firedBullet.damage *= guns[equipped].onHitModifier;
        playerStats.firedBullet.duration = 1.26f;

        playerStats.firedBullet.damage *= 1.2f + 0.1f * playerStats.firedBullet.pierce;
        playerStats.firedBullet.penetration += 0.12f;
        playerStats.firedBullet.pierceEfficiency += 0.15f * playerStats.firedBullet.pierce;
        playerStats.firedBullet.pierce = 5;
    }

    void FireBoomerang()
    {
        for (int i = 0; i < guns[equipped].BulletsFired(); i++)
        {
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-0.6f * guns[equipped].accuracy - guns[equipped].BulletsFired() * 1f, 0.6f * guns[equipped].accuracy + guns[equipped].BulletsFired() * 1f));
            GameObject bullet = Instantiate(Boomerange, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * 18f * Random.Range(0.96f, 1.04f), ForceMode2D.Impulse);

            playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            playerStats.SetBullet(1f);
            playerStats.firedBullet.damage *= guns[equipped].onHitModifier;
            playerStats.firedBullet.duration = 33f;

            playerStats.firedBullet.damage *= 1f + 0.06f * playerStats.firedBullet.pierce + 0.3f * playerStats.firedBullet.pierceEfficiency;
            playerStats.firedBullet.pierce += 7;
            playerStats.firedBullet.pierceEfficiency = 0.6f + 0.6f * playerStats.firedBullet.pierceEfficiency;
        }
    }

    void FireWave()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(guns[equipped].accuracy, guns[equipped].accuracy));
        GameObject bullet = Instantiate(Wave, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        playerStats.SetBullet(1f);
        waveBullet = bullet.GetComponent(typeof(MultipleBullets)) as MultipleBullets;
        if (guns[equipped].targetArea && Vector3.Distance(playerStats.transform.position, new Vector2(playerStats.mousePos[0], playerStats.mousePos[1])) <= guns[equipped].range * 24f)
            waveBullet.bulletForce = 20f * Random.Range(1.07f, 1.08f) * Vector3.Distance(playerStats.transform.position, new Vector2(playerStats.mousePos[0], playerStats.mousePos[1])) / (guns[equipped].range * 23f);
        else waveBullet.bulletForce = 20f * Random.Range(0.95f, 1.05f);
        waveBullet.BulletShard = guns[equipped].bulletPrefab[Random.Range(0, guns[equipped].bulletPrefab.Length)];
    }

    void FireOrb()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-0.7f * guns[equipped].accuracy, 0.7f * guns[equipped].accuracy));
        GameObject bullet = Instantiate(Orb, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 18f * Random.Range(0.95f, 1.05f), ForceMode2D.Impulse);

        playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        playerStats.SetBullet(1f);
        playerStats.firedBullet.damage *= guns[equipped].onHitModifier;
        playerStats.firedBullet.duration = 1.41f;

        playerStats.firedBullet.damage *= 0.16f;
        playerStats.firedBullet.DoT *= 3.8f; playerStats.firedBullet.DoT += 1.6f;
        playerStats.firedBullet.curse *= 8.8f; playerStats.firedBullet.curse += 5.4f;
    }

    public void Deflect(float damage)
    {
        for (int i = 0; i < 3; i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Random.Range(0f, 80f) + 120f * i);
            GameObject bullet = Instantiate(DeflectProjectal, Barrel.position, Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrel.up * Random.Range(16f, 18.2f), ForceMode2D.Impulse);

            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            firedBullet.damage = damage * (playerStats.DamageDealtMultiplyer(1.2f) - 0.16f);
        }
    }

    /*void ThrowCaltrops()
    {
        if (Items[0] > 0 && !playerStats.day)
        {
            for (int i = 0; i < 5 * Items[0]; i++)
            {
                Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, playerStats.Gun.rotation + (Random.Range(-4f, 4f) + i * 72f) / Items[0]);
                GameObject caltrop = Instantiate(Caltrop, Barrel.position, Barrel.rotation);
                Rigidbody2D caltrop_body = caltrop.GetComponent<Rigidbody2D>();
                caltrop_body.AddForce(Barrel.up * Random.Range(2.45f, 2.79f), ForceMode2D.Impulse);

                firedBullet = caltrop.GetComponent(typeof(Bullet)) as Bullet;
                firedBullet.damage *= playerStats.DamageDealtMultiplyer(1f);
            }
        }

        Invoke("ThrowCaltrops", 8f / itemsActivationRate);
    }

    void KnifeThrow()
    {
        if (Items[1] > 0 && !playerStats.day)
        {
            for (int i = 0; i < 2 * Items[1]; i++)
            {
                Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, playerStats.Gun.rotation - 6f - 3f * Items[1] + (3f + 6f / Items[1]) * i);
                GameObject knife = Instantiate(Knife, Barrel.position, Barrel.rotation);
                Rigidbody2D knife_body = knife.GetComponent<Rigidbody2D>();
                knife_body.AddForce(Barrel.up * Random.Range(17.5f, 18.9f), ForceMode2D.Impulse);

                firedBullet = knife.GetComponent(typeof(Bullet)) as Bullet;
                firedBullet.damage *= playerStats.DamageDealtMultiplyer(1f);
            }
        }

        temp = (2.85f / (1f + playerStats.SpeedMultiplyer(0.5f))) / itemsActivationRate;
        Invoke("KnifeThrow", temp);
    }

    void ThrowSaw()
    {
        if (Items[2] > 0 && !playerStats.day)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(0f, 360f));
            GameObject saw = Instantiate(Saw, Barrel.position, Barrel.rotation);
            Rigidbody2D saw_body = saw.GetComponent<Rigidbody2D>();
            saw_body.AddForce(Barrel.up * Random.Range(17.3f, 18.65f), ForceMode2D.Impulse);

            firedBullet = saw.GetComponent(typeof(Bullet)) as Bullet;
            firedBullet.damage *= playerStats.DamageDealtMultiplyer(1.12f);
        }

        temp = 3.5f / itemsActivationRate;

        temp /= 0.1f + 0.9f * Items[2];

        Invoke("ThrowSaw", temp);
    }

    void ThrowCleaver()
    {
        if (Items[10] > 0 && !playerStats.day)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-(12 + 2 * Items[10]), (12 + 2 * Items[10])));
            GameObject cleaver = Instantiate(Cleaver, Barrel.position, Barrel.rotation);
            Rigidbody2D cleaver_body = cleaver.GetComponent<Rigidbody2D>();
            cleaver_body.AddForce(Barrel.up * Random.Range(16.25f, 17.55f), ForceMode2D.Impulse);

            firedBullet = cleaver.GetComponent(typeof(Bullet)) as Bullet;
            firedBullet.damage = 40 + 0.05f * playerStats.maxHealth + 9 * Items[10];
            firedBullet.damage *= playerStats.DamageDealtMultiplyer(1f);
        }

        temp = (4f / (1f + playerStats.SpeedMultiplyer(0.6f))) / itemsActivationRate;

        temp /= 0.25f + 0.75f * Items[10];

        Invoke("ThrowCleaver", temp);
    }

    void AutoReload()
    {
        if (guns[equipped].bulletsLeft < guns[equipped].magazineSize)
        {
            guns[equipped].bulletsLeft += guns[equipped].Accessories[4 + playerStats.accessoriesPerType * 2];
            playerStats.DisplayAmmo();

            if (guns[equipped].individualReload)
            {
                Invoke("AutoReload", (0.75f + 10f * guns[equipped].reloadTime));
            }
            else Invoke("AutoReload", (0.75f + 10f * guns[equipped].reloadTime / (2 + guns[equipped].magazineSize)));
        }
        else Invoke("AutoReload", 2f);
    }

    public void SpecialCharges()
    {
        sawCharges += guns[equipped].Accessories[4] * guns[equipped].BulletsFired() * (1f + 0.15f * guns[equipped].Accessories[4 + playerStats.accessoriesPerType * 3]);
        if (sawCharges >= 12f)
        {
            FireSaw();
            sawCharges -= 12f;
        }

        laserCharges += guns[equipped].Accessories[4 + playerStats.accessoriesPerType] * (1f + 0.15f * guns[equipped].Accessories[4 + playerStats.accessoriesPerType * 3]);
        if (laserCharges >= 7f)
        {
            FireLaser();
            laserCharges -= 7f;
        }
    }

    public void FireSaw()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-(3f + 1.1f * guns[equipped].accuracy), (3f + 1.1f * guns[equipped].accuracy)));
        GameObject bullet = Instantiate(Saw, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * guns[equipped].force * 1.19f * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);

        playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        playerStats.SetBullet(1f);

        playerStats.firedBullet.damage *= 1.2f;
        playerStats.firedBullet.penetration += 0.12f;
        playerStats.firedBullet.DoT += 0.25f + 0.06f * playerStats.firedBullet.DoT;
        playerStats.firedBullet.pierce += 3;
        playerStats.firedBullet.pierceEfficiency *= 0.6f; playerStats.firedBullet.pierceEfficiency += 0.6f;
    }

    public void FireLaser()
    {
        for (int i = 0; i < guns[equipped].BulletsFired(); i++)
        {
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-(1f + 0.7f * guns[equipped].accuracy), (1f + .7f * guns[equipped].accuracy)));
            GameObject bullet = Instantiate(Laser, playerStats.Barrel.position, playerStats.Barrel.rotation);

            playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            playerStats.SetBullet(1f);

            playerStats.firedBullet.damage *= 0.44f;
            playerStats.firedBullet.penetration *= 0.8f; playerStats.firedBullet.penetration += 0.05f;
            playerStats.firedBullet.vulnerableApplied += 0.05f + 0.15f * guns[equipped].fireRate;
            playerStats.firedBullet.pierce = 50;
            playerStats.firedBullet.pierceEfficiency = 1f;
        }
    }*/
}
