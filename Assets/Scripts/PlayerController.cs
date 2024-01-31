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
    public Backpack bp;
    public Map map;

    [Header("UI")]
    public TMPro.TextMeshProUGUI healthInfo;
    public TMPro.TextMeshProUGUI ShieldInfo, ToxicityInfo, magazineInfo, ammoInfo, goldInfo, toolsInfo, potionsInfo, DashCharge, GrenadeCharge;
    public Image healthBar, dropBar, shieldBar, dischargeBar, posionBar, taskImage, dashImage, abilityImage, gunImage, damageFlash;
    public RectTransform healthBack, healthFill, healthDrop, shieldBack, shieldFill, shieldDrop;

    [Header("Objects")]
    public Bullet firedBullet;
    public GameObject Grenade, CurrentBullet, DamageFlash;
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
    public int level = 1, dayCount = 1, luck, toxicityLevel;
    public bool undamaged, invulnerable;
    bool dashSecondCharge, protection;
    int tempi, tempi2, bonusTool;
    float temp, temp2, temp3, temp4, flashA;
    bool greenF;

    [Header("Items")]
    public int bloodMoney;
    public int builtShield;
    public float wrath, shieldCapacitor;

    [Header("Resources")]
    public float gold;
    public int tools, toolsStored, potions, maxPotions;

    [Header("Special Bullets")]
    public bool[] effectOn;
    public GameObject[] SpecialBullets;

    public SpriteRenderer playerSprite;

    // -- animacje --
    public Animator animator;
    public float moveSpeed = 5f;

    // -- Huds --
    public GameObject Menu;
    public bool menuOpened, tabOpened;

    void Start()
    {
        GetMouseInput();
        Cam = FindObjectOfType<CameraController>();
        map = GameObject.FindGameObjectWithTag("Map").GetComponent(typeof(Map)) as Map;
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
        //GainKeys(0);
        dash = 0f;
        Invoke("Tick", 0.8f);
        if (eq.gambler)
        {
            temp = 40.0f;
            while (temp > 0f)
            {
                tempi = Random.Range(0, 7);
                switch (tempi)
                {
                    case 0:
                        GainHP(5);
                        temp -= 2f;
                        break;
                    case 1:
                        GainDMG(0.01f);
                        temp -= 1.5f;
                        break;
                    case 2:
                        GainFR(0.01f);
                        temp -= 1.25f;
                        break;
                    case 3:
                        GainMS(1f);
                        temp -= 1f;
                        break;
                    case 4:
                        GainGold(3);
                        temp -= 0.6f;
                        break;
                    case 5:
                        GainTools(1);
                        temp -= 0.6f;
                        break;
                    case 6:
                        cooldownReduction += 0.004f;
                        temp -= 0.3f;
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
        toolsStored = tools;
        eq.guns[eq.equipped].parts = toolsStored;

        UpdateBars();
    }

    void Update()
    {
        if (free)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                DashCast();

            GetInput();
            move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Movement();
            Aim();
            if (Input.GetMouseButtonDown(1))
                GrenadeCast();
            if (Input.GetKeyDown(KeyCode.V))
                DrinkPotion();
            if (task <= 0)
            {
                Action();
            }
            else
            {
                task -= Time.deltaTime * SpeedMultiplyer(1f);
                taskImage.fillAmount = 1 - (task / taskMax);
                if (Input.GetMouseButtonDown(0) && reloading && eq.guns[eq.equipped].bulletsLeft > 0)
                {
                    reloading = false;
                    NewTask(0.1f);
                    Shoot(0f);
                    task += eq.guns[eq.equipped].fireRate;
                }
            }
        }
        else move = new Vector2(0, 0);

        if (flashA > 0f)
        {
            flashA -= 0.8f * Time.deltaTime;
            if (greenF)
                damageFlash.color = new Color(0f, 1f, 0f, flashA);
            else damageFlash.color = new Color(1f, 0f, 0f, flashA);
            if (flashA <= 0f)
                DamageFlash.SetActive(false);
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

    void UpdateBars()
    {
        healthBack.sizeDelta = new Vector2(maxHealth * 3 + 20, 45);
        healthFill.sizeDelta = new Vector2(maxHealth * 3, 30);
        healthDrop.sizeDelta = new Vector2(maxHealth * 3, 30);
        shieldBack.sizeDelta = new Vector2(maxShield * 2 + 20, 30);
        shieldFill.sizeDelta = new Vector2(maxShield * 2, 15);
        shieldDrop.sizeDelta = new Vector2(maxShield * 2, 15);
    }

    void FixedUpdate()
    {
        Body.velocity = move * (movementSpeed + dash) * Time.deltaTime;
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
        dash = 720f + 0.72f * movementSpeed;
        Invoke("Dashed", 0.12f);

        invulnerable = true;
        playerSprite.color = new Color(0.4f, 0.4f, 1f, 1f);
        Invoke("Recovered", 0.2f);

        if (eq.Items[13])
        {
            tempi = (eq.guns[eq.equipped].MagazineTotalSize() * 5) / 20;

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
        dash = 0f;

        if (eq.guns[eq.equipped].Accessories[19] > 0 || eq.guns[eq.equipped].Accessories[19 + bp.ALibrary.count] > 0)
            Invoke("DashFire", 0.07f);

        if (eq.Items[9])
        {
            movementSpeed += 100f;
            Invoke("SprintEnd", 2f);
        }
    }

    void DashFire()
    {
        temp = 0;
        temp += 0.9f * eq.guns[eq.equipped].Accessories[19] * SpeedMultiplyer(1f);
        temp += 1.44f * eq.guns[eq.equipped].Accessories[19 + bp.ALibrary.count] * SpeedMultiplyer(1f);
        tempi2 = 0;

        for (float f = 0; f <= temp; f += eq.guns[eq.equipped].fireRate)
        {
            tempi2++;
        }

        for (int i = 0; i < tempi2; i++)
        {
            FireDirection((i * 2 - tempi2 + 1) * (5f / (1f + 0.05f * tempi2)), 0f);
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
        GunRot.localScale = new Vector3(1f, 1f, 1f);
        Dude.rotation = new Quaternion(0, 0, 0, 0);
        if (Gun.rotation > 0f || Gun.rotation < -180f)
        {
            GunRot.localScale = new Vector3(-1f, 1f, 1f);
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
                if (eq.guns[eq.equipped].Accessories[21] > 0 || eq.guns[eq.equipped].Accessories[21 + bp.ALibrary.count] > 0)
                {
                    temp = 1f + 0.45f * (eq.guns[eq.equipped].Accessories[21] + 1.6f * eq.guns[eq.equipped].Accessories[21 + bp.ALibrary.count]) * (eq.guns[eq.equipped].MagazineTotalSize() - eq.guns[eq.equipped].bulletsLeft) / eq.guns[eq.equipped].MagazineTotalSize();
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
            if (eq.Items[24] && Random.Range(0f, 1f) >= 0.825f - 0.007f * luck)
                Fire();
            if (!eq.guns[eq.equipped].infiniteMagazine)
            {
                if (eq.guns[eq.equipped].Accessories[14] * 0.2f + eq.guns[eq.equipped].Accessories[14 + bp.ALibrary.count] * 0.32f < Random.Range(0f, 1f))
                    eq.guns[eq.equipped].bulletsLeft--;
                DisplayAmmo();
            }
        }
        else
        {
            if (eq.guns[eq.equipped].Accessories[14] * 0.2f + eq.guns[eq.equipped].Accessories[14 + bp.ALibrary.count] * 0.32f < Random.Range(0f, 1f))
            {
                Fire();
                if (eq.Items[24] && Random.Range(0f, 1f) >= 0.83f - 0.005f * luck)
                    Fire();
            }
        }
    }

    public void Shoot(float accuracy_change = 0f)
    {
        if (eq.Items[24] && Random.Range(0f, 1f) >= 0.825f - 0.007f * luck)
            Fire(accuracy_change);

        Fire(accuracy_change);

        if (!eq.guns[eq.equipped].infiniteMagazine)
        {
            if (eq.guns[eq.equipped].Accessories[14] * 0.2f + eq.guns[eq.equipped].Accessories[14 + bp.ALibrary.count] * 0.32f < Random.Range(0f, 1f))
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
                GameObject bullet = Instantiate(SetBulletPrefab(), Barrel.position, Barrel.rotation);
                Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
                firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
                SetBullet(1f);
                bullet_body.AddForce(Barrel.up * firedBullet.force, ForceMode2D.Impulse);
            }
        }
        if (eq.guns[eq.equipped].Accessories[15] * 0.2f + eq.guns[eq.equipped].Accessories[15 + bp.ALibrary.count] * 0.32f >= Random.Range(0f, 1f))
        {
            FireDirection(-32f, accuracy_change);
            FireDirection(32f, accuracy_change);
        }

        if (eq.Items[17])
            eq.OnHit(1.2f + 0.3f * (eq.guns[eq.equipped].critChance + additionalCritChance));
        else eq.OnHit(1f);
    }

    public void FireDirection(float direction, float accuracy_change = 0f)
    {
        for (int i = 0; i < eq.guns[eq.equipped].BulletsFired(); i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + direction + Random.Range(-eq.guns[eq.equipped].accuracy - accuracy_change, eq.guns[eq.equipped].accuracy + accuracy_change));
            GameObject bullet = Instantiate(SetBulletPrefab(), Barrel.position, Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet(1f);
            if (eq.guns[eq.equipped].targetArea && Vector3.Distance(transform.position, new Vector2(mousePos[0], mousePos[1])) <= eq.guns[eq.equipped].range * 24f)
                bullet_body.AddForce(Barrel.up * firedBullet.force * (Vector3.Distance(transform.position, new Vector2(mousePos[0], mousePos[1])) / (eq.guns[eq.equipped].range * 24f)), ForceMode2D.Impulse);
            else bullet_body.AddForce(Barrel.up * firedBullet.force, ForceMode2D.Impulse);
        }
    }

    public GameObject SetBulletPrefab()
    {
        tempi = 0;
        /*for (int i = 0; i < 3; i++)
        {
            if (eq.guns[eq.equipped].specialBulletChance[i] > Random.Range(0f, 1f))
                tempi += (2 ^ i) - 1;
        }*/
        if (eq.Items[20])
            temp = 0.85f - 0.01f * luck;
        else temp = 1f;

        temp2 = 1f + eq.guns[eq.equipped].critChance * (0.06f * eq.guns[eq.equipped].Accessories[34]) + (0.096f * eq.guns[eq.equipped].Accessories[34 + bp.ALibrary.count]);

        if ((eq.guns[eq.equipped].specialBulletChance[0] + eq.guns[eq.equipped].specialBulletNextChance[0]) * temp2 > Random.Range(0f, temp))
        {
            effectOn[0] = true;
            tempi += 1;
            eq.guns[eq.equipped].specialBulletNextChance[0] = 0f;
        }
        else 
        {
            effectOn[0] = false;
            eq.guns[eq.equipped].specialBulletNextChance[0] += 0.05f * eq.guns[eq.equipped].Accessories[35] + 0.08f * eq.guns[eq.equipped].Accessories[35 + bp.ALibrary.count];
        }

        if ((eq.guns[eq.equipped].specialBulletChance[1] + eq.guns[eq.equipped].specialBulletNextChance[1]) * temp2> Random.Range(0f, temp))
        {
            effectOn[1] = true;
            tempi += 2;
            eq.guns[eq.equipped].specialBulletNextChance[1] = 0f;
        }
        else 
        {
            effectOn[1] = false;
            eq.guns[eq.equipped].specialBulletNextChance[1] += 0.05f * eq.guns[eq.equipped].Accessories[35] + 0.08f * eq.guns[eq.equipped].Accessories[35 + bp.ALibrary.count];
        }

        if ((eq.guns[eq.equipped].specialBulletChance[2] + eq.guns[eq.equipped].specialBulletNextChance[2]) * temp2> Random.Range(0f, temp))
        {
            effectOn[2] = true;
            tempi += 4;
            eq.guns[eq.equipped].specialBulletNextChance[2] = 0f;
        }
        else 
        {
            effectOn[2] = false;
            eq.guns[eq.equipped].specialBulletNextChance[2] += 0.05f * eq.guns[eq.equipped].Accessories[35] + 0.08f * eq.guns[eq.equipped].Accessories[35 + bp.ALibrary.count];
        }

        if ((eq.guns[eq.equipped].specialBulletChance[3] + eq.guns[eq.equipped].specialBulletNextChance[3]) * temp2 > Random.Range(0f, temp))
        {
            effectOn[3] = true;
            tempi += 8;
            eq.guns[eq.equipped].specialBulletNextChance[3] = 0f;
        }
        else
        {
            effectOn[3] = false;
            eq.guns[eq.equipped].specialBulletNextChance[3] += 0.05f * eq.guns[eq.equipped].Accessories[35] + 0.08f * eq.guns[eq.equipped].Accessories[35 + bp.ALibrary.count];
        }

        if (eq.guns[eq.equipped].Accessories[36] > 0 || eq.guns[eq.equipped].Accessories[36 + bp.ALibrary.count] > 0)
        {
            temp3 = 0f;
            for (int i = 0; i < 3; i++)
            {
                if (effectOn[i])
                    temp3 += eq.guns[eq.equipped].Accessories[36] * 0.2f + eq.guns[eq.equipped].Accessories[36 + bp.ALibrary.count] * 0.32f;
            }

            if (!effectOn[0])
            {
                if (temp3 * temp2 > Random.Range(0f, temp))
                    tempi += 1;
            }

            if (!effectOn[1])
            {
                if (temp3 * temp2 > Random.Range(0f, temp))
                    tempi += 2;
            }

            if (!effectOn[2])
            {
                if (temp3 * temp2 > Random.Range(0f, temp))
                    tempi += 4;
            }

            if (!effectOn[3])
            {
                if (temp3 * temp2 > Random.Range(0f, temp))
                    tempi += 8;
            }
        }

        if (tempi > 0)
            CurrentBullet = SpecialBullets[tempi];
        else CurrentBullet = eq.guns[eq.equipped].bulletPrefab[Random.Range(0, eq.guns[eq.equipped].bulletPrefab.Length)];

        return CurrentBullet;
    }

    public void SetBullet(float efficiency)
    {
        firedBullet.falloff = eq.guns[eq.equipped].range / forceIncrease;
        firedBullet.duration = (0.5f + eq.guns[eq.equipped].range * 2f) / forceIncrease;
        firedBullet.force = eq.guns[eq.equipped].force * forceIncrease * Random.Range(1.02f, 1.08f);
        firedBullet.mass = eq.guns[eq.equipped].heft;
        firedBullet.damage = eq.guns[eq.equipped].Damage() * DamageDealtMultiplyer(1f);
        if (eq.guns[eq.equipped].Accessories[16] > 0)
            firedBullet.damage *= 1f + (0.004f * eq.guns[eq.equipped].Damage() * eq.guns[eq.equipped].Accessories[16]);
        if (eq.guns[eq.equipped].Accessories[16 + bp.ALibrary.count] > 0)
            firedBullet.damage *= 1f + (0.0064f * eq.guns[eq.equipped].Damage() * eq.guns[eq.equipped].Accessories[16 + bp.ALibrary.count]);
        if (eq.guns[eq.equipped].Accessories[22] > 0)
            firedBullet.damage *= 1f + (0.008f * eq.guns[eq.equipped].MagazineTotalSize() * eq.guns[eq.equipped].Accessories[22]);
        if (eq.guns[eq.equipped].Accessories[22 + bp.ALibrary.count] > 0)
            firedBullet.damage *= 1f + (0.0128f * eq.guns[eq.equipped].MagazineTotalSize() * eq.guns[eq.equipped].Accessories[22 + bp.ALibrary.count]);
        firedBullet.DoT = eq.guns[eq.equipped].DoT;
        firedBullet.shatter = eq.guns[eq.equipped].shatter;
        firedBullet.incendiary = eq.guns[eq.equipped].incendiary;
        firedBullet.curse = eq.guns[eq.equipped].curse;
        firedBullet.damageGain = eq.guns[eq.equipped].damageGain;
        firedBullet.armorShred = eq.guns[eq.equipped].armorShred;
        firedBullet.vulnerableApplied = eq.guns[eq.equipped].vulnerableApplied;
        firedBullet.slowDuration = eq.guns[eq.equipped].slowDuration;
        firedBullet.stunDuration = eq.guns[eq.equipped].stunDuration;
        firedBullet.pierce = eq.guns[eq.equipped].pierce;
        firedBullet.pierceEfficiency = eq.guns[eq.equipped].pierceEfficiency;
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
            {
                firedBullet.pierce += eq.guns[eq.equipped].Accessories[8];
                firedBullet.pierceEfficiency += 0.1f * eq.guns[eq.equipped].Accessories[8];
            }
            if (eq.guns[eq.equipped].Accessories[8 + bp.ALibrary.count] > 0)
            {
                firedBullet.pierce += eq.guns[eq.equipped].Accessories[8 + bp.ALibrary.count];
                firedBullet.pierceEfficiency += 0.15f * eq.guns[eq.equipped].Accessories[8 + bp.ALibrary.count];
            }
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
        firedBullet.duration = 0.8f / forceIncrease;
        firedBullet.falloff = 0.8f / forceIncrease;
        firedBullet.damage = (32.5f + /*toolsStored * 0.12f +*/ level * 1.5f) * DamageDealtMultiplyer(1.1f);
        if (eq.Items[15])
            firedBullet.damage *= 1.24f;
        if (eq.Items[28])
            firedBullet.shatter += 0.91f;
        if (eq.Items[29])
            Effects.venom = true;
        if (eq.Items[30])
            Effects.small = true;
        if (eq.Items[31])
            firedBullet.duration /= DamageDealtMultiplyer(0.4f);
    }

    float ThrowRange()
    {
        if (eq.Items[31])
            return throwRange * DamageDealtMultiplyer(0.4f);
        else return throwRange;
    }

    void DrinkPotion()
    {
        if (potions > 0 && (health < maxHealth || eq.Items[34]))
        {
            temp = 10f;
            if (eq.Items[36])
                GainHP(2);
            if (eq.Items[6])
                temp += maxHealth * 0.1f;
            if (eq.Items[37])
                temp += 5f;
            if (eq.Items[34])
            {
                if (temp > maxHealth - health)
                {
                    RestoreHealth(maxHealth - health);
                    GainShield(temp + health - maxHealth);
                }
            }
            RestoreHealth(temp);
            potions--;
            potionsInfo.text = potions.ToString("0") + "/" + maxPotions.ToString("0");
        }
    }

    void Rain()
    {
        if (!day)
        {
            if (eq.guns[eq.equipped].burst > 0)
            {
                for (int i = 0; i < eq.guns[eq.equipped].burst; i++)
                {
                    FireDirection(Random.Range(0f, 360f), 0f);
                }
            }
            FireDirection(Random.Range(0f, 360f), 0f);
            Invoke("Rain", eq.guns[eq.equipped].fireRate * 2.62f / SpeedMultiplyer(1.41f));
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
                    eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].Accessories[10] * eq.guns[eq.equipped].MagazineTotalSize() / 6;
                if (eq.guns[eq.equipped].Accessories[10 + bp.ALibrary.count] > 0)
                    eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].Accessories[10 + bp.ALibrary.count] * eq.guns[eq.equipped].MagazineTotalSize() / 4;
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
                eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].Accessories[10] * eq.guns[eq.equipped].MagazineTotalSize() / 6;
                eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].Accessories[10 + bp.ALibrary.count] * eq.guns[eq.equipped].MagazineTotalSize() / 4;
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
            eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].Accessories[10] * eq.guns[eq.equipped].MagazineTotalSize() / 6;
            eq.guns[eq.equipped].bulletsLeft += eq.guns[eq.equipped].Accessories[10 + bp.ALibrary.count] * eq.guns[eq.equipped].MagazineTotalSize() / 4;
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
        eq.equippedGun.sprite = eq.guns[eq.equipped].holdingSprite;
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
        movementSpeed -= 100f;
    }

    public void TakeDamage(float value, bool pierce)
    {
        if (!invulnerable)
        {
            Damaged();
            if (protection)
                protection = false;
            else
            {
                if (eq.Items[19])
                    value *= 1.24f;
                if (eq.Items[0])
                {
                    if (undamaged)
                    {
                        undamaged = false;
                        movementSpeed -= 44f;
                        fireRateBonus -= 0.14f;
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
                        wrath += value * 0.0032f;
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
                            wrath += value * 0.0032f;
                    }
                    shieldBar.fillAmount = shield / maxShield;
                }

                healthInfo.text = health.ToString("0") + "/" + maxHealth.ToString("0");
                ShieldInfo.text = shield.ToString("0") + "/" + maxShield.ToString("0");

                if (health <= 0f)
                    ReturnToMenu();
            }
        }
    }

    void Damaged()
    {
        greenF = false;
        invulnerable = true;
        playerSprite.color = new Color(1f, 0f, 0f, 1f);
        flashA = 0.35f;
        DamageFlash.SetActive(true);
        damageFlash.color = new Color(1f, 0f, 0f, flashA);
        Invoke("Recovered", 0.22f);
    }

    void Healed()
    {
        greenF = true;
        flashA = 0.3f;
        DamageFlash.SetActive(true);
        damageFlash.color = new Color(0f, 1f, 0f, flashA);
    }

    void Recovered()
    {
        invulnerable = false;
        playerSprite.color = new Color(1f, 1f, 1f, 1f);
    }

    public void GainPoison(float value)
    {
        poison += value;
        while (poison >= 100f)
        {
            poison -= 100f;
            TakeDamage(toxicityLevel, true);
            GainToxicity(1);
        }
        posionBar.fillAmount = poison / 100f;
        /*poison += value;

        if (poison >= value * 3f)
            TakeDamage(value * 3f, true);
        else TakeDamage(poison, true);*/
    }

    public void GainToxicity(int amount)
    {
        toxicityLevel += 1;
        ToxicityInfo.text = toxicityLevel.ToString("0");
    }

    public void RestoreHealth(float value)
    {
        Healed();
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
        if (eq.Items[11])
            ShieldCapacitor(value);
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
        if (eq.Items[19])
            temp *= 1.36f;
        temp *= 1f + (temp - 1f) * efficiency;
        return temp;
    }

    public void Collided(Collider2D other)
    {
        if (other.transform.tag == "Scrap")
        {
            GainGold(1);
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "Scrap5")
        {
            GainGold(5);
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "Tools")
        {
            GainTools(1);

            Destroy(other.gameObject);
        }
        if (other.transform.tag == "Potion")
        {
            if (potions < maxPotions)
            {
                GainPotions(1);
                Destroy(other.gameObject);
            }
        }
        else if (other.transform.tag == "Medkit")
        {
            RestoreHealth(10f + maxHealth * 0.16f);
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "Accessory")
        {
            eq.Accessories[Random.Range(0, bp.ALibrary.count)]++;
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
        else if (other.transform.tag == "NeutralProjectal")
        {
            collidedBullet = other.GetComponent(typeof(EnemyBullet)) as EnemyBullet;
            TakeDamage(collidedBullet.damage, false);
            GainPoison(collidedBullet.poison);
            //Destroy(other.gameObject);
        }
    }

    public void EnemySlained()
    {
        if (eq.Items[10])
        {
            bloodMoney++;
            while (bloodMoney >= 5)
            {
                bloodMoney -= 5;
                GainGold(4);
            }
        }
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
        AmmoRefill();
        dayCount++;
        //RestoreHealth(40 + maxHealth * 0.5f);
        wrath = 0;
        LevelUp();
    }

    public void Nightfall()
    {
        day = false;
        AmmoRefill();
        eq.ActivateItems();
        if (eq.Items[0])
        {
            if (!undamaged)
            {
                undamaged = true;
                movementSpeed += 44f;
                fireRateBonus += 0.14f;
            }
        }
        if (eq.Items[2])
            Invoke("Rain", 0.2f);
        if (eq.Items[11])
        {
            shield = maxShield;
            GainShield(0f);
        }
        //if (eq.Items[34])
            //protection = true;
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
                    eq.guns[i].ammo += (eq.guns[i].maxAmmo * 3) / 20;
                if (eq.guns[i].Accessories[18] > 0)
                    eq.guns[i].ammo += (eq.guns[i].maxAmmo * eq.guns[i].Accessories[18]) / 5;
                if (eq.guns[i].Accessories[18 + bp.ALibrary.count] > 0)
                    eq.guns[i].ammo += (eq.guns[i].maxAmmo * eq.guns[i].Accessories[18 + bp.ALibrary.count * 8]) / 25;
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
            GainDMG(0.0012f * value);
        dHealth += value;
        UpdateBars();
        dropBar.fillAmount = dHealth / maxHealth;
        healthBar.fillAmount = health / maxHealth;
    }

    public void GainDMG(float value)
    {
        damageBonus += value;
    }

    public void GainFR(float value)
    {
        fireRateBonus += value;
        if (eq.Items[32])
            GainCR(value / 2.8f);
    }

    public void GainCR(float value)
    {
        if (eq.Items[5])
            value *= 2;
        cooldownReduction += value;
    }

    public void GainMS(float value)
    {
        movementSpeed += value * 4f; //value = procent, 4f as in base 400 movement speed 
    }

    public void GainSC(float value)
    {
        if (eq.Items[11])
            value *= 0.3f;
        maxShield += value;
        UpdateBars();
        GainShield(value);
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
        if (eq.Items[35])
        {
            builtShield += amount;
            while (builtShield >= 5)
            {
                builtShield -= 5;
                GainShield(6);
            }
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

    public void GainPotions(int amount)
    {
        potions += amount;
        if (potions > maxPotions)
            potions = maxPotions;
        potionsInfo.text = potions.ToString("0") + "/" + maxPotions.ToString("0");
    }

    public void GainPotionSlots(int amount)
    {
        maxPotions += amount;
        potionsInfo.text = potions.ToString("0") + "/" + maxPotions.ToString("0");
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

    public void Item11()
    {
        maxShield *= 0.3f;
        ShieldCapacitor(shield);
        shield = maxShield;
        UpdateBars();
        GainShield(0);
    }

    public void ShieldCapacitor(float amount)
    {
        shieldCapacitor += amount;
        while (shieldCapacitor >= 15f)
        {
            shieldCapacitor -= 15f;
            maxShield += 1f;
            UpdateBars();
            GainShield(0f);
        }
    }

    public void Item34()
    {
        if (maxHealth > 5)
        {
            temp = maxHealth - 5;
            maxHealth = 5;
            if (eq.Items[7])
                GainDMG(-0.0012f * temp);
            maxShield += temp * 1.1f;
        }

        if (health > 5)
        {
            temp = health - 5;
            health = 5;
            shield += temp * 1.2f;
        }

        UpdateBars();
        healthBar.fillAmount = health / maxHealth;
        healthInfo.text = health.ToString("0") + "/" + maxHealth.ToString("0");
        shieldBar.fillAmount = shield / maxShield;
        ShieldInfo.text = shield.ToString("0") + "/" + maxShield.ToString("0");
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
        bp.OpenBackpack();
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
        bp.CloseBackpack();
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