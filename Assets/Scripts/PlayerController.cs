using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Transform Barrel, Hand;
    public Rigidbody2D Body, Gun;
    public GameObject reload_image;
    public Equipment eq;
    public TMPro.TextMeshProUGUI magazineInfo, ammoInfo, scrapInfo;
    public Image healthBar;
    private Bullet firedBullet;
    private EnemyBullet collidedBullet;

    public Gunslinger gunslinger;
    public Berserker berserker;
    public SteamGolem steamGolem;

    public float xInput = 0, yInput = 0;
    public bool mouseLeft, reloading, free = true, day = true;
    Vector3 mousePos, mouseVector;
    CameraController Cam;
    public float task;

    // -- statystyki --
    public float maxHealth, health, poison, damageBonus, fireRateBonus, movementSpeed = 7, dashCooldown, dash;
    public int level = 1, scrap;
    public float healthIncrease, damageIncrease, fireRateIncrease, movementSpeedIncrease, additionalCritChance;

    void Start()
    {
        GetMouseInput();
        Cam = FindObjectOfType<CameraController>();
        DisplayAmmo();
        health = maxHealth;
        healthBar.fillAmount = health / maxHealth;
        Invoke("Tick", 0.8f);
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
                task -= Time.deltaTime * SpeedMultiplyer(1f);
                if (mouseLeft && reloading && eq.guns[eq.equipped].bulletsLeft > 0)
                {
                    reloading = false;
                    reload_image.SetActive(false);
                    task = 0.1f;
                    Shoot(0f);
                    task += eq.guns[eq.equipped].fireRate;
                }
            }
        }
        if (dashCooldown > 0)
            dashCooldown -= Time.deltaTime;
    }

    void Tick()
    {
        RestoreHealth(maxHealth * 0.0025f);

        if (berserker == true)
        {
            RestoreHealth((maxHealth - health) * 0.007f);
            berserker.GainCharge(1f + 0.05f * level);
            if (poison > 0)
                poison -= 0.06f;
        }

        if (poison > 0)
            poison -= 0.2f;

        Invoke("Tick", 1f);
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
                Shoot(0f);
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
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SwapGun(2);
    }

    public void Shoot(float accuracy_change)
    {
        if (gunslinger == true)
        {
            if (Random.Range(0f,1f) <= gunslinger.doubleShotChance)
                Fire(accuracy_change);
        }

        Fire(accuracy_change);

        Cam.Shake((transform.position - Barrel.position).normalized, eq.guns[eq.equipped].cameraShake, eq.guns[eq.equipped].shakeDuration);
        if (!eq.guns[eq.equipped].infiniteMagazine)
        {
            eq.guns[eq.equipped].bulletsLeft--;
            DisplayAmmo();
        }
    }

    void Fire(float accuracy_change)
    {
        for (int i = 0; i < eq.guns[eq.equipped].bulletSpread; i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + Random.Range(-eq.guns[eq.equipped].accuracy - accuracy_change, eq.guns[eq.equipped].accuracy + accuracy_change));
            GameObject bullet = Instantiate(eq.guns[eq.equipped].bulletPrefab, Barrel.position, Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrel.up * eq.guns[eq.equipped].force * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet();
        }
    }

    public void FireDirection(float direction)
    {
        for (int i = 0; i < eq.guns[eq.equipped].bulletSpread; i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + direction);
            GameObject bullet = Instantiate(eq.guns[eq.equipped].bulletPrefab, Barrel.position, Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrel.up * eq.guns[eq.equipped].force * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet();
        }
    }

    void SetBullet()
    {
        firedBullet.duration = eq.guns[eq.equipped].range;
        firedBullet.damage = eq.guns[eq.equipped].damage * DamageDealtMultiplyer(1f);
        firedBullet.DoT = eq.guns[eq.equipped].DoT;
        firedBullet.penetration = eq.guns[eq.equipped].penetration;
        firedBullet.armorShred = eq.guns[eq.equipped].armorShred;
        firedBullet.vulnerableApplied = eq.guns[eq.equipped].vulnerableApplied;
        firedBullet.slowDuration = eq.guns[eq.equipped].slowDuration;
        firedBullet.stunChance = eq.guns[eq.equipped].stunChance;
        firedBullet.stunDuration = eq.guns[eq.equipped].stunDuration;
        firedBullet.pierce = eq.guns[eq.equipped].pierce;
        firedBullet.pierceDamage = eq.guns[eq.equipped].pierceDamage;
        if (eq.guns[eq.equipped].critChance + additionalCritChance >= Random.Range(0f, 1f))
        {
            firedBullet.damage *= eq.guns[eq.equipped].critDamage;
            firedBullet.armorShred *= 0.6f + eq.guns[eq.equipped].critDamage * 0.4f;
            firedBullet.vulnerableApplied *= 0.6f + eq.guns[eq.equipped].critDamage * 0.4f;
            firedBullet.stunChance *= 0.4f + eq.guns[eq.equipped].critDamage * 0.6f;
            firedBullet.stunDuration *= 0.7f + eq.guns[eq.equipped].stunDuration * 0.3f;
            firedBullet.crit = true;
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
                eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].overload;
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
                eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].overload;
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
            eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].overload;
            reloading = false;
            reload_image.SetActive(false);
        }
        DisplayAmmo();
    }

    public void DisplayAmmo()
    {
        if (!eq.guns[eq.equipped].infiniteMagazine)
        {
            magazineInfo.text = (eq.guns[eq.equipped].bulletsLeft).ToString("") + "/" + eq.guns[eq.equipped].magazineSize;
            if (eq.guns[eq.equipped].infiniteAmmo)
                ammoInfo.text = "NaN";
            else ammoInfo.text = (eq.guns[eq.equipped].ammo).ToString("");
        }
        else
        {
            magazineInfo.text = "";
            ammoInfo.text = "";
        }
    }

    void SwapGun(int which)
    {
        if (which != eq.equipped)
        {
            eq.gunSprite[eq.equipped].SetActive(false);
            eq.equipped = which;
            eq.gunSprite[eq.equipped].SetActive(true);
            DisplayAmmo();
            task = 0.775f;
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
        health -= value;
        healthBar.fillAmount = health / maxHealth;

        if (berserker == true)
            berserker.GainCharge(0.28f * value);

        if (health < 0f)
            Application.Quit();
    }

    public void GainPoison(float value)
    {
        poison += value;

        if (poison >= value * 3f)
            TakeDamage(value * 3f);
        else TakeDamage(poison);
    }

    public void RestoreHealth(float value)
    {
        health += value;
        if (health > maxHealth)
            health = maxHealth;
        healthBar.fillAmount = health / maxHealth;
    }

    public float SpeedMultiplyer(float efficiency)
    {
        return 1f + (fireRateBonus - 1f) * efficiency;
    }

    public float DamageDealtMultiplyer(float efficiency)
    {
        return 1f + (damageBonus - 1f) * efficiency;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Scrap")
        {
            GainScrap(1);
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "Scrap5")
        {
            GainScrap(5);
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "Ammo")
        {
            PickedUpAmmo();
            Destroy(other.gameObject);
            DisplayAmmo();
        }
        else if (other.transform.tag == "Tools")
        {
            for (int i = 0; i < eq.guns.Length; i++)
            {
                eq.guns[i].GainSpecialCharge(0.2f);
            }
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "EnemyProjectal")
        {
            collidedBullet = other.GetComponent(typeof(EnemyBullet)) as EnemyBullet;
            TakeDamage(collidedBullet.damage);
            GainPoison(collidedBullet.poison);
            Destroy(other.gameObject);
        }
    }

    public void LevelUp()
    {
        level++;
        maxHealth += healthIncrease;
        damageBonus += damageIncrease;
        fireRateBonus += fireRateIncrease;
        movementSpeed += movementSpeedIncrease;
    }

    void GainScrap(int amount)
    {
        scrap += amount;
        scrapInfo.text = scrap.ToString("0");

        if (steamGolem == true)
            steamGolem.ClockworkMachine(amount);
    }

    public void SpendScrap(int amount)
    {
        scrap -= amount;
        scrapInfo.text = scrap.ToString("0");
    }

    void PickedUpAmmo()
    {
        for (int i = 0; i < eq.guns.Length; i++)
        {
            eq.guns[i].AmmoPicked();
        }
    }
}
