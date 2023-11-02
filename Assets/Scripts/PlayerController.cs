using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Transform Barrel, Hand, Dude, GunRot, TargetArea;
    public Rigidbody2D Body, Gun;
    public Equipment eq;
    public TMPro.TextMeshProUGUI healthInfo, ShieldInfo, magazineInfo, ammoInfo, goldInfo, toolsInfo, keysInfo, DashCharge, GrenadeCharge;
    public Image healthBar, dropBar, shieldBar, dischargeBar, taskImage, dashImage, abilityImage, gunImage;
    public Bullet firedBullet;
    public GameObject Grenade;
    public GrenadeEffects Effects;
    private EnemyBullet collidedBullet;

    /*public Gunslinger gunslinger;
    public Berserker berserker;
    public SteamGolem steamGolem;
    public Engineer engineer;*/

    Vector2 move;
    public bool mouseLeft, reloading, free = true, day = true;
    public Vector3 mousePos, mouseVector;
    CameraController Cam;
    public float task, taskMax;

    [Header("Stats")]
    public float maxHealth;
    public float dHealth, health, maxShield, dShield, shield, poison,
    damageBonus, fireRateBonus, movementSpeed, additionalCritChance, cooldownReduction, forceIncrease, dashBaseCooldown, maxDashCooldown, dashCooldown, grenadeMaxCharges, grenadeCharges, throwRange, grenadeBaseCooldown, grenadeMaxCooldown, grenadeCooldown, dash;
    public int level = 1, dayCount = 1;
    bool undamaged, dashSecondCharge, protection;
    int tempi, bonusTool;
    float temp, wrath;

    [Header("Resources")]
    public float gold;
    public int tools, toolsStored, keys;

    // -- animacje --
    public Animator animator;
    public float moveSpeed = 5f;

    // -- Huds --
    public GameObject Menu, Tab;
    public bool menuOpened, tabOpened;
    public TMPro.TextMeshProUGUI[] TabStatsText;
    public GameObject[] CollectedItems;
    public Image[] CollectedImages;

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
        GainGold(0f);
        GainTools(0);
        GainKeys(0);
        toolsStored = tools;
        eq.guns[eq.equipped].parts = toolsStored;
        Invoke("Tick", 0.8f);
        if (eq.gambler)
        {
            temp = 28.9f;
            while (temp > 0f)
            {
                tempi = Random.Range(0, 6);
                switch (tempi)
                {
                    case 0:
                        GainHP(10);
                        temp -= 2f;
                        break;
                    case 1:
                        GainDMG(0.012f);
                        temp -= 1.8f;
                        break;
                    case 2:
                        GainFR(0.014f);
                        temp -= 1.75f;
                        break;
                    case 3:
                        GainMS(8f);
                        temp -= 1.6f;
                        break;
                    case 4:
                        GainGold(4);
                        temp -= 0.8f;
                        break;
                    case 5:
                        GainTools(1);
                        temp -= 0.6f;
                        break;
                }
            }
        }
        else
        {
            for (int i = 0; i < eq.Items.Length; i++)
            {
                if (eq.Items[i])
                    eq.PickUpItem(i);
            }
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
            if (Input.GetMouseButtonDown(1))
                GrenadeCast();
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
            if (tabOpened)
                CloseTab();
            else if (!menuOpened)
                OpenMenu();
            else
            {
                CloseMenu();
                CloseTab();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!tabOpened)
                OpenTab();
            else
                CloseTab();
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
                DashCharge.text = "1";
            }
        }
        if (grenadeCooldown > 0)
        {
            grenadeCooldown -= Time.deltaTime;
            abilityImage.fillAmount = 1 - (grenadeCooldown / grenadeMaxCooldown);
        }
        else
        {
            if (grenadeCharges < grenadeMaxCharges)
            {
                grenadeCharges++;
                GrenadeCharge.text = "+" + grenadeCharges.ToString("0");
                grenadeMaxCooldown = grenadeBaseCooldown / cooldownReduction;
                grenadeCooldown = grenadeMaxCooldown;
            }
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
            Invoke("DashFire", 0.07f);

        if (eq.Items[9])
        {
            movementSpeed *= 1.25f;
            Invoke("SprintEnd", 2.25f);
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
        if (eq.Items[24] && Random.Range(0f, 1f) >= 0.82f)
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
        if (eq.guns[eq.equipped].targetArea)
        {
            for (int i = 0; i < eq.guns[eq.equipped].BulletsFired(); i++)
            {
                temp = 1f;
                if (Vector3.Distance(transform.position, new Vector2(mousePos[0], mousePos[1])) <= eq.guns[eq.equipped].range * 24f)
                    TargetArea.position = new Vector2(mousePos[0] + Random.Range(-eq.guns[eq.equipped].accuracy, eq.guns[eq.equipped].accuracy) / 5, mousePos[1] + Random.Range(-eq.guns[eq.equipped].accuracy, eq.guns[eq.equipped].accuracy) / 5);
                else
                {
                    temp = Vector3.Distance(transform.position, new Vector2(mousePos[0], mousePos[1])) / (eq.guns[eq.equipped].range * 24f);
                    TargetArea.position = new Vector2(transform.position.x + (mousePos[0] - transform.position.x) / temp + Random.Range(-eq.guns[eq.equipped].accuracy, eq.guns[eq.equipped].accuracy) / 5, transform.position.y + (mousePos[1] - transform.position.y) / temp + Random.Range(-eq.guns[eq.equipped].accuracy, eq.guns[eq.equipped].accuracy) / 5);
                }
                GameObject bullet = Instantiate(eq.guns[eq.equipped].bulletPrefab[Random.Range(0, eq.guns[eq.equipped].bulletPrefab.Length)], Barrel.position, Barrel.rotation);
                Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
                firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
                SetBullet(1f);
                firedBullet.TargetedLocation = TargetArea;
                firedBullet.duration /= Random.Range(0.94f, 1.06f);
                firedBullet.duration /= forceIncrease;
            }
        }
        else
        {
            for (int i = 0; i < eq.guns[eq.equipped].BulletsFired(); i++)
            {
                Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + Random.Range(-eq.guns[eq.equipped].accuracy - accuracy_change, eq.guns[eq.equipped].accuracy + accuracy_change));
                GameObject bullet = Instantiate(eq.guns[eq.equipped].bulletPrefab[Random.Range(0, eq.guns[eq.equipped].bulletPrefab.Length)], Barrel.position, Barrel.rotation);
                Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
                firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
                SetBullet(1f);
                bullet_body.AddForce(Barrel.up * firedBullet.force, ForceMode2D.Impulse);
            }
        }
        if (eq.guns[eq.equipped].Accessories[15] * 0.15f >= Random.Range(0f, 1f))
        {
            FireDirection(-32f, accuracy_change);
            FireDirection(32f, accuracy_change);
        }

        if (eq.Items[17])
            eq.OnHit(1.16f);
        else eq.OnHit(1f);
    }

    public void FireDirection(float direction, float accuracy_change = 0f)
    {
        for (int i = 0; i < eq.guns[eq.equipped].BulletsFired(); i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + direction + Random.Range(-eq.guns[eq.equipped].accuracy - accuracy_change, eq.guns[eq.equipped].accuracy + accuracy_change));
            GameObject bullet = Instantiate(eq.guns[eq.equipped].bulletPrefab[Random.Range(0, eq.guns[eq.equipped].bulletPrefab.Length)], Barrel.position, Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet(1f);
            if (eq.guns[eq.equipped].targetArea && Vector3.Distance(transform.position, new Vector2(mousePos[0], mousePos[1])) <= eq.guns[eq.equipped].range * 24f)
                bullet_body.AddForce(Barrel.up * firedBullet.force * (Vector3.Distance(transform.position, new Vector2(mousePos[0], mousePos[1])) / (eq.guns[eq.equipped].range * 24f)), ForceMode2D.Impulse);
            else bullet_body.AddForce(Barrel.up * firedBullet.force, ForceMode2D.Impulse);
        }
    }

    public void SetBullet(float efficiency)
    {
        firedBullet.duration = eq.guns[eq.equipped].range;
        firedBullet.force = eq.guns[eq.equipped].force * forceIncrease * Random.Range(1.02f, 1.08f);
        firedBullet.mass = eq.guns[eq.equipped].heft;
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
        firedBullet.shatter = eq.guns[eq.equipped].shatter;
        firedBullet.incendiary = eq.guns[eq.equipped].incendiary;
        firedBullet.curse = eq.guns[eq.equipped].curse;
        firedBullet.damageGain = eq.guns[eq.equipped].damageGain;
        firedBullet.penetration = eq.guns[eq.equipped].penetration;
        firedBullet.armorShred = eq.guns[eq.equipped].armorShred;
        firedBullet.vulnerableApplied = eq.guns[eq.equipped].vulnerableApplied;
        firedBullet.slowDuration = eq.guns[eq.equipped].slowDuration;
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
            firedBullet.mass *= 0.8f + eq.guns[eq.equipped].critDamage * 0.5f;
            firedBullet.stunDuration *= 0.7f + eq.guns[eq.equipped].critDamage * 0.3f;
            firedBullet.pierceEfficiency *= 1.1f;
            firedBullet.crit = true;
            if (eq.guns[eq.equipped].Accessories[8] > 0)
                firedBullet.pierce += eq.guns[eq.equipped].Accessories[8];

            if (eq.Items[17])
                eq.OnHit(0.24f);
        }
    }

    void GrenadeCast()
    {
        if (grenadeCooldown <= 0f || grenadeCharges > 0)
        {
            ThrowGrenade();
        }
    }

    void ThrowGrenade()
    {
        if (grenadeCharges > 0)
        {
            grenadeCharges--;
            if (grenadeCharges == 0)
                GrenadeCharge.text = "";
            else GrenadeCharge.text = "+" + grenadeCharges.ToString("0");
        }
        else
        {
            grenadeMaxCooldown = grenadeBaseCooldown / cooldownReduction;
            grenadeCooldown = grenadeMaxCooldown;
        }

        temp = 1f;
        if (Vector3.Distance(transform.position, new Vector2(mousePos[0], mousePos[1])) <= ThrowRange())
            TargetArea.position = new Vector2(mousePos[0], mousePos[1]);
        else
        {
            temp = Vector3.Distance(transform.position, new Vector2(mousePos[0], mousePos[1])) / ThrowRange();
            TargetArea.position = new Vector2(transform.position.x + (mousePos[0] - transform.position.x) / temp, transform.position.y + (mousePos[1] - transform.position.y) / temp);
        }
        GameObject bullet = Instantiate(Grenade, Barrel.position, Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        Effects = bullet.GetComponent(typeof(GrenadeEffects)) as GrenadeEffects;
        firedBullet.TargetedLocation = TargetArea;
        firedBullet.duration /= forceIncrease;
        firedBullet.damage = (26f + toolsStored * 0.1f) * DamageDealtMultiplyer(1.05f);
        if (eq.Items[15])
            firedBullet.damage *= 1.23f;
        if (eq.Items[28])
            firedBullet.shatter += 0.78f;
        if (eq.Items[29])
            Effects.venom = true;
        if (eq.Items[30])
            Effects.small = true;
    }

    float ThrowRange()
    {
        if (eq.Items[31])
            return throwRange * DamageDealtMultiplyer(0.6f);
        else return throwRange;
    }

    /*void UseAbility()
    {
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
                firedBullet.damage *= 0.8f + 0.12f * eq.guns[eq.equipped].BulletsFired();
                firedBullet.special = eq.guns[eq.equipped].BulletsFired() + eq.guns[eq.equipped].level;
                break;
            case "Burst Handgun":
                ThrowAbility();
                firedBullet.damage *= 1.16f + 0.12f * eq.guns[eq.equipped].level;
                firedBullet.shatter += 0.66f + 0.11f * eq.guns[eq.equipped].level;
                firedBullet.stunDuration += 0.33f + (0.22f / eq.guns[eq.equipped].fireRate);
                break;
            case "Jumping SMG":
                FireAbility();
                firedBullet.damage *= 1.18f + 0.09f * eq.guns[eq.equipped].level;
                break;
            case "Shotgun":
                InstantiateAbility();
                firedBullet.damage = (0.34f + 0.22f * eq.guns[eq.equipped].BulletsFired()) * firedBullet.damage + 20f + 12f * eq.guns[eq.equipped].level;
                firedBullet.pierce = 100;
                firedBullet.pierceEfficiency = 1f;
                firedBullet.duration = 0.4f;
                break;
            case "Poison Gun":
                ThrowAbility();
                firedBullet.damage = (0.04f + 0.01f * eq.guns[eq.equipped].level) * firedBullet.damage + 2f;
                firedBullet.DoT += 0.4f * firedBullet.DoT + 2.8f;
                firedBullet.slowDuration += 0.16f / eq.guns[eq.equipped].fireRate;
                break;
            case "Parallel Gun":
                eq.guns[eq.equipped].bulletsLeft -= 1;
                DisplayAmmo();
                FireAbility();
                firedBullet.damage *= 1.04f + 0.08f * eq.guns[eq.equipped].level;
                break;
        }
    }*/

    /*void FireAbility()
    {
        Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + 0.5f * Random.Range(-eq.guns[eq.equipped].accuracy, eq.guns[eq.equipped].accuracy));
        GameObject bullet = Instantiate(eq.guns[eq.equipped].AbilityBullet, Barrel.position, Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        SetBullet(1f);
        bullet_body.AddForce(Barrel.up * firedBullet.force, ForceMode2D.Impulse);
    }*/

    void Rain()
    {
        if (!day)
        {
            FireDirection(Random.Range(0f, 360f), 0f);
            Invoke("Rain", eq.guns[eq.equipped].fireRate * 2.75f / SpeedMultiplyer(1.35f));
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
        movementSpeed /= 1.25f;
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
        if (protection)
            protection = false;
        else
        {
            if (eq.Items[19])
                value *= 1.25f;
            if (eq.Items[0])
            {
                if (undamaged)
                {
                    undamaged = false;
                    movementSpeed /= 1.17f;
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
                    wrath += value / 600;
            }
            else
            {
                shield -= value;

                if (shield < 0)
                {
                    value = shield * (-1f);
                    shield = 0;
                    health -= value;
                    if (eq.Items[3])
                        GainShield(value * 0.45f);
                    healthBar.fillAmount = health / maxHealth;

                    if (eq.Items[25] && !day)
                        wrath += value / 600;
                }
                shieldBar.fillAmount = shield / maxShield;
            }

            healthInfo.text = health.ToString("0") + "/" + maxHealth.ToString("0");
            ShieldInfo.text = shield.ToString("0") + "/" + maxShield.ToString("0");

            if (health < 0f)
                ReturnToMenu();
        }
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
            value *= 1.25f;
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

    public void EnemySlained()
    {
        for (int i = 1; i < 3; i++)
        {
            if (eq.slotFilled[i] && eq.guns[i].ammoRegain > 0)
            {
                eq.guns[i].ammo += eq.guns[i].ammoRegain;
                DisplayAmmo();
            }
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
                movementSpeed *= 1.17f;
            }
        }
        if (eq.Items[2])
            Invoke("Rain", 0.2f);
        if (eq.Items[11])
            GainShield(10 + 0.05f * maxShield);
        if (eq.Items[34])
            protection = true;
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

    public void GainDMG(float value)
    {
        damageBonus += value;
    }

    public void GainFR(float value)
    {
        fireRateBonus += value;
        if (eq.Items[32])
            cooldownReduction += value / 4f;
    }

    public void GainMS(float value)
    {
        movementSpeed += value;
    }

    public void GainHP(float value)
    {
        maxHealth += value;
        health += value;
        healthInfo.text = health.ToString("0") + "/" + maxHealth.ToString("0");
        if (eq.Items[7])
            GainDMG(0.0011f * value);
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

    void OpenTab()
    {
        Tab.SetActive(true);
        TabStatsText[0].text = maxHealth.ToString("0");
        TabStatsText[1].text = ((damageBonus * 100f) -100f).ToString("0.0") + "%";
        TabStatsText[2].text = ((fireRateBonus * 100f) -100f).ToString("0.0") + "%";
        TabStatsText[3].text = ((movementSpeed / 5f) - 100f).ToString("0.0") + "%";
        for (int i = 0; i < eq.itemsCollected; i++)
        {
            CollectedItems[i].SetActive(true);
            CollectedImages[i].sprite = eq.ILibrary.ItemSprite[eq.ItemList[i]];
        }
        tabOpened = true;
        free = false;
    }

    public void ItemTooltipOpen(int which)
    {
        eq.Tooltip.text = eq.ILibrary.ItemTooltip[eq.ItemList[which]];
    }

    public void ItemTooltipClose()
    {
        eq.Tooltip.text = "";
    }

    public void CloseTab()
    {
        ItemTooltipClose();
        Tab.SetActive(false);
        tabOpened = false;
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