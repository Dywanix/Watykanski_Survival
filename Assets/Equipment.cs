using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    public PlayerController playerStats;
    public Gun[] guns;
    public GameObject Caltrop, Turret;
    public GameObject[] gunSprite;
    public int[] Items, MaxItems, Accessories;
    public Image itemImage;
    public SpriteRenderer equippedGun;
    public Sprite[] itemSprites;

    public int equipped, item;

    // -- special bullets
    public GameObject Saw, Laser;
    public float sawCharges, laserCharges;

    void Start()
    {
        Invoke("AutoReload", 3f);
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
        else Invoke("AutoReload", 0.75f);
    }

    public void SpecialCharges()
    {
        sawCharges += guns[equipped].Accessories[4] * (1f + 0.15f * guns[equipped].Accessories[4 + playerStats.accessoriesPerType * 3]);
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
        for (int i = 0; i < guns[equipped].bulletSpread; i++)
        {
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-(3f + 1.1f * guns[equipped].accuracy), (3f + 1.1f * guns[equipped].accuracy)));
            GameObject bullet = Instantiate(Saw, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * guns[equipped].force * 1.22f * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);

            playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            playerStats.SetBullet();

            playerStats.firedBullet.damage *= 1.2f;
            playerStats.firedBullet.penetration += 0.12f;
            playerStats.firedBullet.DoT += 0.25f;
            playerStats.firedBullet.pierce += 3;
            playerStats.firedBullet.pierceEfficiency *= 0.6f; playerStats.firedBullet.pierceEfficiency += 0.6f;
        }
    }

    public void FireLaser()
    {
        for (int i = 0; i < guns[equipped].bulletSpread; i++)
        {
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-(1f + 0.7f * guns[equipped].accuracy), (1f + .7f * guns[equipped].accuracy)));
            GameObject bullet = Instantiate(Laser, playerStats.Barrel.position, playerStats.Barrel.rotation);

            playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            playerStats.SetBullet();

            playerStats.firedBullet.damage *= 0.4f;
            playerStats.firedBullet.penetration *= 0.8f; playerStats.firedBullet.penetration += 0.05f;
            playerStats.firedBullet.vulnerableApplied += 0.05f + 0.15f * guns[equipped].fireRate;
            playerStats.firedBullet.pierce = 50;
            playerStats.firedBullet.pierceEfficiency = 1f;
        }
    }
}
