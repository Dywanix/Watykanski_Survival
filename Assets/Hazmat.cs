using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazmat : MonoBehaviour
{
    public Enemy enemy;
    public LeftOver DeathDrop;

    public GameObject ExplosiveBottle;

    public float bottleTossCooldown, range, additionalBottleChance;
    float bottleTossCooldownMax;

    void Start()
    {
        bottleTossCooldownMax = bottleTossCooldown;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, enemy.Player.transform.position) <= range)
        {
            if (!enemy.attackTimer && enemy.stun <= 0f)
            {
                bottleTossCooldown -= Time.deltaTime;
                if (bottleTossCooldown <= 0f)
                {
                    enemy.GainStun(0.72f);
                    Invoke("TossBottle", 0.36f);
                }
            }
        }
        else
        {
            if (bottleTossCooldown > 0)
                bottleTossCooldown -= Time.deltaTime * 0.15f;
        }
    }

    void TossBottle()
    {
        additionalBottleChance += Random.Range(0.15f, 0.8f);
        if (additionalBottleChance >= 1f)
        {
            additionalBottleChance -= 1f;
            DeathDrop.Count[0]++;
            DeathDrop.ForceMin[0] += 0.23f;
            DeathDrop.ForceMax[0] += 0.4f;
        }

        bottleTossCooldown = bottleTossCooldownMax * Random.Range(0.92f, 1.08f);

        enemy.Sight.rotation = Quaternion.Euler(enemy.Sight.rotation.x, enemy.Sight.rotation.y, enemy.Dir.rotation + Random.Range(-5.7f, 5.7f));
        GameObject bottle = Instantiate(ExplosiveBottle, enemy.Dir.position, enemy.Sight.rotation);
        Rigidbody2D bottle_body = bottle.GetComponent<Rigidbody2D>();
        bottle_body.AddForce(enemy.Sight.up * (19.6f + Vector3.Distance(transform.position, enemy.Player.transform.position) * 0.3f) * Random.Range(0.94f, 1.06f), ForceMode2D.Impulse);
    }
}
