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
    public GameObject Player, Scrap, Orb, Orb5, Item, damageTook, Projectal;
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
    public bool elite, boss, dead;

    [Header("Health & Resistance")]
    public int weight;
    public Image healthFill, DoTFill, ShieldFill;
    public float maxHealth, health, vulnerable;
    public float shieldCapacity, shield;

    [Header("Movement")]
    public float movementSpeed;
    public float aimingMovement, tenacity, stun, leashRange;
    public bool flank, agro;

    [Header("Damage & Attacks")]
    public float attackTimer;
    public float attackDamage, attackPoison, attackSpeed, attackRange;
    public float cooldownReduction;
    public bool repositioning;

    [Header("Ranged Stuff")]
    public bool ranged;
    public bool throws;
    public int bulletCount, bulletBurst;
    public float bulletSpread, burstDelay, accuracy, force, minRange;
    float recoil, currentForce;

    [Header("Dropy")]
    public LeftOver DeathDrop;
    public float scrapChance, itemChance;
    public int scrapCount, itemCount;
    public int experienceDroped, BigOrbs;

    [Header("Graficzne")]
    public GameObject Smoke;

    [Header("Status")]
    public GameObject FireExplosion;
    public GameObject CurseBullet;
    public bool burning, slowed;
    public float slow, DoT, ablaze, curse;

    void Start()
    {
        Smoke.SetActive(true);
        Player = GameObject.FindGameObjectWithTag("Player");
        playerBody = Player.GetComponent<Rigidbody2D>();
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;

        /*if (elite)
        {
            tempi = weight - 50;
            maxHealth *= 1f + tempi * 0.02f;
            shieldCapacity *= 1f + tempi * 0.02f;
            shieldRechargeRate *= 1f + tempi * 0.02f;
            attackSpeed /= 1f + tempi * 0.008f;
            cooldownReduction = 1f + tempi * 0.01f;
            scrapChance *= 1f + tempi * 0.02f;
            itemChance *= 1f + tempi * 0.02f;
        }*/

        if (boss)
            day_night = GameObject.FindGameObjectWithTag("Cycle").GetComponent(typeof(Day_Night_Cycle)) as Day_Night_Cycle;

        //if (playerStats.eq.Items[22])
            //curse += (24f + maxHealth * 0.2f) * playerStats.DamageDealtMultiplyer(0.312f);

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
        /*else if (!agro && Vector3.Distance(transform.position, Player.transform.position) <= 15f + leashRange)
            agro = true;
        else leashRange += 0.3f * Time.deltaTime;*/
        if (stun <= 0f)
        {
            if (attackTimer >= 0f && !repositioning)
                attackTimer -= Time.deltaTime * SpeedEfficiency();
            if (Vector3.Distance(transform.position, Player.transform.position) <= attackRange && !repositioning)
                Attack();
            if (flank)
                Flank();
            else if (Vector3.Distance(transform.position, Player.transform.position) >= minRange)
                Chase();
            /*if (agro)
            {
                if (flank)
                    Flank();
                else if (Vector3.Distance(transform.position, Player.transform.position) >= minRange)
                    Chase();
            }
            switch (CurrentState)
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

    public float SpeedEfficiency()
    {
        temp = 1f;
        if (slowed)
            temp *= 0.6f;
        return temp;
    }

    void Flank()
    {
        if (attackTimer < 0f || repositioning)
            transform.position = Vector2.MoveTowards(transform.position, FlankPosition.position, movementSpeed * SpeedEfficiency() * Time.deltaTime);
        else transform.position = Vector2.MoveTowards(transform.position, FlankPosition.position, movementSpeed * SpeedEfficiency() * aimingMovement * Time.deltaTime);
    }

    void Chase()
    {
        if (attackTimer < 0f || repositioning)
            transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, movementSpeed * SpeedEfficiency() * Time.deltaTime);
        else transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, movementSpeed * SpeedEfficiency() * aimingMovement * Time.deltaTime);
    }

    public void FoundObstacle(Transform Point, bool solid)
    {
        if (!flank)
        {   
            FlankPosition = Point;
            flank = true;
            if (solid)
            {
                repositioning = true;
                Invoke("EndReposition", 0.35f);
            }
            else Invoke("EndFlank", 0.15f);
        }
    }

    void EndFlank()
    {
        flank = false;
    }

    void EndReposition()
    {
        flank = false;
        repositioning = false;
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
        if (playerStats.eq.Items[38] > 0)
            TakeDamage(attackDamage * (0.42f + 0.84f * playerStats.eq.Items[38]), false, true);
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
            temp = collidedBullet.force * collidedBullet.mass / tenacity; temp *= 0.5f;
            Body.AddForce(Push.up * temp, ForceMode2D.Impulse);
            //armor *= 1 - (collidedBullet.armorShred * 1.6f / (1f + 0.03f * weight));
            vulnerable += collidedBullet.vulnerableApplied * 1.6f / (1f + 0.03f * weight);
            if (collidedBullet.slowDuration > 0)
                GainSlow(collidedBullet.slowDuration * 1.6f / (1f + 0.03f * weight));
            if (collidedBullet.stunDuration > 0)
                GainStun(collidedBullet.stunDuration * 1.6f / (1f + 0.03f * weight));
            temp = collidedBullet.damage * DamageTakenMultiplyer();
            if (collidedBullet.shatter > 0)
                ShatterShield(temp * collidedBullet.shatter);
            if (!collidedBullet.damageLess)
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

            if (shield < 0f)
                health += shield;
        }
        else health -= value;

        UpdateBars();

        if (health <= 0)
            Death();
    }

    void TakePierceDamage(float value, bool crited, bool display)
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

        health -= value;

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

                LoseShield(shield);
            }
            else
            {
                DamageOrigin.rotation = Quaternion.Euler(DamageOrigin.rotation.x, DamageOrigin.rotation.y, Body.rotation + Random.Range(-12f, 12f));
                GameObject text = Instantiate(damageTook, Body.position, DamageOrigin.rotation);
                Rigidbody2D text_body = text.GetComponent<Rigidbody2D>();
                damageDisplay = text.GetComponent(typeof(DamageTaken)) as DamageTaken;
                damageDisplay.SetText(value, "cyan");
                text_body.AddForce(DamageOrigin.up * 3.6f, ForceMode2D.Impulse);

                LoseShield(value);
            }
        }
    }

    void LoseShield(float amount)
    {
        shield -= amount;
        if (playerStats.eq.Items[46] > 0)
            TakePierceDamage(amount * 0.25f * playerStats.eq.Items[46], false, true);
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

    float DamageTakenMultiplyer()
    {
        //temp = 1f + (armor * (1 - penetration) * 0.01f);
        temp = 1f + vulnerable;
        return temp;
    }

    void Tick()
    {
        if (DoT > 0)
            DoTproc();

        //RestoreHealth(regen * 0.5f);

        Invoke("Tick", 0.5f);
    }

    void DoTproc()
    {
        temp = 2.5f + DoT * 0.25f;
        if (playerStats.eq.Items[58] > 0)
            DoT -= temp / (1f + 0.13f * playerStats.eq.Items[58] + 0.02f * playerStats.eq.Items[58] * playerStats.eq.Items[58]);
        else DoT -= temp;
        TakePoisonDamage(temp);
    }

    void Death()
    {
        if (!dead)
        {
            dead = true;

            experienceDroped = Random.Range(weight * 8 / 10, weight /* 9 / 10*/ + 1);
            playerStats.EnemySlained();

            for (int i = 0; i < scrapCount; i++)
            {
                if (scrapChance >= Random.Range(0f, 1f))
                {
                    DropScrap();
                    experienceDroped -= 1; 
                }
            }

            DropExperience();

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

            if (elite)
            {
                Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
                GameObject scrap = Instantiate(Item, Body.position, transform.rotation);
                Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
                scrap_body.AddForce(Sight.up * Random.Range(1.2f, 3.8f), ForceMode2D.Impulse);
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
        GameObject scrap = Instantiate(Scrap, Body.position, transform.rotation);
        Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
        scrap_body.AddForce(Sight.up * Random.Range(1.2f, 4.6f), ForceMode2D.Impulse);
    }

    void DropExperience()
    {
        while (experienceDroped >= 5 + BigOrbs)
        {
            Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
            GameObject orb = Instantiate(Orb5, Body.position, transform.rotation);
            Rigidbody2D orb_body = orb.GetComponent<Rigidbody2D>();
            orb_body.AddForce(Sight.up * Random.Range(1.1f, 4.2f), ForceMode2D.Impulse);

            BigOrbs++;
            experienceDroped -= 5;
        }
        for (int i = 0; i < experienceDroped; i++)
        {
            Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
            GameObject orb = Instantiate(Orb, Body.position, transform.rotation);
            Rigidbody2D orb_body = orb.GetComponent<Rigidbody2D>();
            orb_body.AddForce(Sight.up * Random.Range(1.2f, 4.6f), ForceMode2D.Impulse);
        }
    }

    void DropItem()
    {
        Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
        GameObject scrap = Instantiate(Items[Random.Range(0, Items.Length)], Dir.position, transform.rotation);
        Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
        scrap_body.AddForce(Sight.up * Random.Range(1.2f, 4.6f), ForceMode2D.Impulse);
    }
}
