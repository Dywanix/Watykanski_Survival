using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toxic : MonoBehaviour
{
    public Enemy enemy;
    public GameObject ToxicCloud;

    public float cloudTimer, cloudCooldown;

    void Update()
    {
        if (Vector3.Distance(transform.position, enemy.Player.transform.position) <= 16f)
        {
            cloudTimer -= Time.deltaTime;
            if (cloudTimer <= 0f)
                Fard();
        }
    }

    void Fard()
    {
        cloudTimer += cloudCooldown / (1f + enemy.accuracy * 0.02f);

        enemy.Sight.rotation = Quaternion.Euler(enemy.Sight.rotation.x, enemy.Sight.rotation.y, enemy.Dir.rotation + Random.Range(0, 360f));
        GameObject cloud = Instantiate(ToxicCloud, enemy.Dir.position, enemy.Sight.rotation);
        Rigidbody2D cloud_body = cloud.GetComponent<Rigidbody2D>();
        cloud_body.AddForce(enemy.Sight.up * (0.3f + enemy.accuracy * 0.003f) * Random.Range(0.85f, 1.15f), ForceMode2D.Impulse);
    }
}
