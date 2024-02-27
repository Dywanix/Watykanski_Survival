using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlock : MonoBehaviour
{
    public int hitValue;
    public bool instaDestroy;

    private Bullet collidedBullet;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "PlayerProjectal")
        {
            if (instaDestroy)
                Destroy(other.gameObject);
            else
            {
                collidedBullet = other.GetComponent(typeof(Bullet)) as Bullet;

                for (int i = 0; i < hitValue; i++)
                {
                    collidedBullet.Struck();
                }
            }
        }
    }
}
