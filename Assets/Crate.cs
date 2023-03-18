using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public GameObject Player, Scrap;
    public PlayerController playerStats;
    private Bullet collidedBullet;
    public Rigidbody2D Dir;
    public Transform Sight;

    public int[] scrapDroppedRange;
    private int roll;
    public float health;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "PlayerProjectal")
        {
            collidedBullet = other.GetComponent(typeof(Bullet)) as Bullet;
            TakeDamage(collidedBullet.damage);
        }
    }

    void TakeDamage(float value)
    {
        health -= value;

        if (health <= 0)
            Destroy();
    }

    void Destroy()
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
            GameObject scrap = Instantiate(Scrap, Dir.position, Sight.rotation);
            Rigidbody2D scrap_body = scrap.GetComponent<Rigidbody2D>();
            scrap_body.AddForce(Sight.up * Random.Range(1.2f, 5.1f), ForceMode2D.Impulse);
        }
    }
}
