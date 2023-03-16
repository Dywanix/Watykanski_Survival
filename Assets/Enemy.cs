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
    public GameObject Player, Scrap;
    public PlayerController playerStats;
    public Rigidbody2D Body, playerBody, Dir;
    public Transform Sight;
    private Bullet collidedBullet;

    // ----- enemy stats -----
    public int[] scrapDroppedRange;
    private int roll;

    // -- Health & Resistance
    public Bar hpBar;
    public float maxHealth, health, armor;

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
        health = maxHealth;
        hpBar.SetMaxValue(maxHealth);
        hpBar.SetValue(health);
        movementSpeed *= Random.Range(0.95f, 1.05f);
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
            TakeDamage(collidedBullet.damage / DamageTakenMultiplyer(collidedBullet.penetration));
        }
    }

    void TakeDamage(float value)
    {
        health -= value;
        hpBar.SetValue(health);

        if (health <= 0)
            Death();
    }

    float DamageTakenMultiplyer(float penetration)
    {
        return 1f + (armor * (1 - penetration) * 0.01f);
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
