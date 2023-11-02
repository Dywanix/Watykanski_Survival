using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyState
{
    Chase,

    Attack
};

public class Enemy : MonoBehaviour
{
    public EnemyState CurrentState = EnemyState.Chase;
    public GameObject Player, Scrap, damageTook, Projectal;
    public GameObject[] Items;
    public PlayerController playerStats;
    public Rigidbody2D Body, playerBody, Dir;
    public Transform Sight, DamageOrigin, FlankPosition, Push;
    private Bullet collidedBullet, firedBullet;
    private DamageTaken damageDisplay;
    public Day_Night_Cycle day_night;

    // ----- enemy stats -----
    private int roll, tempi;
    private float temp;
    public bool rare, boss, dead;

    [Header("Health & Resistance")]
    public int weight;
    public Image healthFill, DoTFill, ShieldFill;
    public float maxHealth, health, regen, armor, vulnerable;
    public float shieldCapacity, shield, rechargeTimer, shieldRechargeRate, shieldRechargeDelay;

    [Header("Movement")]
    public float movementSpeed;
    public float aimingMovement, tenacity, stun;
    public bool flank;

    [Header("Damage & Attacks")]
    public float attackTimer;
    public float attackDamage, attackPoison, attackSpeed, attackRange;

    [Header("Ranged Stuff")]
    public bool ranged = false;
    public bool throws = false;
    public int bulletCount, bulletBurst;
    public float bulletSpread, burstDelay, accuracy, force, minRange;
    float recoil, currentForce;

    [Header("Dropy")]
    public LeftOver DeathDrop;
    public float scrapChance, itemChance;
    public int scrapCount, itemCount;

    [Header("Graficzne")]
    public GameObject Blood;

    [Header("Status")]
    public GameObject FireExplosion;
    public GameObject CurseBullet;
    public bool burning, slowed;
    public float slow, DoT, ablaze, curse;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerBody = Player.GetComponent<Rigidbody2D>();
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;

        /*maxHealth *= Random.Range(0.96f, 1.04f);
        armor *= Random.Range(0.98f, 1.02f);
        movementSpeed *= Random.Range(0.95f, 1.05f);
        attackDamage *= Random.Range(0.92f, 1.08f);
        attackSpeed *= Random.Range(0.92f, 1.08f);*/

        if (boss)
            day_night = GameObject.FindGameObjectWithTag("Cycle").GetComponent(typeof(Day_Night_Cycle)) as Day_Night_Cycle;

        /*if (ranged)
        {
            attackRange *= Random.Range(0.95f, 1.05f);
            force *= Random.Range(0.95f, 1.05f);
        }*/

        if (playerStats.eq.Items[20])
            vulnerable += armor * 0.0032f;
        if (playerStats.eq.Items[22])
            curse += (22f + maxHealth * 0.18f) * playerStats.DamageDealtMultiplyer(0.287f);

        health = maxHealth;
        shield = shieldCapacity;
        UpdateBars();

