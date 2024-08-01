using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerControllerLobby : MonoBehaviour
{
    public PlayerController Player;

    [Header("Classes")]
    public int currentClass;
    public Gun[] guns;
    public Sprite[] sprites;
    public GameObject[] classSprites;
    public GameObject[] classesToSelect, classesTab;

    public Transform Barrel, Hand, Dude, GunRot, TargetArea;
    public Rigidbody2D Body, Gun;
    public TMPro.TextMeshProUGUI magazineInfo, ammoInfo, GrenadeCharge;
    public Image taskImage, dashImage, abilityImage, gunImage;
    public Bullet firedBullet;
    public GameObject Grenade, CurrentBullet;
    public GameObject[] SpecialBullets;

    Vector2 move;
    public bool mouseLeft, reloading, free = true;
    public Vector3 mousePos, mouseVector;
    CameraController Cam;
    public float task, taskMax;

    [Header("Stats")]
    public float damageBonus;
    public float fireRateBonus, movementSpeed, cooldownReduction, dashBaseCooldown,
    maxDashCooldown, dashCooldown, grenadeMaxCharges, grenadeCharges, throwRange, grenadeBaseCooldown, grenadeMaxCooldown, grenadeCooldown, dash;
    public int tempi;
    float temp, flashA;
    bool greenF;

    [Header("TAB")]
    public GameObject TabScreen;

    public SpriteRenderer playerSprite, heldGunSprite;

    // -- animacje --
    public Animator[] animators;
    public float moveSpeed = 5f;

    // -- Huds --
    public GameObject Menu;
    public bool menuOpened, tabOpened;

    void Start()
    {
        GetMouseInput();
        Cam = FindObjectOfType<CameraController>();
        DisplayAmmo();
        dash = 0f;
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
            if (task <= 0)
            {
                Action();
            }
            else
            {
                task -= Time.deltaTime * SpeedMultiplyer(1f);
                taskImage.fillAmount = 1 - (task / taskMax);
                if (mouseLeft && reloading && guns[currentClass].bulletsLeft > 0)
                {
                    reloading = false;
                    NewTask(0.1f);
                    Shoot(0f);
                    task += guns[currentClass].fireRate;
                }
            }
        }
        else move = new Vector2(0, 0);

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
    }

    void FixedUpdate()
    {
        Body.velocity = move * (movementSpeed + dash) * Time.deltaTime;
    }

    void DashCast()
    {
        if (dashCooldown <= 0)
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
    }

    void Dashed()
    {
        dash = 0f;
    }

    public void NewTask(float duration)
    {
        taskMax = duration;
        task += duration;
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
            animators[currentClass].SetFloat("moveSpeed", Mathf.Abs(Input.GetAxis("Horizontal")));
        }
        else if (Input.GetAxis("Vertical") != 0)
        {
            animators[currentClass].SetFloat("moveSpeed", Mathf.Abs(Input.GetAxis("Vertical")));
        }
        else
        {
            animators[currentClass].SetFloat("moveSpeed", 0f);
        }
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
            if (guns[currentClass].bulletsLeft > 0 || guns[currentClass].infiniteMagazine)
            {
                Shoot();
                if (guns[currentClass].burst > 0)
                {
                    for (int i = 0; i < guns[currentClass].burst; i++)
                    {
                        Invoke("BurstShot", guns[currentClass].burstDelay * (i + 1));
                    }
                }
                NewTask(guns[currentClass].fireRate);
            }
            else Reload();
        }
        else if (Input.GetKeyDown(KeyCode.R))
            Reload();
    }

    void BurstShot()
    {
        if (guns[currentClass].bulletsLeft > 0 || guns[currentClass].infiniteMagazine)
        {
            Fire();

            if (!guns[currentClass].infiniteMagazine)
            {
                guns[currentClass].bulletsLeft--;
                DisplayAmmo();
            }
        }
    }

    public void Shoot(float accuracy_change = 0f)
    {
        Fire(accuracy_change);

        if (!guns[currentClass].infiniteMagazine)
            guns[currentClass].bulletsLeft--;
        DisplayAmmo();

        Cam.Shake((transform.position - Barrel.position).normalized, guns[currentClass].cameraShake, guns[currentClass].shakeDuration);
    }

    public void Fire(float accuracy_change = 0f)
    {
        for (int i = 0; i < guns[currentClass].BulletsFired(); i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + Random.Range(-guns[currentClass].accuracy - accuracy_change, guns[currentClass].accuracy + accuracy_change));
            GameObject bullet = Instantiate(SetBulletPrefab(), Barrel.position, Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet(1f);
            bullet_body.AddForce(Barrel.up * firedBullet.force, ForceMode2D.Impulse);
        }
    }

    public void FireDirection(float direction, float accuracy_change = 0f)
    {
        for (int i = 0; i < guns[currentClass].BulletsFired(); i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Gun.rotation + direction + Random.Range(-guns[currentClass].accuracy - accuracy_change, guns[currentClass].accuracy + accuracy_change));
            GameObject bullet = Instantiate(SetBulletPrefab(), Barrel.position, Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            SetBullet(1f);
            if (guns[currentClass].targetArea && Vector3.Distance(transform.position, new Vector2(mousePos[0], mousePos[1])) <= guns[currentClass].range * 24f)
                bullet_body.AddForce(Barrel.up * firedBullet.force * (Vector3.Distance(transform.position, new Vector2(mousePos[0], mousePos[1])) / (guns[currentClass].range * 24f)), ForceMode2D.Impulse);
            else bullet_body.AddForce(Barrel.up * firedBullet.force, ForceMode2D.Impulse);
        }
    }

    public GameObject SetBulletPrefab()
    {
        tempi = 0;
        /*for (int i = 0; i < 3; i++)
        {
            if (guns[currentClass].specialBulletChance[i] > Random.Range(0f, 1f))
                tempi += (2 ^ i) - 1;
        }*/
        if (guns[currentClass].specialBulletChance[0] > Random.Range(0f, 1f))
            tempi += 1;
        if (guns[currentClass].specialBulletChance[1] > Random.Range(0f, 1f))
            tempi += 2;
        if (guns[currentClass].specialBulletChance[2] > Random.Range(0f, 1f))
            tempi += 4;
        if (guns[currentClass].specialBulletChance[3] > Random.Range(0f, 1f))
            tempi += 8;

        if (tempi > 0)
            CurrentBullet = SpecialBullets[tempi - 1];
        else CurrentBullet = guns[currentClass].bulletPrefab[Random.Range(0, guns[currentClass].bulletPrefab.Length)];

        return CurrentBullet;
    }

    public void SetBullet(float efficiency)
    {
        firedBullet.falloff = guns[currentClass].range;
        firedBullet.duration = (0.5f + guns[currentClass].range * 2f);
        firedBullet.force = guns[currentClass].force * Random.Range(1.02f, 1.08f);
        firedBullet.mass = guns[currentClass].heft;
        firedBullet.damage = guns[currentClass].Damage() * DamageDealtMultiplyer(1f);
        firedBullet.DoT = guns[currentClass].DoT;
        firedBullet.shatter = guns[currentClass].shatter;
        //firedBullet.burn = guns[currentClass].incendiary;
        firedBullet.curse = guns[currentClass].curse;
        firedBullet.damageGain = guns[currentClass].damageGain;
        firedBullet.vulnerableApplied = guns[currentClass].vulnerableApplied;
        firedBullet.slowDuration = guns[currentClass].slowDuration;
        firedBullet.stunDuration = guns[currentClass].stunDuration;
        firedBullet.pierce = guns[currentClass].pierce;
        firedBullet.pierceEfficiency = guns[currentClass].pierceEfficiency;
        firedBullet.special = guns[currentClass].special;

        if (guns[currentClass].critChance >= Random.Range(0f, 1f))
        {
            firedBullet.damage *= guns[currentClass].critDamage;
            firedBullet.vulnerableApplied *= 0.6f + guns[currentClass].critDamage * 0.4f;
            firedBullet.slowDuration *= 0.7f + guns[currentClass].critDamage * 0.3f;
            firedBullet.mass *= 0.8f + guns[currentClass].critDamage * 0.5f;
            firedBullet.stunDuration *= 0.7f + guns[currentClass].critDamage * 0.3f;
            firedBullet.pierceEfficiency *= 1.1f;
            firedBullet.crit = true;
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
        if (Vector3.Distance(transform.position, new Vector2(mousePos[0], mousePos[1])) <= throwRange)
            TargetArea.position = new Vector2(mousePos[0], mousePos[1]);
        else
        {
            temp = Vector3.Distance(transform.position, new Vector2(mousePos[0], mousePos[1])) / throwRange;
            TargetArea.position = new Vector2(transform.position.x + (mousePos[0] - transform.position.x) / temp, transform.position.y + (mousePos[1] - transform.position.y) / temp);
        }
        GameObject bullet = Instantiate(Grenade, Barrel.position, Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        firedBullet.TargetedLocation = TargetArea;
        firedBullet.damage = 32.5f * DamageDealtMultiplyer(1.1f);
    }

    void Reload()
    {
        if (!guns[currentClass].infiniteMagazine && guns[currentClass].bulletsLeft < guns[currentClass].MagazineTotalSize())
        {
            if (guns[currentClass].infiniteAmmo || guns[currentClass].ammo > 0)
            {
                reloading = true;
                NewTask(guns[currentClass].reloadTime);
            }
        }
    }

    void Reloaded()
    {
        if (guns[currentClass].individualReload)
        {
            if (guns[currentClass].infiniteAmmo)
            {
                guns[currentClass].bulletsLeft += guns[currentClass].individualReloadCount;
                if (guns[currentClass].bulletsLeft > guns[currentClass].MagazineTotalSize())
                    guns[currentClass].bulletsLeft = guns[currentClass].MagazineTotalSize();
                guns[currentClass].bulletsLeft += guns[currentClass].overload;
                if (guns[currentClass].bulletsLeft >= guns[currentClass].MagazineTotalSize())
                    reloading = false;
                else
                {
                    NewTask(guns[currentClass].reloadTime);
                }
            }
            else
            {
                guns[currentClass].bulletsLeft += guns[currentClass].individualReloadCount;
                guns[currentClass].ammo -= guns[currentClass].individualReloadCount;
                if (guns[currentClass].bulletsLeft > guns[currentClass].MagazineTotalSize())
                {
                    guns[currentClass].ammo += guns[currentClass].bulletsLeft -= guns[currentClass].MagazineTotalSize();
                    guns[currentClass].bulletsLeft = guns[currentClass].MagazineTotalSize();
                }
                guns[currentClass].bulletsLeft += guns[currentClass].overload;
                if (guns[currentClass].bulletsLeft >= guns[currentClass].MagazineTotalSize() || guns[currentClass].ammo <= 0)
                    reloading = false;
                else
                {
                    NewTask(guns[currentClass].reloadTime);
                }
            }
        }
        else
        {
            if (guns[currentClass].infiniteAmmo)
            {
                guns[currentClass].bulletsLeft = guns[currentClass].MagazineTotalSize();
            }
            else if (guns[currentClass].ammo >= guns[currentClass].MagazineTotalSize() - guns[currentClass].bulletsLeft)
            {
                guns[currentClass].ammo -= (guns[currentClass].MagazineTotalSize() - guns[currentClass].bulletsLeft);
                guns[currentClass].bulletsLeft = guns[currentClass].MagazineTotalSize();
            }
            else
            {
                guns[currentClass].bulletsLeft += guns[currentClass].ammo;
                guns[currentClass].ammo = 0;
            }
            guns[currentClass].bulletsLeft += guns[currentClass].overload;
            reloading = false;
        }
        DisplayAmmo();
    }

    public void DisplayAmmo()
    {
        magazineInfo.text = (guns[currentClass].bulletsLeft).ToString("") + "/" + guns[currentClass].MagazineTotalSize();
        ammoInfo.text = (guns[currentClass].ammo).ToString("");
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
        temp *= 1f + (temp - 1f) * efficiency;
        return temp;
    }

    void OpenMenu()
    {
        Menu.SetActive(true);
        menuOpened = true;
        free = false;
        Player.free = false;
    }

    public void CloseMenu()
    {
        Menu.SetActive(false);
        menuOpened = false;
        free = true;
        Player.free = true;
    }

    void OpenTab()
    {
        TabScreen.SetActive(true);
        tabOpened = true;
        free = false;
        Player.free = false;
    }

    public void ItemTooltipOpen(int which)
    {
        //eq.Tooltip.text = eq.ILibrary.ItemTooltip[eq.ItemList[which]];
    }

    public void ItemTooltipClose()
    {
        //eq.Tooltip.text = "";
    }

    public void CloseTab()
    {
        ItemTooltipClose();
        TabScreen.SetActive(false);
        tabOpened = false;
        free = true;
        Player.free = true;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void ChangeClass(int which)
    {
        classesToSelect[currentClass].SetActive(true);
        classSprites[currentClass].SetActive(false);
        currentClass = which;
        TabScreen = classesTab[which];
        classSprites[currentClass].SetActive(true);
        //playerSprite.sprite = sprites[which];
        guns[currentClass].bulletsLeft = guns[currentClass].MagazineTotalSize();
        gunImage.sprite = guns[currentClass].gunSprite;
        heldGunSprite.sprite = guns[currentClass].holdingSprite;
        DisplayAmmo();
    }

    public void EnterElevator()
    {
        PlayerPrefs.SetInt("Class", currentClass);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}