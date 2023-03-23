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
    public GameObject Player, Scrap, damageTook;
    public PlayerController playerStats;
    public Rigidbody2D Body, playerBody, Dir;
    public Transform Sight;
    private Bullet collidedBullet;
    private DamageTaken damageDisplay;

    // ----- enemy stats -----
    public int weight;
    public int[] scrapDroppedRange;
    private int roll;
    private float temp, tick;

    // -- Health & Resistance
    public Bar hpBar;
    public float maxHealth, health, armor, vulnerable, DoT;

    // -- Movement
    public float movementSpeed;

    // -- Damage & Attacks
    public float attackDamage, attackSpeed, attackRange;
    private bool attackTimer = false;

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

        health = maxHealth;
        hpBar.SetMaxValue(maxHealth);
        hpBar.SetValue(health);
    }

    void Update()
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

        tick += Time.deltaTime;
        if (tick >= 0.3f)
            Tick();

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
            transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, movementSpeed * Time.deltaTime);
        }
    }

    void Attack()
    {
        playerStats.TakeDamage(attackDamage);
        StartCoroutine(attackTime());
    }

    private IEnumerator attackTime()
    {
        attackTimer = true;
        yield return new WaitForSeconds(attackSpeed);
        attackTimer = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "PlayerProjectal")
        {
            collidedBullet = other.GetComponent(typeof(Bullet)) as Bullet;
            armor *= 1 - collidedBullet.armorShred;
            vulnerable += collidedBullet.vulnerableApplied;
            TakeDamage(collidedBullet.damage / DamageTakenMultiplyer(collidedBullet.penetration), collidedBullet.crit);
            if (collidedBullet.DoT > 0)
                GainDoT(collidedBullet.damage * collidedBullet.DoT / DamageTakenMultiplyer(collidedBullet.penetration));
            collidedBullet.Struck();
        }
    }

    void TakeDamage(float value, bool crited)
    {
        health -= value;
        hpBar.SetValue(health);

        Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Body.rotation + Random.Range(-12f, 12f));
        GameObject text = Instantiate(damageTook, Body.position, Sight.rotation);
        Rigidbody2D text_body = text.GetComponent<Rigidbody2D>();
        damageDisplay = text.GetComponent(typeof(DamageTaken)) as DamageTaken;
        damageDisplay.SetText(value, crited);
        text_body.AddForce(Sight.up * 3.6f, ForceMode2D.Impulse);

        if (health <= 0)
            Death();
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
        tick -= 0.3f;
        if (playerStats.day)
            Burn();
        if (DoT > 0)
            DoTproc();
    }

    void Burn()
    {
        vulnerable += 0.01f;
        temp = (1.8f + maxHealth * 0.006f);
        TakeDamage(temp / DamageTakenMultiplyer(0.8f), false);
    }

    void DoTproc()
    {
        temp = 1 + DoT * 0.2f;
        TakeDamage(temp / DamageTakenMultiplyer(1f), false);
        DoT -= temp;
    }

    void Death()
    {
        DropScrap();
        Destroy(gameObject);
    }

    void DropScrap()
    {
        roll = Random.Range(scrapDroppedRange[0], scrapDroppedRange[1] + 1);
        for (int i = 0; i < roll; i++)
        {
            Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
            GameObject scrap = Instantiate(Scrap, Body.position, Sight.rotation);
            Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
            scrap_body.AddForce(Sight.up * Random.Range(1.2f, 5.1f), ForceMode2D.Impulse);
        }
    }
}
