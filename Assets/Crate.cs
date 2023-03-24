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
    public float health, dropChance;

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
            TakeDamage(collidedBullet.damage, collidedBullet.crit);
            collidedBullet.Struck();
        }
    }

    void TakeDamage(float value, bool crited)
    {
        health -= value;

        Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation);
        GameObject text = Instantiate(damageTook, Dir.position, Sight.rotation);
        Rigidbody2D text_body = text.GetComponent<Rigidbody2D>();
        damageDisplay = text.GetComponent(typeof(DamageTaken)) as DamageTaken;
        damageDisplay.SetText(value, crited);
        text_body.AddForce(Sight.up * 3.6f, ForceMode2D.Impulse);

        if (health <= 0)
            Destroy();
    }

    void Destroy()
    {
        DropScrap();
        for (int i = 0; i < itemsCount; i++)
        {
            if (dropChance >= Random.Range(0f, 1f))
                DropItem();
        }
        Destroy(gameObject);
    }

    void DropScrap()
    {
        roll = Random.Range(scrapDroppedRange[0], scrapDroppedRange[1] + 1);
        for (int i = 0; i < roll; i++)
        {
            Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
            GameObject scrap = Instantiate(Scrap, Dir.position, Sight.rotation);
            Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
            scrap_body.AddForce(Sight.up * Random.Range(1.2f, 4.8f), ForceMode2D.Impulse);
        }
    }

    void DropItem()
    {
        Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
        GameObject scrap = Instantiate(Items[Random.Range(0, Items.Length)], Dir.position, Sight.rotation);
        Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
        scrap_body.AddForce(Sight.up * Random.Range(1.2f, 4.8f), ForceMode2D.Impulse);
    }
}
