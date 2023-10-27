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
    public Transform Sight, DamageOrigin, FlankPosition;
    private Bullet collidedBullet, firedBullet;
    private DamageTaken damageDisplay;
    public Day_Night_Cycle day_night;

    // ----- enemy stats -----
    private int roll, tempi;
    private float temp;
    public bool rare, boss, dead;

    [Header("Health & Resistance")]
    public int weight;
    public Image healthFill, DoTFill;
    public float maxHealth, health, regen, armor, vulnerable;

    [Header("Movement")]
    public float movementSpeed;
    public float aimingMovement, slow, stun;
    public bool flank;

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

    [Header("Status")]
    public GameObject FireExplosion;
    public GameObject CurseBullet;
    public bool burning;
    public float DoT, ablaze, curse;

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

        if (boss)
            day_night = GameObject.FindGameObjectWithTag("Cycle").GetComponent(typeof(Day_Night_Cycle)) as Day_Night_Cycle;

        if (ranged)
        {
            attackRange *= Random.Range(0.95f, 1.05f);
            force *= Random.Range(0.95f, 1.05f);
        }

        if (playerStats.eq.Items[20])
            vulnerable += armor * 0.003f;
        if (playerStats.eq.Items[22])
            curse += (10.5f + maxHealth * 0.15f) * playerStats.DamageDealtMultiplyer(0.2f);

        health = maxHealth;
        DoTFill.fillAmount = 1f;
        healthFill.fillAmount = 1f;

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

    void Flank()
    {
        if (!attackTimer)
        {
            if (slow > 0f)
            {
                transform.position = Vector2.MoveTowards(transform.position, FlankPosition.position, movementSpeed * 0.6f * Time.deltaTime);
                slow -= Time.deltaTime;
            }
            else transform.position = Vector2.MoveTowards(transform.position, FlankPosition.position, movementSpeed * Time.deltaTime);
        }
        else
        {
            if (slow > 0f)
            {
                transform.position = Vector2.MoveTowards(transform.position, FlankPosition.position, movementSpeed * 0.6f * aimingMovement * Time.deltaTime);
                slow -= Time.deltaTime;
            }
            else transform.position = Vector2.MoveTowards(transform.position, FlankPosition.position, movementSpeed * aimingMovement * Time.deltaTime);
        }
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
        if (!collidedBullet.AoE)
        {
            armor *= 1 - (collidedBullet.armorShred * 1.6f / (1f + 0.03f * weight));
            vulnerable += collidedBullet.vulnerableApplied * 1.6f / (1f + 0.03f * weight);
            if (collidedBullet.slowDuration > 0)
                slow += collidedBullet.slowDuration * 1.6f / (1f + 0.03f * weight);
            if (collidedBullet.stunChance >= Random.Range(0f, 1f))
                GainStun(collidedBullet.stunDuration * 1.6f / (1f + 0.03f * weight));
            temp = collidedBullet.damage / DamageTakenMultiplyer(collidedBullet.penetration);
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
        health -= value;
        DoTFill.fillAmount = (health - DoT) / maxHealth;
        healthFill.fillAmount = health / maxHealth;

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
        DoTFill.fillAmount = (health - DoT) / maxHealth;
        healthFill.fillAmount = health / maxHealth;
    }

    public void GainStun(float duration)
    {
        stun += duration;
    }

    void GainDoT(float value)
    {
        DoT += value;
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
        //if (playerStats.day)
            //Burn();

        if (DoT > 0)
            DoTproc();

        RestoreHealth(regen * 0.5f);

        Invoke("Tick", 0.5f);
    }

    void Burn()
    {
        vulnerable += 0.04f;
        temp = (1.5f + maxHealth * 0.005f);
        TakeDamage(temp / DamageTakenMultiplyer(0.8f), false, false);
    }

    void DoTproc()
    {
        temp = 2f + DoT * 0.25f;
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
