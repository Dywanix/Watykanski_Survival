using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float duration, damage, poison;
    public bool persist;

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            if (!persist)
                Destroy(gameObject);
        }
    }
}
