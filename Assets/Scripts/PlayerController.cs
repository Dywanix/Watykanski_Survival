using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform Barrel, Hand;
    public Rigidbody2D Body, Gun;
    public GameObject reload_image;
    public Equipment eq;
    public TMPro.TextMeshProUGUI magazineInfo, ammoInfo, scrapInfo;
    private Bullet firedBullet;
    public Gunslinger gunslinger;

    public float xInput = 0, yInput = 0;
    public bool mouseLeft, reloading, free = true;
    Vector3 mousePos, mouseVector;
    CameraController Cam;
    public float task;

    // -- statystyki --
    public float maxHealth, damageBonus, fireRateBonus, movementSpeed = 7, dashCooldown, dash, scrap;
    public int level, experience, experienceNeeded;
    public float healthIncrease, damageIncrease, fireRateIncrease, movementSpeedIncrease, additionalCritChance;

    void Start()
    {
        GetMouseInput();
        Cam = FindObjectOfType<CameraController>();
        DisplayAmmo();
        experienceNeeded = 300 + level * 100 + level * level * 3;
    }

    void Update()
    {
        if (free)
        {
            GetInput();
            Movement();
            Aim();
            if (task <= 0)
            {
                Action();
            }
            else
            {
                task -= Time.deltaTime * fireRateBonus;
                if (mouseLeft && reloading && eq.guns[eq.equipped].bulletsLeft > 0)
                {
                    reloading = false;
                    reload_image.SetActive(false);
                    task = 0.12f;
                    Fire(0f);
                    task += eq.guns[eq.equipped].fireRate;
                }
            }
        }
        if (dashCooldown > 0)
            dashCooldown -= Time.deltaTime;
    }

    void GetInput()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
        GetMouseInput();
    }

    void GetMouseInput()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        mouseVector = (mousePos - Hand.position).normalized;
        mouseLeft = Input.GetMouseButton(0);
    }

    void Movement()
    {
        // TUTAJ !!
        Vector3 tempPos = transform.position;
        tempPos += new Vector3(xInput, yInput, 0) * (movementSpeed + dash) * Time.deltaTime;
        transform.position = tempPos;
        if (dash > 0)
            dash -= (30 + 5 * dash) * Time.deltaTime;
    }

    void Aim()
    {
        float gunAngle = Mathf.Atan2(mouseVector.y, mouseVector.x) * Mathf.Rad2Deg;
        Gun.rotation = gunAngle - 90f;
        /*gunRend.sortingOrder = playerSortingOrder - 1; //put the gun sprite bellow the player sprite
        if (gunAngle > 0)
        { //put the gun on top of player if it's at the correct angle
            gunRend.sortingOrder = playerSortingOrder + 1;
        }*/
    }

    void Action()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Dash();

        if (reloading)
        {
            Reloaded();
        }

        if (mouseLeft)
        {
            if (eq.guns[eq.equipped].bulletsLeft > 0 || eq.guns[eq.equipped].infiniteMagazine)
            {
                Fire(0f);
                task += eq.guns[eq.equipped].fireRate;
            }
            else Reload();
        }
        else if (Input.GetKeyDown(KeyCode.R))
            Reload();
        else if (Input.GetKeyDown(KeyCode.Alpha1))
            SwapGun(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SwapGun(1);
    }

    public void Fire(float accuracy_change)
    {
        if (gunslinger == true)
        {
            if (Random.Range(0f,1f) <= gunslinger.doubleShotChance)
            {
                for (int i = 0; i < eq.guns[eq.equipped].bulletSpread; i++)
                {
                    Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + Random.Range(-eq.guns[eq.equipped].accuracy - accuracy_change, eq.guns[eq.equipped].accuracy + accuracy_change));
                    GameObject bullet = Instantiate(eq.guns[eq.equipped].bulletPrefab, Barrel.position, Barrel.rotation);
                    Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
                    bullet_body.AddForce(Barrel.up * eq.guns[eq.equipped].force * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
                    firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
                    firedBullet.damage = eq.guns[eq.equipped].damage * DamageDealtMultiplyer(); firedBullet.penetration = eq.guns[eq.equipped].penetration;
                    if (eq.guns[eq.equipped].critChance + additionalCritChance >= Random.Range(0f, 1f))
                    {
                        firedBullet.damage *= eq.guns[eq.equipped].critDamage;
                        firedBullet.crit = true;
                    }
                }
            }
        }
        for (int i = 0; i < eq.guns[eq.equipped].bulletSpread; i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + Random.Range(-eq.guns[eq.equipped].accuracy - accuracy_change, eq.guns[eq.equipped].accuracy + accuracy_change));
            GameObject bullet = Instantiate(eq.guns[eq.equipped].bulletPrefab, Barrel.position, Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrel.up * eq.guns[eq.equipped].force * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            firedBullet.damage = eq.guns[eq.equipped].damage * DamageDealtMultiplyer(); firedBullet.penetration = eq.guns[eq.equipped].penetration;
            if (eq.guns[eq.equipped].critChance + additionalCritChance >= Random.Range(0f, 1f))
            {
                firedBullet.damage *= eq.guns[eq.equipped].critDamage;
                firedBullet.crit = true;
            }
        }
        Cam.Shake((transform.position - Barrel.position).normalized, eq.guns[eq.equipped].cameraShake, eq.guns[eq.equipped].shakeDuration);
        if (!eq.guns[eq.equipped].infiniteMagazine)
        {
            eq.guns[eq.equipped].bulletsLeft--;
            DisplayAmmo();
        }
    }

    void Reload()
    {
        if (!eq.guns[eq.equipped].infiniteMagazine && eq.guns[eq.equipped].bulletsLeft < eq.guns[eq.equipped].magazineSize)
        {
            if (eq.guns[eq.equipped].infiniteAmmo || eq.guns[eq.equipped].ammo > 0)
            {
                reloading = true;
                reload_image.SetActive(true);
                task = eq.guns[eq.equipped].reloadTime;
            }
        }
    }

    void Reloaded()
    {
        if (eq.guns[eq.equipped].individualReload)
        {
            if (eq.guns[eq.equipped].infiniteAmmo)
            {
                eq.guns[eq.equipped].bulletsLeft++;
                if (eq.guns[eq.equipped].bulletsLeft >= eq.guns[eq.equipped].magazineSize)
                {
                    reloading = false;
                    reload_image.SetActive(false);
                }
                else
                {
                    task = eq.guns[eq.equipped].reloadTime;
                }
            }
            else
            {
                eq.guns[eq.equipped].bulletsLeft++;
                eq.guns[eq.equipped].ammo--;
                if (eq.guns[eq.equipped].bulletsLeft >= eq.guns[eq.equipped].magazineSize || eq.guns[eq.equipped].ammo <= 0)
                {
                    reloading = false;
                    reload_image.SetActive(false);
                }
                else
                {
                    task = eq.guns[eq.equipped].reloadTime;
                }
            }
        }
        else
        {
            if (eq.guns[eq.equipped].infiniteAmmo)
            {
                eq.guns[eq.equipped].bulletsLeft = eq.guns[eq.equipped].magazineSize;
            }
            else if (eq.guns[eq.equipped].ammo >= eq.guns[eq.equipped].magazineSize - eq.guns[eq.equipped].bulletsLeft)
            {
                eq.guns[eq.equipped].ammo -= (eq.guns[eq.equipped].magazineSize - eq.guns[eq.equipped].bulletsLeft);
                eq.guns[eq.equipped].bulletsLeft = eq.guns[eq.equipped].magazineSize;
            }
            else
            {
                eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].ammo;
                eq.guns[eq.equipped].ammo = 0;
            }
            reloading = false;
            reload_image.SetActive(false);
        }
        DisplayAmmo();
    }

    void DisplayAmmo()
    {
        magazineInfo.text = (eq.guns[eq.equipped].bulletsLeft).ToString("") + "/" + eq.guns[eq.equipped].magazineSize;
        if (eq.guns[eq.equipped].infiniteAmmo)
            ammoInfo.text = "NaN";
        else ammoInfo.text = (eq.guns[eq.equipped].ammo).ToString("");
    }

    void SwapGun(int which)
    {
        if (which != eq.equipped)
        {
            eq.gunSprite[eq.equipped].SetActive(false);
            eq.equipped = which;
            eq.gunSprite[eq.equipped].SetActive(true);
            DisplayAmmo();
            task = 1.25f;
        }
    }

    void Dash()
    {
        if (dashCooldown <= 0)
        {
            dash = 36f + movementSpeed * 0.5f;
            dashCooldown = 8f;
        }
    }

    public void TakeDamage(float value)
    {
        //bla bla
    }

    float DamageDealtMultiplyer()
    {
        return damageBonus;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Scrap")
        {
            GainScrap(1);
            Destroy(other.gameObject);
        }
    }

    void GainScrap(int amount)
    {
        scrap += amount;
        scrapInfo.text = scrap.ToString("0");
    }

    public void SpendScrap(int amount)
    {
        scrap -= amount;
        scrapInfo.text = scrap.ToString("0");
    }
}
