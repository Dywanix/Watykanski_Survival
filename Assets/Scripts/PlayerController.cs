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
    public TMPro.TextMeshProUGUI healthInfo, ShieldInfo, magazineInfo, ammoInfo, goldInfo, toolsInfo, keysInfo, DashCharge;
    public Image healthBar, dropBar, shieldBar, dischargeBar, taskImage, dashImage, abilityImage, gunImage;
    public Bullet firedBullet;
    private EnemyBullet collidedBullet;

    /*public Gunslinger gunslinger;
    public Berserker berserker;
    public SteamGolem steamGolem;
    public Engineer engineer;*/

    Vector2 move;
    public bool mouseLeft, reloading, free = true, day = true;
    Vector3 mousePos, mouseVector;
    CameraController Cam;
    public float task, taskMax;

    // -- statystyki --
    public float maxHealth, dHealth, health, maxShield, dShield, shield, shieldChargeRate, shieldChargeDelay, rechargeTimer, poison,
    damageBonus, fireRateBonus, movementSpeed, additionalCritChance, abilityDamageBonus, cooldownReduction, forceIncrease, dashBaseCooldown, maxDashCooldown, abilityMaxCooldown, abilityCooldown, dashCooldown, dash;
    public int level = 1, dayCount = 1;
    bool undamaged, dashSecondCharge;
    int tempi, bonusTool;
    float temp, wrath;

    // -- zasoby --
    public int tools, toolsStored, keys;
    public float gold;

    // -- animacje --
    public Animator animator;
    public float moveSpeed = 5f;

    // -- pause menu --
    public GameObject Menu;
    public bool menuOpened;

    void Start()
    {
        GetMouseInput();
        Cam = FindObjectOfType<CameraController>();
        DisplayAmmo();
        health = maxHealth;
        dHealth = maxHealth;
        dShield = shield;
        healthBar.fillAmount = health / maxHealth;
        healthInfo.text = health.ToString("0") + "/" + maxHealth.ToString("0");
        dropBar.fillAmount = dHealth / maxHealth;
        dischargeBar.fillAmount = dShield / maxShield;
        shieldBar.fillAmount = shield / maxShield;
        ShieldInfo.text = shield.ToString("0") + "/" + maxShield.ToString("0");
        Invoke("Tick", 0.8f);
        for (int i = 0; i < eq.Items.Length; i++)
        {
            if (eq.Items[i])
                eq.PickUpItem(i);
        }
    }

    void Update()
    {
        if (free)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                DashCast();
            if (Input.GetKeyDown(KeyCode.M))
                eq.Accessories[Random.Range(0, eq.Accessories.Length)]++;

            GetInput();
            move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Movement();
            Aim();
            if (Input.GetMouseButton(1))
                AbilityCast();
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuOpened)
                OpenMenu();
            else CloseMenu();
        }

        if (dashCooldown > 0)
        {
            dashCooldown -= Time.deltaTime;
            dashImage.fillAmount = 1 - (dashCooldown / maxDashCooldown);
        }
        if (eq.Items[4] && !dashSecondCharge)
        {
            if (dashCooldown <= 0)
            {
                dashSecondCharge = true;
                maxDashCooldown = dashBaseCooldown / cooldownReduction;
                dashCooldown += maxDashCooldown;
                DashCharge.text = "+";
            }
        }
        if (abilityCooldown > 0)
        {
            abilityCooldown -= Time.deltaTime;
            abilityImage.fillAmount = 1 - (abilityCooldown / abilityMaxCooldown);
        }

        if (shield < maxShield)
        {
            if (rechargeTimer > 0)
                rechargeTimer -= Time.deltaTime;
            else
                GainShield(shieldChargeRate * Time.deltaTime);
        }

        if (dHealth > health)
        {
            dHealth -= (12f + maxHealth * 0.11f) * Time.deltaTime;
            dropBar.fillAmount = dHealth / maxHealth;
        }

        if (dShield > shield)
        {
            dShield -= (12f + maxShield * 0.11f) * Time.deltaTime;
            dischargeBar.fillAmount = dShield / maxShield;
        }
    }

    void FixedUpdate()
    {
        Body.velocity = move * movementSpeed * dash * Time.deltaTime;
    }

    void DashCast()
    {
        if (dashSecondCharge)
        {
            dashSecondCharge = false;
            DashCharge.text = "";
            Dash();
        }
        else if (dashCooldown <= 0)
        {
            maxDashCooldown = dashBaseCooldown / cooldownReduction;
            dashCooldown = maxDashCooldown;

            Dash();
        }
    }

    void Dash()
    {
        dash = 6.55f;
        Invoke("Dashed", 0.13f);

        if (eq.Items[13])
        {
            tempi = eq.guns[eq.equipped].MagazineTotalSize() / 4;

            for (int i = 0; i < tempi; i++)
            {
                if (eq.guns[eq.equipped].infiniteAmmo)
                {
                    if (eq.guns[eq.equipped].bulletsLeft < eq.guns[eq.equipped].MagazineTotalSize())
                        eq.guns[eq.equipped].bulletsLeft++;
                }
                else
                {
                    if (eq.guns[eq.equipped].bulletsLeft < eq.guns[eq.equipped].MagazineTotalSize() && eq.guns[eq.equipped].ammo > 0)
                    {
                        eq.guns[eq.equipped].bulletsLeft++;
                        eq.guns[eq.equipped].ammo--;
                    }
                }
            }
            DisplayAmmo();
        }
    }

    void Dashed()
    {
        dash = 1f;

        if (eq.guns[eq.equipped].Accessories[19] > 0)
            DashFire();

        if (eq.Items[9])
        {
            movementSpeed *= 1.2f;
            Invoke("SprintEnd", 2.5f);
        }
    }

    public void NewTask(float duration)
    {
        taskMax = duration;
        task += duration;
    }

    void Tick()
    {
        //Invoke("Tick", 1f);
    }

    void GetInput()
    {
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
        if (Input.GetAxis("Horizontal") != 0)
        {
            animator.SetFloat("moveSpeed", Mathf.Abs(Input.GetAxis("Horizontal")));
        }
        else if (Input.GetAxis("Vertical") != 0)
        {
            animator.SetFloat("moveSpeed", Mathf.Abs(Input.GetAxis("Vertical")));
        }
        else
        {
            animator.SetFloat("moveSpeed", 0f);
        }

        /*if(Input.GetAxis("Horizontal") > 0.01f)
        {
            Dude.rotation = new Quaternion(0, 0, 0, 0);
        }
        else if (Input.GetAxis("Horizontal") < -0.01f)
        {
            Dude.rotation = new Quaternion(0, 180, 0, 0);
        }*/

        //Vector3 tempPos = transform.position;
        /*if ((xInput == -1f || xInput == 1f) && (yInput == -1f || yInput == 1f))
        {
            xInput *= 0.7f;
            yInput *= 0.7f;
        }*/
        //tempPos += new Vector3(xInput, yInput, 0) * (movementSpeed + dash) * Time.deltaTime;
        //transform.position = tempPos;
    }

    void Aim()
    {
        float gunAngle = Mathf.Atan2(mouseVector.y, mouseVector.x) * Mathf.Rad2Deg;
        Gun.rotation = gunAngle - 90f;
        GunRot.localScale = new Vector3(0.25f, 0.25f, 1f);
        Dude.rotation = new Quaternion(0, 0, 0, 0);
        if (Gun.rotation > 0f || Gun.rotation < -180f)
        {
            GunRot.localScale = new Vector3(-0.25f, 0.25f, 1f);
            Dude.rotation = new Quaternion(0, 180, 0, 0);
        }
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
                Shoot();
                if (eq.guns[eq.equipped].burst > 0)
                {
                    for (int i = 0; i < eq.guns[eq.equipped].burst; i++)
                    {
                        Invoke("BurstShot", eq.guns[eq.equipped].burstDelay * (i + 1));
                    }
                }
                if (eq.guns[eq.equipped].Accessories[21] > 0)
                {
                    temp = 1f + 0.35f * eq.guns[eq.equipped].Accessories[21] * (eq.guns[eq.equipped].MagazineTotalSize() - eq.guns[eq.equipped].bulletsLeft) / eq.guns[eq.equipped].MagazineTotalSize();
                    NewTask(eq.guns[eq.equipped].fireRate / temp);
                }
                else NewTask(eq.guns[eq.equipped].fireRate);
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
        else if (Input.mouseScrollDelta.y > 0f)
            SwapNextGun(true);
        else if (Input.mouseScrollDelta.y < 0f)
            SwapNextGun(false);
    }

    void BurstShot()
    {
        if (eq.guns[eq.equipped].bulletsLeft > 0 || eq.guns[eq.equipped].infiniteMagazine)
        {
            Fire();
            if (!eq.guns[eq.equipped].infiniteMagazine)
            {
                if (eq.guns[eq.equipped].Accessories[14] * 0.18f < Random.Range(0f, 1f))
                    eq.guns[eq.equipped].bulletsLeft--;
                DisplayAmmo();
            }
        }
        else
        {
            if (eq.guns[eq.equipped].Accessories[14] * 0.18f > Random.Range(0f, 1f))
                Fire();
        }
    }

    public void Shoot(float accuracy_change = 0f)
    {
        if (eq.Items[24] && Random.Range(0f, 1f) >= 0.83f)
            Fire(accuracy_change);

        Fire(accuracy_change);

        if (!eq.guns[eq.equipped].infiniteMagazine)
        {
            if (eq.guns[eq.equipped].Accessories[14] * 0.18f < Random.Range(0f, 1f))
                eq.guns[eq.equipped].bulletsLeft--;
            DisplayAmmo();
        }

        Cam.Shake((transform.position - Barrel.position).normalized, eq.guns[eq.equipped].cameraShake, eq.guns[eq.equipped].shakeDuration);
    }

    public void Fire(float accuracy_change = 0f)
    {
        for (int i = 0; i < eq.guns[eq.equipped].BulletsFired(); i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + Random.Range(-eq.guns[eq.equipped].accuracy - accuracy_change, eq.guns[eq.equipped].accuracy + accuracy_change));
            GameObject bullet = Instantiate(eq.guns[eq.equipped].bulletPrefab[Random.Range(0, eq.guns[eq.equipped].bulletPrefab.Length)], Barrel.position, Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrel.up * eq.guns[eq.equipped].force * forceIncrease * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet(1f);
        }
        if (eq.guns[eq.equipped].Accessories[15] * 0.15f >= Random.Range(0f, 1f))
        {
            FireDirection(-32f, accuracy_change);
            FireDirection(32f, accuracy_change);
        }

        if (eq.Items[17])
            eq.OnHit(1.1f);
        else eq.OnHit(1f);
    }

    public void FireDirection(float direction, float accuracy_change = 0f)
    {
        for (int i = 0; i < eq.guns[eq.equipped].BulletsFired(); i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + direction + Random.Range(-eq.guns[eq.equipped].accuracy - accuracy_change, eq.guns[eq.equipped].accuracy + accuracy_change));
            GameObject bullet = Instantiate(eq.guns[eq.equipped].bulletPrefab[Random.Range(0, eq.guns[eq.equipped].bulletPrefab.Length)], Barrel.position, Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrel.up * eq.guns[eq.equipped].force * forceIncrease * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet(1f);
        }
    }

    public void SetBullet(float efficiency)
    {
        firedBullet.duration = eq.guns[eq.equipped].range;
        firedBullet.damage = eq.guns[eq.equipped].Damage() * DamageDealtMultiplyer(1f);
        if (eq.guns[eq.equipped].Accessories[16] > 0)
        {
            firedBullet.damage *= 1f + (0.0025f * eq.guns[eq.equipped].Damage() * eq.guns[eq.equipped].Accessories[16]);
        }
        if (eq.guns[eq.equipped].Accessories[22] > 0)
        {
            firedBullet.damage *= 1f + (0.006f * eq.guns[eq.equipped].MagazineTotalSize() * eq.guns[eq.equipped].Accessories[16]);
        }
        firedBullet.DoT = eq.guns[eq.equipped].DoT;
        if (eq.guns[eq.equipped].Accessories[18] > 0)
        {
            firedBullet.DoT += 0.5f * eq.guns[eq.equipped].penetration * eq.guns[eq.equipped].Accessories[18];
        }
        firedBullet.curse = eq.guns[eq.equipped].curse;
        firedBullet.damageGain = eq.guns[eq.equipped].damageGain;
        firedBullet.penetration = eq.guns[eq.equipped].penetration;
        firedBullet.armorShred = eq.guns[eq.equipped].armorShred;
        firedBullet.vulnerableApplied = eq.guns[eq.equipped].vulnerableApplied;
        firedBullet.slowDuration = eq.guns[eq.equipped].slowDuration;
        firedBullet.stunChance = eq.guns[eq.equipped].stunChance;
        firedBullet.stunDuration = eq.guns[eq.equipped].stunDuration;
        firedBullet.pierce = eq.guns[eq.equipped].pierce;
        firedBullet.pierceEfficiency = eq.guns[eq.equipped].pierceEfficiency;
        if (eq.guns[eq.equipped].Accessories[9] > 0)
        {
            temp = 0.05f + 0.1f / (1f * eq.guns[eq.equipped].pierce);
            firedBullet.pierceEfficiency += temp * eq.guns[eq.equipped].Accessories[9];
        }
        firedBullet.special = eq.guns[eq.equipped].special;

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
            if (eq.guns[eq.equipped].Accessories[8] > 0)
                firedBullet.pierce += eq.guns[eq.equipped].Accessories[8];

            if (eq.Items[17])
                eq.OnHit(0.25f);
        }
    }

    void AbilityCast()
    {
        if (abilityCooldown <= 0f)
        {
            if (eq.guns[eq.equipped].ammoRequired == 0)
            {
                if (eq.guns[eq.equipped].free || task <= 0f)
                    UseAbility();
            }
            else if (eq.guns[eq.equipped].bulletsLeft >= eq.guns[eq.equipped].ammoRequired)
            {
                if (eq.guns[eq.equipped].free || task <= 0f)
                    UseAbility();
            }
        }
    }

    void UseAbility()
    {
        abilityMaxCooldown = eq.guns[eq.equipped].Cooldown() / cooldownReduction;
        abilityCooldown = abilityMaxCooldown;
        NewTask(eq.guns[eq.equipped].task);

        switch (eq.guns[eq.equipped].gunName)
        {
            case "Revolver":
                temp = (0.03f + 0.1f * eq.guns[eq.equipped].fireRate / SpeedMultiplyer(1f)) / (1f + 0.12f * eq.guns[eq.equipped].level);
                for (float i = 0; i < 0.4f; i += temp)
                {
                    Invoke("BurstShot", i);
                }
                break;
            case "Sawed-off Shotgun":
                eq.guns[eq.equipped].bulletsLeft -= 1;
                DisplayAmmo();
                FireAbility();
                firedBullet.damage *= 0.84f + 0.1f * eq.guns[eq.equipped].BulletsFired();
                firedBullet.special = eq.guns[eq.equipped].BulletsFired() + eq.guns[eq.equipped].level;
                break;
            case "Jumping SMG":
                FireAbility();
                firedBullet.damage *= 1.18f + 0.09f * eq.guns[eq.equipped].level;
                break;
            case "Poison Gun":
                FireAbility();
                firedBullet.damage = (0.04f + 0.01f * eq.guns[eq.equipped].level) * firedBullet.damage + 2f;
                firedBullet.DoT += 0.5f * firedBullet.DoT + 3f;
                firedBullet.slowDuration += 0.16f / eq.guns[eq.equipped].fireRate;
                break;
            case "Parallel Gun":
                eq.guns[eq.equipped].bulletsLeft -= 1;
                DisplayAmmo();
                FireAbility();
                firedBullet.damage *= 1.04f + 0.08f * eq.guns[eq.equipped].level;
                break;
        }
    }

    void FireAbility()
    {
        Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + 0.5f * Random.Range(-eq.guns[eq.equipped].accuracy, eq.guns[eq.equipped].accuracy));
        GameObject bullet = Instantiate(eq.guns[eq.equipped].AbilityBullet, Barrel.position, Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(Barrel.up * eq.guns[eq.equipped].force * forceIncrease * Random.Range(0.94f, 1.06f), ForceMode2D.Impulse);
        firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        SetBullet(1f);
    }

    void Rain()
    {
        if (!day)
        {
            FireDirection(Random.Range(0f, 360f), 0f);
            Invoke("Rain", eq.guns[eq.equipped].fireRate * 2.8f / SpeedMultiplyer(1.3f));
        }
    }

    void Reload()
    {
        if (!eq.guns[eq.equipped].infiniteMagazine && eq.guns[eq.equipped].bulletsLeft < eq.guns[eq.equipped].MagazineTotalSize())
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
                eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].individualReloadCount;
                if (eq.guns[eq.equipped].bulletsLeft > eq.guns[eq.equipped].MagazineTotalSize())
                    eq.guns[eq.equipped].bulletsLeft = eq.guns[eq.equipped].MagazineTotalSize();
                eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].overload;
                if (eq.guns[eq.equipped].Accessories[10] > 0)
                    eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].Accessories[10] * eq.guns[eq.equipped].MagazineTotalSize() / 7;
                if (eq.guns[eq.equipped].bulletsLeft >= eq.guns[eq.equipped].MagazineTotalSize())
                    reloading = false;
                else
                {
                    NewTask(eq.guns[eq.equipped].reloadTime);
                }
            }
            else
            {
                eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].individualReloadCount;
                eq.guns[eq.equipped].ammo -= eq.guns[eq.equipped].individualReloadCount;
                if (eq.guns[eq.equipped].bulletsLeft > eq.guns[eq.equipped].MagazineTotalSize())
                {
                    eq.guns[eq.equipped].ammo += eq.guns[eq.equipped].bulletsLeft -= eq.guns[eq.equipped].MagazineTotalSize();
                    eq.guns[eq.equipped].bulletsLeft = eq.guns[eq.equipped].MagazineTotalSize();
                }
                eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].overload;
                eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].Accessories[10] * eq.guns[eq.equipped].MagazineTotalSize() / 7;
                if (eq.guns[eq.equipped].bulletsLeft >= eq.guns[eq.equipped].MagazineTotalSize() || eq.guns[eq.equipped].ammo <= 0)
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
                eq.guns[eq.equipped].bulletsLeft = eq.guns[eq.equipped].MagazineTotalSize();
            }
            else if (eq.guns[eq.equipped].ammo >= eq.guns[eq.equipped].MagazineTotalSize() - eq.guns[eq.equipped].bulletsLeft)
            {
                eq.guns[eq.equipped].ammo -= (eq.guns[eq.equipped].MagazineTotalSize() - eq.guns[eq.equipped].bulletsLeft);
                eq.guns[eq.equipped].bulletsLeft = eq.guns[eq.equipped].MagazineTotalSize();
            }
            else
            {
                eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].ammo;
                eq.guns[eq.equipped].ammo = 0;
            }
            eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].overload;
            eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].Accessories[10] * eq.guns[eq.equipped].MagazineTotalSize() / 7;
            reloading = false;
        }
        DisplayAmmo();
    }

    /*public void ReloadAmmo(int amount, bool full)
    {
        tempi = amount;
        eq.guns[eq.equipped].bulletsLeft += amount;
        if (eq.guns[eq.equipped].bulletsLeft > eq.guns[eq.equipped].MagazineTotalSize())
            eq.guns[eq.equipped].bulletsLeft = eq.guns[eq.equipped].MagazineTotalSize();
        if (full)
            eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].overload;
        if (eq.guns[eq.equipped].Accessories[10] > 0)
            eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].Accessories[10] * eq.guns[eq.equipped].MagazineTotalSize() / 8;
    }*/

    public void DisplayAmmo()
    {
        magazineInfo.text = (eq.guns[eq.equipped].bulletsLeft).ToString("") + "/" + eq.guns[eq.equipped].MagazineTotalSize();
        ammoInfo.text = (eq.guns[eq.equipped].ammo).ToString("");
    }

    public void SwapGun(int which)
    {
        if (which != eq.equipped && eq.slotFilled[which])
        {
            eq.equipped = which;
            gunImage.sprite = eq.guns[eq.equipped].gunSprite;
            eq.equippedGun.sprite = eq.guns[eq.equipped].holdingSprite;
            DisplayAmmo();
            NewTask(0.33f);
        }
    }

    public void SwapNextGun(bool up)
    {
        if (up)
        {
            do
            {
                eq.equipped++;
                if (eq.equipped >= 3)
                    eq.equipped -= 3;
            } while (!eq.slotFilled[eq.equipped]);
        }
        else
        {
            do
            {
                eq.equipped--;
                if (eq.equipped < 0)
                    eq.equipped += 3;
            } while (!eq.slotFilled[eq.equipped]);
        }
        gunImage.sprite = eq.guns[eq.equipped].gunSprite;
        eq.equippedGun.sprite = eq.guns[eq.equipped].gunSprite;
        DisplayAmmo();
        NewTask(0.33f);
    }

    public void PickUpGun(int which)
    {
        if (!eq.slotFilled[1])
        {
            eq.guns[1] = eq.Library.guns[which];
            eq.slotFilled[1] = true;
            eq.guns[1].parts = toolsStored;
            if (eq.Items[8])
                eq.guns[1].MaxSlots++;
        }
        else if (!eq.slotFilled[2])
        {
            eq.guns[2] = eq.Library.guns[which];
            eq.slotFilled[2] = true;
            eq.guns[2].parts = toolsStored;
            if (eq.Items[8])
                eq.guns[2].MaxSlots++;
        }
    }

    void SprintEnd()
    {
        movementSpeed /= 1.2f;
    }

    void DashFire()
    {
        temp = 1.1f * eq.guns[eq.equipped].Accessories[19] * SpeedMultiplyer(1f);
        tempi = 0;

        for (float f = 0; f <= temp; f += eq.guns[eq.equipped].fireRate)
        {
            tempi++;
        }

        for (int i = 0; i < tempi; i++)
        {
            FireDirection((i * 2 - tempi + 1) * (5f / (1f + 0.05f * tempi)), 0f);
        }
    }

    public void TakeDamage(float value, bool pierce)
    {
        if (eq.Items[19])
            value *= 1.25f;
        if (eq.Items[0])
        {
            if (undamaged)
            {
                undamaged = false;
                movementSpeed /= 1.16f;
            }
        }
        if (eq.Items[12])
        {
            if (value > 8)
                value -= 3;
            else if (value > 5)
                value = 5;
        }
        if (pierce)
        {
            health -= value;
            if (eq.Items[3])
                GainShield(value * 0.45f);
            healthBar.fillAmount = health / maxHealth;

            if (eq.Items[25] && !day)
                wrath += value / 800;
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
                if (eq.Items[3])
                    GainShield(value * 0.4f);
                healthBar.fillAmount = health / maxHealth;

                if (eq.Items[25] && !day)
                    wrath += value / 800;
            }
            shieldBar.fillAmount = shield / maxShield;
        }

        healthInfo.text = health.ToString("0") + "/" + maxHealth.ToString("0");
        ShieldInfo.text = shield.ToString("0") + "/" + maxShield.ToString("0");

        if (health < 0f)
            ReturnToMenu();
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
        if (eq.Items[6])
            value *= 1.2f;
        health += value;
        if (health > maxHealth)
            health = maxHealth;
        if (dHealth < health)
        {
            dHealth = health;
            dropBar.fillAmount = dHealth / maxHealth;
        }
        healthBar.fillAmount = health / maxHealth;
        healthInfo.text = health.ToString("0") + "/" + maxHealth.ToString("0");
    }

    public void GainShield(float value)
    {
        shield += value;
        if (shield > maxShield)
            shield = maxShield;
        if (dShield < shield)
        {
            dShield = shield;
            dischargeBar.fillAmount = dShield / maxShield;
        }
        shieldBar.fillAmount = shield / maxShield;
        ShieldInfo.text = shield.ToString("0") + "/" + maxShield.ToString("0");
    }

    public float SpeedMultiplyer(float efficiency)
    {
        temp = fireRateBonus;
        temp *= 1f + (temp - 1f) * efficiency;
        return temp;
    }

    public float DamageDealtMultiplyer(float efficiency)
    {
        temp = damageBonus;
        temp *= 1f + wrath;
        temp *= 1f + (temp - 1f) * efficiency;
        return temp;
    }

    public void Collided(Collider2D other)
    {
        if (other.transform.tag == "Gold")
        {
            GainGold(1);
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "Gold5")
        {
            GainGold(5);
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "Tools")
        {
            GainTools(1);

            Destroy(other.gameObject);
        }
        if (other.transform.tag == "Key")
        {
            GainKeys(1);
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
        else if (other.transform.tag == "Item")
        {
            eq.Items[Random.Range(0, eq.Items.Length)] = true;
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "EnemyProjectal")
        {
            collidedBullet = other.GetComponent(typeof(EnemyBullet)) as EnemyBullet;
            if (eq.Items[1])
            {
                TakeDamage(collidedBullet.damage * 0.9f, false);
                eq.Deflect(collidedBullet.damage);
            }
            else TakeDamage(collidedBullet.damage, false);
            GainPoison(collidedBullet.poison);
            //Destroy(other.gameObject);
        }
    }

    public void NewDay()
    {
        day = true;
        dayCount++;
        //RestoreHealth(40 + maxHealth * 0.5f);
        wrath = 0;
        for (int i = 0; i < 3; i++)
        {
            if (eq.slotFilled[i])
            {
                if (eq.guns[i].Accessories[27] > 0)
                    eq.guns[i].parts += 5 * eq.guns[i].Accessories[27];
            }
        }
        if (eq.Items[18])
        {
            GainGold(5);
            GainTools(1);
        }
        LevelUp();
    }

    public void Nightfall()
    {
        day = false;
        if (eq.Items[0])
        {
            if (!undamaged)
            {
                undamaged = true;
                movementSpeed *= 1.16f;
            }
        }
        if (eq.Items[2])
            Invoke("Rain", 0.2f);
        if (eq.Items[11])
            GainShield(10 + 0.05f * maxShield);
    }

    public void AmmoRefill()
    {
        for (int i = 1; i < 3; i++)
        {
            if (eq.slotFilled[i])
            {
                eq.guns[i].ammo = eq.guns[i].maxAmmo + eq.guns[i].bonusAmmo - eq.guns[i].bulletsLeft;
                eq.guns[i].bonusAmmo = 0;
                if (eq.Items[23])
                    eq.guns[i].ammo += eq.guns[i].maxAmmo / 4;
            }
        }
        DisplayAmmo();
    }

    public void LevelUp()
    {
        level++;
    }

    public void GainHP(float value)
    {
        maxHealth += value;
        health += value;
        healthInfo.text = health.ToString("0") + "/" + maxHealth.ToString("0");
        if (eq.Items[7])
        {
            damageBonus += 0.001f * value;
        }
        dHealth += value;
        dropBar.fillAmount = dHealth / maxHealth;
        healthBar.fillAmount = health / maxHealth;
    }

    public void GainGold(float amount)
    {
        //amount *= 1f + 0.2f * eq.Items[5];

        gold += amount;
        goldInfo.text = gold.ToString("0");
    }

    public void SpendGold(float amount)
    {
        gold -= amount;
        goldInfo.text = gold.ToString("0");
    }

    public void GainTools(int amount)
    {
        if (eq.Items[26])
        {
            tempi = 0;
            bonusTool += amount;
            while (bonusTool >= 5)
            {
                bonusTool -= 5;
                tempi++;
            }
            amount += tempi;
        }

        tools += amount;
        toolsStored += amount;
        toolsInfo.text = tools.ToString("0");
        for (int i = 0; i < 3; i++)
        {
            if (eq.slotFilled[i])
                eq.guns[i].parts += amount;
        }

        //eq.guns[eq.equipped].GainSpecialCharge(0.06f * amount);
    }

    public void SpendTools(int amount)
    {
        tools -= amount;
        toolsInfo.text = tools.ToString("0");
    }

    public void GainKeys(int amount)
    {
        keys += amount;
        keysInfo.text = keys.ToString("0");
    }

    public void SpendKeys(int amount)
    {
        keys -= amount;
        keysInfo.text = keys.ToString("0");
    }

    public void AmmoPack()
    {
        for (int i = 1; i < 3; i++)
        {
            if (eq.slotFilled[i])
            {
                eq.guns[i].bonusAmmo += eq.guns[i].maxAmmo / 3;
                eq.guns[i].ammo += eq.guns[i].maxAmmo / 3;
            }
        }
    }

    /*public void GainPerk(int ability, int which)
    {
        if (gunslinger)
        {
            gunslinger.GainPerk(ability, which);
        }
        else if (berserker)
        {
            berserker.GainPerk(ability, which);
        }
        else if (engineer)
        {
            engineer.GainPerk(ability, which);
        }
    }*/

    void OpenMenu()
    {
        Menu.SetActive(true);
        menuOpened = true;
        free = false;
    }

    public void CloseMenu()
    {
        Menu.SetActive(false);
        menuOpened = false;
        free = true;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}