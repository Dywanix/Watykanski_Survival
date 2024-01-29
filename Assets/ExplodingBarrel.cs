using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : MonoBehaviour
{
    public GameObject ExplodeArea, ExplodeRadius;

    public GameObject Bullet;
    public Transform Sight;
    public int bulletsCount;

    bool exploded;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "PlayerProjectal")
        {
            Arm();
            Destroy(other.gameObject);
        }
        /*else if (other.transform.tag == "EnemyProjectal")
        {
            Explode();
            Destroy(other.gameObject);
        }*/
    }

    void Explode()
    {
        Instantiate(ExplodeArea, transform.position, transform.rotation);
        if (Bullet)
        {
            for (int i = 0; i < bulletsCount; i++)
            {
                Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Random.Range(0, 30f) + i * 360f / bulletsCount);
                GameObject bullet = Instantiate(Bullet, Sight.position, Sight.rotation);
                Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
                bullet_body.AddForce(Sight.up * 16f, ForceMode2D.Impulse);
            }
        }
        Destroy(gameObject);
    }

    void Arm()
    {
        if (!exploded)
        {
            ExplodeRadius.SetActive(true);
            exploded = true;
            Invoke("Explode", 0.4f);
        }
    }
}
