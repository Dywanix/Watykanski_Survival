using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicBurst : MonoBehaviour
{
    public Enemy enemy;
    public Toxic toxic;

    public GameObject ToxicCloud;

    public float cloudCooldown;
    public int cloudsCount;
    float cloudCooldownMax, countIncrease;

    void Start()
    {
        cloudCooldownMax = cloudCooldown;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, enemy.Player.transform.position) <= toxic.range)
        {
            if (enemy.attackTimer < 0f && enemy.stun <= 0f)
            {
                cloudCooldown -= Time.deltaTime;
                if (cloudCooldown <= 0f)
                {
                    enemy.GainStun(0.4f);
                    Invoke("Fard", 0.32f);
                }
            }
        }
        else
        {
            if (cloudCooldown > 0)
                cloudCooldown -= Time.deltaTime * 0.15f;
        }
    }

    void Fard()
    {
        cloudCooldown = cloudCooldownMax * Random.Range(0.9f, 1.1f);
        toxic.cloudTimer += 0.25f + cloudsCount * 0.025f;

        countIncrease += Random.Range(0.28f, 0.33f) + 1.31f / cloudsCount;
        if (countIncrease >= 1f)
        {
            countIncrease -= 1f;
            cloudsCount++;
        }

        for (int i = 0; i < cloudsCount; i++)
        {
            enemy.Sight.rotation = Quaternion.Euler(enemy.Sight.rotation.x, enemy.Sight.rotation.y, enemy.Dir.rotation + Random.Range(-6f, 6f) + i * 360f / cloudsCount);
            GameObject cloud = Instantiate(ToxicCloud, enemy.Dir.position, enemy.Sight.rotation);
            Rigidbody2D cloud_body = cloud.GetComponent<Rigidbody2D>();
            cloud_body.AddForce(enemy.Sight.up * (0.31f + 0.05f * cloudsCount) * Random.Range(0.88f, 1.12f), ForceMode2D.Impulse);
        }
    }
}
