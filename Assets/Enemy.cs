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
    public Transform Sight;
    private Bullet collidedBullet;
    private DamageTaken damageDisplay;

    // ----- enemy stats -----
    private int roll;
    private float temp;

    // -- Health & Resistance
    public Bar hpBar;
    public int weight;
    public float maxHealth, health, regen, armor, vulnerable, DoT, burning;

    // -- Movement
    public float movementSpeed, slow, stun;

    // -- Damage & Attacks
    public float attackDamage, attackPoison, attackSpeed, attackRange, accuracy, force;
    public bool attackTimer = false, ranged = false;

    // -- Special Stats
    public string[] enrageStats;
    public float[] enrageValue;

    // -- Dropy --
    public float scrapChance, itemChance;
    public int scrapCount, itemCount;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerBody = Player.GetComponent<Rigidbody2D>();
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;

        maxHealth *= Random.Range(0.96f, 1.04f);
        armor *= Random.Range(0.98f, 1.02f);
        movementSpeed *= Random.Range(0.95f, 1.05f);
        attackDamage *= Random.Range(0.92f, 1.08f);
        attackSpeed *= Random.Range(0.92f, 1.08f);

        if (ranged)
        {
            attackRange *= Random.Range(0.95f, 1.05f);
            force *= Random.Range(0.95f, 1.05f);
        }

        health = maxHealth;
        hpBar.SetMaxValue(maxHealth);
        hpBar.SetValue(health);

        Invoke("Tick", 0.5f);
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
            switch (CurrentState)
            {
                case (EnemyState.Chase):
                    Chase();
                    break;
                case (EnemyState.Attack):
                    Attack();
                    break;
            }
        }
        else stun -= Time.deltaTime;

        if (Vector3.Distance(transform.position, Player.transform.position) <= attackRange)
        {
            CurrentState = EnemyState.Attack;
        }
        else
        {
            CurrentState = EnemyState.Chase;
        }
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
    }

    void Attack()
    {
        if (!attackTimer)
        {
            if (ranged)
                Shoot();
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
        playerStats.TakeDamage(attackDamage);
        playerStats.GainPoison(attackPoison);
    }

    void Shoot()
    {
        Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(- accuracy, accuracy));
        GameObject scrap = Instantiate(Projectal, Dir.position, Sight.rotation);
        Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
        scrap_body.AddForce(Sight.up * force * Random.Range(1f, 1.1f), ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "PlayerProjectal")
        {
            collidedBullet = other.GetComponent(typeof(Bullet)) as Bullet;
            if (!collidedBullet.AoE)
            {
                armor *= 1 - collidedBullet.armorShred;
                vulnerable += collidedBullet.vulnerableApplied;
                if (collidedBullet.slowDuration > 0)
                    slow += collidedBullet.slowDuration;
                if (collidedBullet.stunChance >= Random.Range(0f, 1f))
                    GainStun(collidedBullet.stunDuration);
                TakeDamage(collidedBullet.damage / DamageTakenMultiplyer(collidedBullet.penetration), collidedBullet.crit, true);
                if (collidedBullet.DoT > 0)
                    GainDoT(collidedBullet.damage * collidedBullet.DoT / DamageTakenMultiplyer(collidedBullet.penetration));
                if (collidedBullet.incendiary > 0)
                    burning += collidedBullet.incendiary;
            }
            collidedBullet.Struck();
        }
    }

    void TakeDamage(float value, bool crited, bool display)
    {
        health -= value;
        hpBar.SetValue(health);

        if (display)
        {
            Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Body.rotation + Random.Range(-12f, 12f));
            GameObject text = Instantiate(damageTook, Body.position, Sight.rotation);
            Rigidbody2D text_body = text.GetComponent<Rigidbody2D>();
            damageDisplay = text.GetComponent(typeof(DamageTaken)) as DamageTaken;
            damageDisplay.SetText(value, crited);
            text_body.AddForce(Sight.up * 3.6f, ForceMode2D.Impulse);
        }

        if (enrageStats.Length > 0)
        {
            for (int i = 0; i < enrageStats.Length; i++)
            {
                switch (enrageStats[i])
                {
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

    void GainStun(float duration)
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
        if (playerStats.day)
            Burn();
        else if (burning > 0f)
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
        temp = 1f + DoT * 0.35f;
        TakeDamage(temp / DamageTakenMultiplyer(1f), false, true);
        DoT -= temp;
    }

    void Death()
    {
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
