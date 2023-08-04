using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Transform Barrel, Hand, Dude, GunRot;
    public Rigidbody2D Body, Gun;
    public Equipment eq;
    public TMPro.TextMeshProUGUI magazineInfo, ammoInfo, scrapInfo, toolsInfo, tokensInfo;
    public Image healthBar, shieldBar, taskImage, dashImage;
    public Bullet firedBullet;
    private EnemyBullet collidedBullet;

    public Gunslinger gunslinger;
    public Berserker berserker;
    public SteamGolem steamGolem;

    public float xInput = 0, yInput = 0;
    public bool mouseLeft, reloading, free = true, day = true;
    Vector3 mousePos, mouseVector;
    CameraController Cam;
    public float task, taskMax;

    // -- statystyki --
    public float maxHealth, health, maxShield, shield, shieldChargeRate, shieldChargeDelay, rechargeTimer, poison, damageBonus, fireRateBonus, movementSpeed = 7, cooldownReduction = 1, maxDashCooldown, dashCooldown, dash;
    public int level = 1, dayCount = 1, accessoriesPerType;
    public float healthIncrease, damageIncrease, fireRateIncrease, movementSpeedIncrease, additionalCritChance;
    int tempi;
    float temp;

    // -- zasoby --
    public int tools, tokens;
    public float scrap;

    // -- animacje --
    public Animator animator;
    public float moveSpeed = 5f;

    void Start()
    {
        GetMouseInput();
        Cam = FindObjectOfType<CameraController>();
        DisplayAmmo();
        health = maxHealth;
        healthBar.fillAmount = health / maxHealth;
        shieldBar.fillAmount = shield / maxShield;
        Invoke("Tick", 0.8f);
    }

    void Update()
    {
        if (free)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Dash();

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
                taskImage.fillAmount = 1 - (task / taskMax);
                if (mouseLeft && reloading && eq.guns[eq.equipped].bulletsLeft > 0)
                {
                    reloading = false;
                    NewTask(0.1f);
                    Shoot(0f);
                    task += eq.guns[eq.equipped].fireRate;
                }
            }
        }

        if (dashCooldown > 0)
        {
            dashCooldown -= Time.deltaTime;
            dashImage.fillAmount = 1 - (dashCooldown / maxDashCooldown);
        }

        if (shield < maxShield)
        {
            if (rechargeTimer > 0)
                rechargeTimer -= Time.deltaTime;
            else
                GainShield(shieldChargeRate * Time.deltaTime);
        }
    }

    public void NewTask(float duration)
    {
        taskMax = duration;
        task += duration;
    }

    void Tick()
    {
        RestoreHealth(maxHealth * 0.0025f);
        RestoreHealth(maxHealth * 0.001f * eq.Items[5]);

        if (berserker == true)
            RestoreHealth((maxHealth * 2f - health) * 0.003f);

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
        if(Input.GetAxis("Horizontal") != 0)
        {
            animator.SetFloat("moveSpeed", Mathf.Abs(Input.GetAxis("Horizontal")));
        }
        else if(Input.GetAxis("Vertical") != 0)
        {
            animator.SetFloat("moveSpeed", Mathf.Abs(Input.GetAxis("Vertical")));
        }
        else
        {
            animator.SetFloat("moveSpeed", 0f);
        }
        
        if(Input.GetAxis("Horizontal") > 0.01f)
        {
            Dude.rotation = new Quaternion(0, 0, 0, 0);
        }
        else if (Input.GetAxis("Horizontal") < -0.01f)
        {
            Dude.rotation = new Quaternion(0, 180, 0, 0);
        }
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
        GunRot.localScale = new Vector3(0.5f, 0.5f, 1f);
        if (Gun.rotation > 0f || Gun.rotation < -180f)
            GunRot.localScale = new Vector3(-0.5f, 0.5f, 1f);
    }

    void Action()
    {
        if (reloading)
        {
            Reloaded();
        }

        if (mouseLeft)
        {
            if (eq.guns[eq.equipped].bulletsLeft > 0 || eq.guns[eq.equipped].infiniteMagazine)
            {
                Shoot(0f);
                NewTask(eq.guns[eq.equipped].fireRate);
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
        if (gunslinger)
        {
            if (Random.Range(0f, 1f) <= gunslinger.doubleShotChance + gunslinger.chanceBonus)
            {
                Fire(accuracy_change);
                gunslinger.chanceBonus = 0f;
                gunslinger.DisplayChance();
            }
            else
            {
                gunslinger.chanceBonus += 0.012f;
                gunslinger.DisplayChance();
            }
        }

        Fire(accuracy_change);

        Cam.Shake((transform.position - Barrel.position).normalized, eq.guns[eq.equipped].cameraShake, eq.guns[eq.equipped].shakeDuration);
        if (!eq.guns[eq.equipped].infiniteMagazine)
        {
            if (eq.guns[eq.equipped].Accessories[3 + accessoriesPerType * 2] * 0.25f < Random.Range(0f, 1f))
                eq.guns[eq.equipped].bulletsLeft--;
            DisplayAmmo();
        }
    }

    void Fire(float accuracy_change)
    {
        for (int i = 0; i < eq.guns[eq.equipped].BulletsFired(); i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + Random.Range(-eq.guns[eq.equipped].accuracy - accuracy_change, eq.guns[eq.equipped].accuracy + accuracy_change));
            GameObject bullet = Instantiate(eq.guns[eq.equipped].bulletPrefab[Random.Range(0, eq.guns[eq.equipped].bulletPrefab.Length)], Barrel.position, Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrel.up * eq.guns[eq.equipped].force * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet();
            eq.Flash();
        }

        eq.SpecialCharges();
    }

    public void FireDirection(float direction)
    {
        for (int i = 0; i < eq.guns[eq.equipped].BulletsFired(); i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + direction);
            GameObject bullet = Instantiate(eq.guns[eq.equipped].bulletPrefab[Random.Range(0, eq.guns[eq.equipped].bulletPrefab.Length)], Barrel.position, Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrel.up * eq.guns[eq.equipped].force * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet();
        }
    }

    public void SetBullet()
    {
        firedBullet.duration = eq.guns[eq.equipped].range;
        firedBullet.damage = eq.guns[eq.equipped].damage * DamageDealtMultiplyer(1f);
        firedBullet.DoT = eq.guns[eq.equipped].DoT;
        firedBullet.penetration = eq.guns[eq.equipped].penetration;
        firedBullet.armorShred = eq.guns[eq.equipped].armorShred;
        if (eq.guns[eq.equipped].Accessories[3] > 0)
        {
            temp = 0.07f * eq.guns[eq.equipped].fireRate / (0.2f + 0.8f * eq.guns[eq.equipped].BulletsFired());
            firedBullet.armorShred += temp * eq.guns[eq.equipped].Accessories[3];
        }
        firedBullet.vulnerableApplied = eq.guns[eq.equipped].vulnerableApplied;
        if (eq.guns[eq.equipped].Accessories[1 + accessoriesPerType] > 0)
        {
            temp = 0.045f * eq.guns[eq.equipped].fireRate / (0.2f + 0.8f * eq.guns[eq.equipped].BulletsFired());
            firedBullet.vulnerableApplied += temp * eq.guns[eq.equipped].Accessories[1 + accessoriesPerType];
        }
        firedBullet.slowDuration = eq.guns[eq.equipped].slowDuration;
        firedBullet.stunChance = eq.guns[eq.equipped].stunChance;
        firedBullet.stunDuration = eq.guns[eq.equipped].stunDuration;
        firedBullet.pierce = eq.guns[eq.equipped].pierce;
        firedBullet.pierceEfficiency = eq.guns[eq.equipped].pierceEfficiency;
        if (eq.guns[eq.equipped].Accessories[3 + accessoriesPerType] > 0)
        {
            temp = 0.06f + 0.12f / (1f * eq.guns[eq.equipped].pierce);
            firedBullet.pierceEfficiency += temp * eq.guns[eq.equipped].Accessories[3 + accessoriesPerType];
        }

        if (eq.guns[eq.equipped].critChance + additionalCritChance >= Random.Range(0f, 1f))
        {
            firedBullet.damage *= eq.guns[eq.equipped].critDamage;
            firedBullet.armorShred *= 0.6f + eq.guns[eq.equipped].critDamage * 0.4f;
            firedBullet.vulnerableApplied *= 0.6f + eq.guns[eq.equipped].critDamage * 0.4f;
            firedBullet.slowDuration *= 0.7f + eq.guns[eq.equipped].critDamage * 0.3f;
            firedBullet.slowDuration += 0.05f + 0.01f * firedBullet.damage;
            firedBullet.stunChance *= 0.4f + eq.guns[eq.equipped].critDamage * 0.6f;
            firedBullet.stunDuration *= 0.7f + eq.guns[eq.equipped].critDamage * 0.3f;
            firedBullet.pierceEfficiency *= 1.1f;
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
                NewTask(eq.guns[eq.equipped].reloadTime);
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
                    reloading = false;
                else
                {
                    NewTask(eq.guns[eq.equipped].reloadTime);
                }
            }
            else
            {
                eq.guns[eq.equipped].bulletsLeft++;
                eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].overload;
                eq.guns[eq.equipped].ammo--;
                if (eq.guns[eq.equipped].bulletsLeft >= eq.guns[eq.equipped].magazineSize || eq.guns[eq.equipped].ammo <= 0)
                    reloading = false;
                else
                {
                    NewTask(eq.guns[eq.equipped].reloadTime);
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
        }
        DisplayAmmo();
    }

    public void DisplayAmmo()
    {
        magazineInfo.text = (eq.guns[eq.equipped].bulletsLeft).ToString("") + "/" + eq.guns[eq.equipped].magazineSize;
        ammoInfo.text = (eq.guns[eq.equipped].ammo).ToString("");
    }

    public void SwapGun(int which)
    {
        if (which != eq.equipped && eq.slotFilled[which])
        {
            eq.gunSprite[eq.equipped].SetActive(false);
            eq.equipped = which;
            eq.gunSprite[eq.equipped].SetActive(true);
            eq.equippedGun.sprite = eq.guns[eq.equipped].gunSprite;
            DisplayAmmo();
            NewTask(0.775f);
        }
    }

    public void PickUpGun(int which)
    {
        if (!eq.slotFilled[1])
        {
            eq.guns[1] = eq.Library.guns[which];
            eq.slotFilled[1] = true;
        }
        else if (!eq.slotFilled[2])
        {
            eq.guns[2] = eq.Library.guns[which];
            eq.slotFilled[2] = true;
        }
    }

    void Dash()
    {
        if (dashCooldown <= 0)
        {
            dash = 36f + movementSpeed * 0.5f;

            maxDashCooldown = 8f / cooldownReduction;
            dashCooldown = maxDashCooldown;

            tempi = eq.guns[eq.equipped].magazineSize * eq.Items[7] / 5;
            if (eq.guns[eq.equipped].bulletsLeft > eq.guns[eq.equipped].magazineSize)
            {
                // nothing
            }
            else if (eq.guns[eq.equipped].magazineSize - eq.guns[eq.equipped].bulletsLeft < tempi)
            {
                eq.guns[eq.equipped].bulletsLeft = eq.guns[eq.equipped].magazineSize;
            }
            else
            {
                eq.guns[eq.equipped].bulletsLeft += tempi;
            }
            DisplayAmmo();
        }
    }

    public void TakeDamage(float value, bool pierce)
    {
        if (pierce)
        {
            health -= value;
            healthBar.fillAmount = health / maxHealth;

            if (berserker == true && !day)
                berserker.GainWrath(value);
        }
        else
        {
            shield -= value;
            rechargeTimer = shieldChargeDelay;

            if (shield < 0)
            {
                value = shield * (-1f);
                shield = 0;
                health -= value;
                healthBar.fillAmount = health / maxHealth;

                if (berserker == true && !day)
                    berserker.GainWrath(value);
            }
            shieldBar.fillAmount = shield / maxShield;
        }

        if (health < 0f)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        //Application.Quit();
    }

    public void GainPoison(float value)
    {
        poison += value;

        if (poison >= value * 3f)
            TakeDamage(value * 3f, true);
        else TakeDamage(poison, true);
    }

    public void RestoreHealth(float value)
    {
        health += value;
        if (health > maxHealth)
            health = maxHealth;
        healthBar.fillAmount = health / maxHealth;
    }

    public void GainShield(float value)
    {
        shield += value;
        if (shield > maxShield)
            shield = maxShield;
        shieldBar.fillAmount = shield / maxShield;
    }

    public float SpeedMultiplyer(float efficiency)
    {
        return 1f + (fireRateBonus - 1f) * efficiency;
    }

    public float DamageDealtMultiplyer(float efficiency)
    {
        temp = 1f + (damageBonus - 1f);
        if (berserker)
            temp *= 1f + berserker.wrath;
        temp *= efficiency;
        return temp;
    }

    public void Collided(Collider2D other)
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
        else if (other.transform.tag == "Tools")
        {
            GainTools(1);

            if (steamGolem == true)
                steamGolem.ClockworkMachine(12 + level);
            Destroy(other.gameObject);
        }
        if (other.transform.tag == "Token")
        {
            GainTokens(1);
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "Medkit")
        {
            RestoreHealth(10f + maxHealth * 0.16f);
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "Accessory")
        {
            eq.Accessories[Random.Range(0, eq.Accessories.Length)]++;
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "EnemyProjectal")
        {
            collidedBullet = other.GetComponent(typeof(EnemyBullet)) as EnemyBullet;
            TakeDamage(collidedBullet.damage, false);
            GainPoison(collidedBullet.poison);
            //Destroy(other.gameObject);
        }
    }

    public void NewDay()
    {
        day = true;
        dayCount++;
        RestoreHealth(40 + maxHealth * 0.5f);
        if (berserker)
        {
            berserker.wrath = 0;
            berserker.GainWrath(0);
            GainHP(1.2f + 1f * dayCount);
        }
        LevelUp();
    }

    public void Nightfall()
    {
        day = false;
        for (int i = 1; i < 3; i++)
        {
            if (eq.slotFilled[i])
            {
                eq.guns[i].ammo = eq.guns[i].maxAmmo - eq.guns[i].bulletsLeft;
            }
        }
        DisplayAmmo();
    }

    public void LevelUp()
    {
        level++;
        GainHP(healthIncrease);
        damageBonus += damageIncrease;
        fireRateBonus += fireRateIncrease;
        movementSpeed += movementSpeedIncrease;
    }

    public void GainHP(float value)
    {
        maxHealth += value;
        health += value;
        if (eq.Items[8] > 0)
        {
            damageBonus += 0.0004f * (maxHealth - 50f) * eq.Items[8];
        }
    }

    public void GainScrap(float amount)
    {
        amount *= 1f + 0.2f * eq.Items[5];

        scrap += amount;
        scrapInfo.text = scrap.ToString("0");

        if (steamGolem == true)
            steamGolem.ClockworkMachine(amount);
    }

    public void SpendScrap(float amount)
    {
        scrap -= amount;
        scrapInfo.text = scrap.ToString("0");
    }

    public void GainTools(int amount)
    {
        tools += amount;
        toolsInfo.text = tools.ToString("0");

        eq.guns[eq.equipped].GainSpecialCharge(0.06f * amount);
    }

    public void SpendTools(int amount)
    {
        tools -= amount;
        toolsInfo.text = tools.ToString("0");
    }

    public void GainTokens(int amount)
    {
        tokens += amount;
        tokensInfo.text = tokens.ToString("0");
    }

    public void SpendTokens(int amount)
    {
        tokens -= amount;
        tokensInfo.text = tokens.ToString("0");
    }
}
