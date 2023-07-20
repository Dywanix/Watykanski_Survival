using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toxic : MonoBehaviour
{
    public Enemy enemy;
    public GameObject ToxicCloud;

    public float cloudTimer, cloudCooldown, range;

    void Update()
    {
        if (Vector3.Distance(transform.position, enemy.Player.transform.position) <= range)
        {
            cloudTimer -= Time.deltaTime;
            if (cloudTimer <= 0f)
                Fard();
        }
    }

    void Fard()
    {
        cloudTimer += cloudCooldown * Random.Range(0.8f, 1.2f);

        enemy.Sight.rotation = Quaternion.Euler(enemy.Sight.rotation.x, enemy.Sight.rotation.y, enemy.Dir.rotation + Random.Range(0, 360f));
        GameObject cloud = Instantiate(ToxicCloud, enemy.Dir.position, enemy.Sight.rotation);
        Rigidbody2D cloud_body = cloud.GetComponent<Rigidbody2D>();
        cloud_body.AddForce(enemy.Sight.up * 0.22f * Random.Range(0.85f, 1.15f), ForceMode2D.Impulse);
    }
}
