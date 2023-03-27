using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public GameObject Player, Scrap, damageTook;
    public GameObject[] Items;
    public PlayerController playerStats;
    private Bullet collidedBullet;
    public Rigidbody2D Dir;
    public Transform Sight;
    private DamageTaken damageDisplay;

    public int[] scrapDroppedRange;
    public int itemsCount;
    int roll;
    public float maxHealth, health, dropChance, scrapChance, healthIncrease, dropChanceIncrease;
    public bool endless;
    float temp;

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "PlayerProjectal")
        {
            collidedBullet = other.GetComponent(typeof(Bullet)) as Bullet;
            if (!collidedBullet.AoE)
                TakeDamage(collidedBullet.damage * (1f + collidedBullet.crateBonus), collidedBullet.crit);
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
        damageDisplay.SetText(value, crited);
        text_body.AddForce(Sight.up * 3.6f, ForceMode2D.Impulse);

        temp = value * scrapChance;
        while (temp > 1f)
        {
            DropScrap(1, 1);
            temp--;
        }
        if (temp >= Random.Range(0f, 1f))
            DropScrap(1, 1);

        if (health <= 0)
            Destroy();
    }

    void Destroy()
    {
        DropScrap(scrapDroppedRange[0], scrapDroppedRange[1]);
        for (int i = 0; i < itemsCount; i++)
        {
            if (dropChance >= Random.Range(0f, 1f))
                DropItem();
        }
        if  (!endless)
            Destroy(gameObject);
        else
        {
            maxHealth += healthIncrease;
            health = maxHealth;
            dropChance += dropChanceIncrease;
            scrapDroppedRange[1]++;
        }
    }

    void DropScrap(int min, int max)
    {
        roll = Random.Range(min, max + 1);
        for (int i = 0; i < roll; i++)
        {
            Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
            GameObject scrap = Instantiate(Scrap, Dir.position, Sight.rotation);
            Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
            scrap_body.AddForce(Sight.up * Random.Range(1.3f, 5.0f), ForceMode2D.Impulse);
        }
    }

    void DropItem()
    {
        Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
        GameObject scrap = Instantiate(Items[Random.Range(0, Items.Length)], Dir.position, Sight.rotation);
        Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
        scrap_body.AddForce(Sight.up * Random.Range(1.2f, 4.4f), ForceMode2D.Impulse);
    }
}
