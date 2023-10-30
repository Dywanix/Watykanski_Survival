using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public GameObject Scrap, Tools, damageTook;
    public GameObject[] Items;
    public PlayerController playerStats;
    private Bullet collidedBullet;
    public Rigidbody2D Dir;
    public Transform Sight;
    private DamageTaken damageDisplay;

    public int itemsCount;
    int roll;
    public float maxHealth, health, dropChance;
    float temp;
    bool destroyed;

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (!playerStats)
            playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent(typeof(PlayerController)) as PlayerController;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "PlayerProjectal")
        {
            collidedBullet = other.GetComponent(typeof(Bullet)) as Bullet;
            if (!collidedBullet.AoE)
                TakeDamage(collidedBullet.damage * (1f + collidedBullet.penetration), collidedBullet.crit);
            collidedBullet.Struck();
        }
    }

    void TakeDamage(float value, bool crited)
    {
        health -= value;

        Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(-12f, 12f));
        GameObject text = Instantiate(damageTook, Dir.position, Sight.rotation);
        Rigidbody2D text_body = text.GetComponent<Rigidbody2D>();
        damageDisplay = text.GetComponent(typeof(DamageTaken)) as DamageTaken;
        if (crited) damageDisplay.SetText(value, "red");
        else damageDisplay.SetText(value, "orange");
        text_body.AddForce(Sight.up * 3.6f, ForceMode2D.Impulse);

        if (health <= 0)
            Destroy();
    }

    void Destroy()
    {
        if (!destroyed)
        {
            destroyed = true;

            for (int i = 0; i < itemsCount; i++)
            {
                if (dropChance >= Random.Range(0f, 1f))
                    DropItem(Random.Range(0, Items.Length));
            }
        }

        Destroy(gameObject);
    }

    void DropItem(int which)
    {
        Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
        GameObject scrap = Instantiate(Items[which], Dir.position, Sight.rotation);
        Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
        scrap_body.AddForce(Sight.up * Random.Range(1.2f, 4.4f), ForceMode2D.Impulse);
    }
}
