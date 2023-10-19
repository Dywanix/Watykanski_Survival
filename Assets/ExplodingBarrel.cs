using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : MonoBehaviour
{
    public GameObject ExplodeArea;

    public GameObject Bullet;
    public Transform Sight;
    public int bulletsCount;

    bool exploded;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "PlayerProjectal")
        {
            Explode();
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "EnemyProjectal")
        {
            Explode();
            Destroy(other.gameObject);
        }
        /*else if (other.transform.tag == "PlayerProjectal")
        {
            Explode();
        }
        else if (other.transform.tag == "PlayerProjectal")
        {
            Explode();
        }*/
    }

    void Explode()
    {
        if (!exploded)
        {
            exploded = true;

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
    }
}
