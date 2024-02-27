using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Bullet collidedBullet;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "PlayerProjectal")
        {
            collidedBullet = other.GetComponent(typeof(Bullet)) as Bullet;
            collidedBullet.Struck();
        }
        else if (other.transform.tag == "EnemyProjectal")
        {
            Destroy(other.gameObject);
        }
    }
}