        Invoke("Tick", 0.6f);
    }

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerBody = Player.GetComponent<Rigidbody2D>();
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        }
        if (stun <= 0f)
        {
            if (attackTimer >= 0f)
                attackTimer -= Time.deltaTime * SpeedEfficiency();
            if (Vector3.Distance(transform.position, Player.transform.position) <= attackRange)
                Attack();
            if (flank)
                Flank();
            else if (Vector3.Distance(transform.position, Player.transform.position) >= minRange)
                Chase();
            /*switch (CurrentState)
            {
                case (EnemyState.Chase):
                    Chase();
                    break;
                case (EnemyState.Attack):
                    Attack();
                    break;
            }*/
        }
        else stun -= Time.deltaTime;

        if (rechargeTimer > 0f)
        {
            rechargeTimer -= Time.deltaTime * SpeedEfficiency();
        }
        else if (shield < shieldCapacity)
        {
            shield += Time.deltaTime * shieldRechargeRate * SpeedEfficiency();
            UpdateBars();
        }

        /*if (Vector3.Distance(transform.position, Player.transform.position) <= attackRange)
        {
            CurrentState = EnemyState.Attack;
        }
        else
        {
            CurrentState = EnemyState.Chase;
        }*/
    }

    void UpdateBars()
    {
        if (shieldCapacity > 0)
            ShieldFill.fillAmount = shield / shieldCapacity;
        DoTFill.fillAmount = (health - DoT) / maxHealth;
        healthFill.fillAmount = health / maxHealth;
    }

    void FixedUpdate()
    {
        Vector2 lookDir = playerBody.position - Body.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        Dir.rotation = angle;
    }

    float SpeedEfficiency()
    {
        temp = 1f;
        if (slowed)
            temp *= 0.6f;
        return temp;
    }

    void Flank()
    {
        if (attackTimer < 0f)
            transform.position = Vector2.MoveTowards(transform.position, FlankPosition.position, movementSpeed * SpeedEfficiency() * Time.deltaTime);
        else transform.position = Vector2.MoveTowards(transform.position, FlankPosition.position, movementSpeed * SpeedEfficiency() * aimingMovement * Time.deltaTime);
    }

    void Chase()
    {
        if (attackTimer < 0f)
            transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, movementSpeed * SpeedEfficiency() * Time.deltaTime);
        else transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, movementSpeed * SpeedEfficiency() * aimingMovement * Time.deltaTime);
    }

    public void FoundObstacle(Transform Point)
    {
        if (!flank)
        {   
            FlankPosition = Point;
            flank = true;
            Invoke("EndFlank", 0.15f);
        }
    }

    void EndFlank()
    {
        flank = false;
    }

    void Attack()
    {
        if (attackTimer < 0f)
        {
            if (ranged)
                Fire();
            else Strike();

            attackTimer += attackSpeed;
        }
    }

    void Strike()
    {
        playerStats.TakeDamage(attackDamage, false);
        playerStats.GainPoison(attackPoison);
    }

    void Fire()
    {
        if (bulletBurst != 1)
        {
            for (int i = 0; i < bulletBurst; i++)
            {
                Invoke("Shoot", i * burstDelay);
            }
        }
        else Shoot();
    }

    void Shoot()
    {
        currentForce = force * Random.Range(1f, 1.1f);
        if (throws)
            currentForce *= 0.25f + 0.75f * Vector3.Distance(transform.position, Player.transform.position) / attackRange;
        if (bulletCount != 1)
        {
            recoil = Random.Range(-accuracy, accuracy);
            for (int i = 0; i < bulletCount; i++)
            {
                Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + recoil + (i * 2 - bulletCount + 1) * bulletSpread / 2);
                GameObject bullet = Instantiate(Projectal, Dir.position, Sight.rotation);
                Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
                bullet_body.AddForce(Sight.up * currentForce, ForceMode2D.Impulse);
            }
        }
        else
        {
            Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(-accuracy, accuracy));
            GameObject bullet = Instantiate(Projectal, Dir.position, Sight.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Sight.up * currentForce, ForceMode2D.Impulse);
        }
    }

    public void Struck(Collider2D other)
    {
        collidedBullet = other.GetComponent(typeof(Bullet)) as Bullet;
        Push.rotation = other.transform.rotation;
        if (!collidedBullet.AoE)
        {   
            temp = collidedBullet.force * collidedBullet.mass / tenacity;
            Body.AddForce(Push.up * temp, ForceMode2D.Impulse);
            armor *= 1 - (collidedBullet.armorShred * 1.6f / (1f + 0.03f * weight));
            vulnerable += collidedBullet.vulnerableApplied * 1.6f / (1f + 0.03f * weight);
            if (collidedBullet.slowDuration > 0)
                GainSlow(collidedBullet.slowDuration * 1.6f / (1f + 0.03f * weight));
            if (collidedBullet.stunDuration > 0)
                GainStun(collidedBullet.stunDuration * 1.6f / (1f + 0.03f * weight));
            temp = collidedBullet.damage / DamageTakenMultiplyer(collidedBullet.penetration);
            if (collidedBullet.shatter > 0)
                ShatterShield(temp * collidedBullet.shatter);
            TakeDamage(temp, collidedBullet.crit, true);
            if (collidedBullet.DoT > 0)
                GainDoT(temp * collidedBullet.DoT);
            if (collidedBullet.incendiary > 0)
                SetAblaze(temp * collidedBullet.incendiary);
            curse += collidedBullet.curse;
        }
        collidedBullet.Struck();
    }

    void TakeDamage(float value, bool crited, bool display)
    {
        if (display)
        {
            DamageOrigin.rotation = Quaternion.Euler(DamageOrigin.rotation.x, DamageOrigin.rotation.y, Body.rotation + Random.Range(-12f, 12f));
            GameObject text = Instantiate(damageTook, Body.position, DamageOrigin.rotation);
            Rigidbody2D text_body = text.GetComponent<Rigidbody2D>();
            damageDisplay = text.GetComponent(typeof(DamageTaken)) as DamageTaken;
            if (crited) damageDisplay.SetText(value, "red");
            else damageDisplay.SetText(value, "orange");
            text_body.AddForce(DamageOrigin.up * 3.6f, ForceMode2D.Impulse);
        }

        if (shield > 0f)
        {
            shield -= value;
            rechargeTimer = shieldRechargeDelay;

            if (shield < 0f)
                health += shield;
        }
        else health -= value;

        UpdateBars();

        if (health <= 0)
            Death();
    }

    void TakePoisonDamage(float value)
    {
        DamageOrigin.rotation = Quaternion.Euler(DamageOrigin.rotation.x, DamageOrigin.rotation.y, Body.rotation + Random.Range(-12f, 12f));
        GameObject text = Instantiate(damageTook, Body.position, DamageOrigin.rotation);
        Rigidbody2D text_body = text.GetComponent<Rigidbody2D>();
        damageDisplay = text.GetComponent(typeof(DamageTaken)) as DamageTaken;
        damageDisplay.SetText(value, "green");
        text_body.AddForce(DamageOrigin.up * 3.6f, ForceMode2D.Impulse);

        health -= value;

        UpdateBars();

        if (health <= 0)
            Death();
    }

    void RestoreHealth(float value)
    {
        health += value;
        if (health > maxHealth)
            health = maxHealth;
        UpdateBars();
    }

    void GainSlow(float duration)
    {
        slow += duration;
        if (slow >= 0.5f && !slowed)
        {
            slow -= 0.5f;
            slowed = true;
            Invoke("Thaw", 0.5f);
        }
    }

    void Thaw()
    {
        if (slow >= 0.5f)
        {
            slow -= 0.5f;
            Invoke("Thaw", 0.5f);
        }
        else
            slowed = false;
    }

    public void GainStun(float duration)
    {
        stun += duration;
    }

    void GainDoT(float value)
    {
        DoT += value;
        UpdateBars();
    }

    void ShatterShield(float value)
    {
        if (shield > 0)
        {
            if (value > shield)
            {
                DamageOrigin.rotation = Quaternion.Euler(DamageOrigin.rotation.x, DamageOrigin.rotation.y, Body.rotation + Random.Range(-12f, 12f));
                GameObject text = Instantiate(damageTook, Body.position, DamageOrigin.rotation);
                Rigidbody2D text_body = text.GetComponent<Rigidbody2D>();
                damageDisplay = text.GetComponent(typeof(DamageTaken)) as DamageTaken;
                damageDisplay.SetText(shield, "cyan");
                text_body.AddForce(DamageOrigin.up * 3.6f, ForceMode2D.Impulse);

                shield = 0;
            }
            else
            {
                shield -= value;

                DamageOrigin.rotation = Quaternion.Euler(DamageOrigin.rotation.x, DamageOrigin.rotation.y, Body.rotation + Random.Range(-12f, 12f));
                GameObject text = Instantiate(damageTook, Body.position, DamageOrigin.rotation);
                Rigidbody2D text_body = text.GetComponent<Rigidbody2D>();
                damageDisplay = text.GetComponent(typeof(DamageTaken)) as DamageTaken;
                damageDisplay.SetText(value, "cyan");
                text_body.AddForce(DamageOrigin.up * 3.6f, ForceMode2D.Impulse);
            }

            rechargeTimer = shieldRechargeDelay;
        }
    }

    void SetAblaze(float value)
    {
        ablaze += value;
        if (ablaze >= 20f && !burning)
        {
            burning = true;
            Invoke("Conflagration", 0.5f);
        }
    }

    void Conflagration()
    {
        ablaze -= 20f;
        GameObject bullet = Instantiate(FireExplosion, Body.position, transform.rotation);
        firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        firedBullet.damage = 20f;
        if (ablaze >= 20f)
            Invoke("Conflagration", 0.5f);
        else burning = false;
        SetAblaze(0f);
    }

    float DamageTakenMultiplyer(float penetration)
    {
        temp = 1f + (armor * (1 - penetration) * 0.01f);
        temp /= 1f + vulnerable;
        return temp;
    }

    void Tick()
    {
        if (DoT > 0)
            DoTproc();

        RestoreHealth(regen * 0.5f);

        Invoke("Tick", 0.5f);
    }

    void DoTproc()
    {
        temp = 2.2f + DoT * 0.22f;
        DoT -= temp;
        TakePoisonDamage(temp);
    }

    void Death()
    {
        if (!dead)
        {
            dead = true;

            playerStats.EnemySlained();

            for (int i = 0; i < scrapCount; i++)
            {
                if (scrapChance >= Random.Range(0f, 1f))
                    DropScrap();
            }

            for (int i = 0; i < itemCount; i++)
            {
                if (itemChance >= Random.Range(0f, 1f))
                    DropItem();
            }

            if (DeathDrop)
                DeathDrop.Trigger();

            if (curse > 0)
            {
                tempi = 1;
                while ((8 + 2 * tempi) * tempi < curse)
                {
                    tempi++;
                }
                curse /= tempi;
                for (int i = 0; i < tempi; i++)
                {
                    Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 40f) + i * 360f / tempi);
                    GameObject bullet = Instantiate(CurseBullet, Body.position, Sight.rotation);
                    Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
                    bullet_body.AddForce(Sight.up * Random.Range(15f, 15.5f), ForceMode2D.Impulse);
                    firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
                    firedBullet.damage = curse;
                }
            }

            if (boss)
            {
                day_night.StartDay();
            }
        }

        Destroy(gameObject);
    }

    void DropScrap()
    {
        Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
        GameObject scrap = Instantiate(Scrap, Body.position, Sight.rotation);
        Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
        scrap_body.AddForce(Sight.up * Random.Range(1.2f, 5.1f), ForceMode2D.Impulse);
    }

    void DropItem()
    {
        Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
        GameObject scrap = Instantiate(Items[Random.Range(0, Items.Length)], Dir.position, Sight.rotation);
        Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
        scrap_body.AddForce(Sight.up * Random.Range(1.2f, 5.1f), ForceMode2D.Impulse);
    }
}
