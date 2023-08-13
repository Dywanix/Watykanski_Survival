using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Transform Sight, DamageOrigin;
    private Bullet collidedBullet;
    private DamageTaken damageDisplay;
    public Day_Night_Cycle day_night;

    // ----- enemy stats -----
    private int roll;
    private float temp;
    public bool rare, boss, dead;

    [Header("Health & Resistance")]
    public Bar hpBar;
    public int weight;
    public float maxHealth, health, regen, armor, vulnerable, DoT, burning;

    [Header("Movement")]
    public float movementSpeed;
    public float aimingMovement, slow, stun;

    [Header("Damage & Attacks")]
    public bool attackTimer = false;
    public float attackDamage, attackPoison, attackSpeed, attackRange;

    [Header("Ranged Stuff")]
    public bool ranged = false;
    public bool throws = false;
    public int bulletCount, bulletBurst;
    public float bulletSpread, burstDelay, accuracy, force, minRange;
    float recoil, currentForce;

    [Header("Specials")]
    public string[] enrageStats;
    public float[] enrageValue;

    [Header("Dropy")]
    public LeftOver DeathDrop;
    public float scrapChance, itemChance;
    public int scrapCount, itemCount;

    [Header("Graficzne")]
    public GameObject Blood;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerBody = Player.GetComponent<Rigidbody2D>();
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;

        maxHealth *= Random.Range(0.96f, 1.04f);
        maxHealth *= 1f + 0.016f * playerStats.dayCount;
        armor *= Random.Range(0.98f, 1.02f);
        movementSpeed *= Random.Range(0.95f, 1.05f);
        movementSpeed *= 1f + 0.005f * playerStats.dayCount;
        attackDamage *= Random.Range(0.92f, 1.08f);
        attackSpeed *= Random.Range(0.92f, 1.08f);

        if (rare)
        {
            maxHealth *= 0.99f + 0.01f * playerStats.dayCount;
            movementSpeed *= 0.986f + 0.014f * playerStats.dayCount;
        }

        if (boss)
        {
            maxHealth *= 0.985f + 0.015f * playerStats.dayCount;
            attackDamage *= 0.98f + 0.02f * playerStats.dayCount;
            day_night = GameObject.FindGameObjectWithTag("Cycle").GetComponent(typeof(Day_Night_Cycle)) as Day_Night_Cycle;
        }

        if (ranged)
        {
            attackRange *= Random.Range(0.95f, 1.05f);
            force *= Random.Range(0.95f, 1.05f);
        }

        scrapChance *= 1f + 0.15f * playerStats.eq.Items[9];
        itemChance *= 1f + 0.08f * playerStats.eq.Items[9];

        health = maxHealth;
        hpBar.SetMaxValue(maxHealth);
        hpBar.SetValue(health);

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
            if (Vector3.Distance(transform.position, Player.transform.position) <= attackRange)
                Attack();
            if (Vector3.Distance(transform.position, Player.transform.position) >= minRange)
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

        /*if (Vector3.Distance(transform.position, Player.transform.position) <= attackRange)
        {
            CurrentState = EnemyState.Attack;
        }
        else
        {
            CurrentState = EnemyState.Chase;
        }*/
    }

    void FixedUpdate()
    {
        Vector2 lookDir = playerBody.position - Body.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        Dir.rotation = angle;
    }

    void Chase()
    {
        if (!attackTimer)
        {
            if (slow > 0f)
            {
                transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, movementSpeed * 0.6f * Time.deltaTime);
                slow -= Time.deltaTime;
            }
            else transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, movementSpeed * Time.deltaTime);
        }
        else
        {
            if (slow > 0f)
            {
                transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, movementSpeed * 0.6f * aimingMovement * Time.deltaTime);
                slow -= Time.deltaTime;
            }
            else transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, movementSpeed * aimingMovement * Time.deltaTime);
        }
    }

    void Attack()
    {
        if (!attackTimer)
        {
            if (ranged)
                Fire();
            else Strike();
            StartCoroutine(attackTime());
        }
    }

    public IEnumerator attackTime()
    {
        attackTimer = true;
        yield return new WaitForSeconds(attackSpeed);
        attackTimer = false;
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
                GameObject scrap = Instantiate(Projectal, Dir.position, Sight.rotation);
                Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
                scrap_body.AddForce(Sight.up * currentForce, ForceMode2D.Impulse);
            }
        }
        else
        {
            Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(-accuracy, accuracy));
            GameObject scrap = Instantiate(Projectal, Dir.position, Sight.rotation);
            Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
            scrap_body.AddForce(Sight.up * currentForce, ForceMode2D.Impulse);
        }
    }

    public void Struck(Collider2D other)
    {
        collidedBullet = other.GetComponent(typeof(Bullet)) as Bullet;
        if (!collidedBullet.AoE)
        {
            armor *= 1 - (collidedBullet.armorShred * 1.6f / (1f + 0.03f * weight));
            vulnerable += collidedBullet.vulnerableApplied * 1.6f / (1f + 0.03f * weight);
            if (collidedBullet.slowDuration > 0)
                slow += collidedBullet.slowDuration * 1.6f / (1f + 0.03f * weight);
            if (collidedBullet.stunChance >= Random.Range(0f, 1f))
                GainStun(collidedBullet.stunDuration * 1.6f / (1f + 0.03f * weight));
            TakeDamage(collidedBullet.damage / DamageTakenMultiplyer(collidedBullet.penetration), collidedBullet.crit, true);
            if (collidedBullet.DoT > 0)
                GainDoT(collidedBullet.damage * collidedBullet.DoT / DamageTakenMultiplyer(collidedBullet.penetration));
            if (collidedBullet.incendiary > 0)
                burning += collidedBullet.incendiary;
        }
        collidedBullet.Struck();
    }

    void TakeDamage(float value, bool crited, bool display)
    {
        health -= value;
        hpBar.SetValue(health);

        if (display)
        {
            DamageOrigin.rotation = Quaternion.Euler(DamageOrigin.rotation.x, DamageOrigin.rotation.y, Body.rotation + Random.Range(-12f, 12f));
            GameObject text = Instantiate(damageTook, Body.position, DamageOrigin.rotation);
            Rigidbody2D text_body = text.GetComponent<Rigidbody2D>();
            damageDisplay = text.GetComponent(typeof(DamageTaken)) as DamageTaken;
            damageDisplay.SetText(value, crited);
            text_body.AddForce(DamageOrigin.up * 3.6f, ForceMode2D.Impulse);
        }

        if (enrageStats.Length > 0)
        {
            for (int i = 0; i < enrageStats.Length; i++)
            {
                switch (enrageStats[i])
                {
                    case "armor":
                        armor += value * enrageValue[i];
                        break;
                    case "movementSpeed":
                        movementSpeed += value * enrageValue[i];
                        break;
                    case "attackDamage":
                        attackDamage += value * enrageValue[i];
                        break;
                    case "accuracy":
                        accuracy += value * enrageValue[i];
                        break;
                    case "DoT":
                        DoT += value * enrageValue[i];
                        break;
                }
            }
        }

        if (health <= 0)
            Death();
    }

    void RestoreHealth(float value)
    {
        health += value;
        if (health > maxHealth)
            health = maxHealth;
        hpBar.SetValue(health);
    }

    public void GainStun(float duration)
    {
        stun += duration;
    }

    void GainDoT(float value)
    {
        DoT += value;
    }

    float DamageTakenMultiplyer(float penetration)
    {
        temp = 1f + (armor * (1 - penetration) * 0.01f);
        temp /= 1f + vulnerable;
        return temp;
    }

    void Tick()
    {
        //if (playerStats.day)
            //Burn();
        if (burning > 0f)
        {
            Burn();
            burning -= 0.5f;
        }

        if (DoT > 0)
            DoTproc();

        RestoreHealth(regen * 0.5f);

        Invoke("Tick", 0.5f);
    }

    void Burn()
    {
        vulnerable += 0.08f * 0.5f;
        temp = (3f + maxHealth * 0.01f) * 0.5f;
        TakeDamage(temp / DamageTakenMultiplyer(0.8f), false, false);
    }

    void DoTproc()
    {
        temp = 1f + DoT * 0.4f;
        DoT -= temp;
        TakeDamage(temp / DamageTakenMultiplyer(1f), false, true);
    }

    void Death()
    {
        if (!dead)
        {
            dead = true;

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
