using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant : MonoBehaviour
{
    public Enemy enemy;
    public GameObject Boulder, Stone;

    public float boulderTossCooldown = 3.5f, additionalStone;

    void Start()
    {
        Invoke("Throw", 6f);
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, enemy.Player.transform.position) <= 8.4f + 1F * enemy.attackDamage)
        {
            if (!enemy.attackTimer)
            {
                boulderTossCooldown -= Time.deltaTime * (1f + 0.01f * enemy.attackDamage);
                if (boulderTossCooldown <= 0f)
                    TossBoulder();
            }
        }
        else
        {
            if (boulderTossCooldown > 0)
                boulderTossCooldown -= Time.deltaTime * 0.168f;
        }
    }

    void TossBoulder()
    {
        enemy.movementSpeed += 0.06f;
        enemy.attackSpeed *= 0.96f;
        enemy.attackDamage += 0.44f;
        enemy.vulnerable += 0.012f;

        enemy.StartCoroutine(enemy.attackTime());

        for (float i = 3f; i < enemy.attackDamage; i += 15f)
        {
            Invoke("Toss", enemy.attackSpeed * 0.09375f * (i + 3));

            boulderTossCooldown += 1.32f + 0.43f * enemy.attackSpeed;
        }
        boulderTossCooldown += 2.64f + 1.3f * enemy.attackSpeed;
    }

    void Toss()
    {
        enemy.Sight.rotation = Quaternion.Euler(enemy.Sight.rotation.x, enemy.Sight.rotation.y, enemy.Dir.rotation + Random.Range(-4.8f, 4.8f));
        GameObject boulder = Instantiate(Boulder, enemy.Dir.position, enemy.Sight.rotation);
        Rigidbody2D boulder_body = boulder.GetComponent<Rigidbody2D>();
        boulder_body.AddForce(enemy.Sight.up * (22.5f + 0.62F * enemy.attackDamage) * Random.Range(0.95f, 1.04f), ForceMode2D.Impulse);
    }

    void Throw()
    {
        if (Vector3.Distance(transform.position, enemy.Player.transform.position) <= 8.5f + 1.18f * enemy.attackDamage)
        {
            enemy.Sight.rotation = Quaternion.Euler(enemy.Sight.rotation.x, enemy.Sight.rotation.y, enemy.Dir.rotation + Random.Range(-7.2f, 7.2f));
            GameObject stone = Instantiate(Stone, enemy.Dir.position, enemy.Sight.rotation);
            Rigidbody2D stone_body = stone.GetComponent<Rigidbody2D>();
            stone_body.AddForce(enemy.Sight.up * (12.7f + 0.22F * enemy.attackDamage) * Random.Range(0.95f, 1.04f), ForceMode2D.Impulse);
            
            if (additionalStone > 1f)
            {
                for (float i = 0; i < additionalStone; i++)
                {
                    enemy.Sight.rotation = Quaternion.Euler(enemy.Sight.rotation.x, enemy.Sight.rotation.y, enemy.Dir.rotation + Random.Range(-7.2f, 7.2f));
                    GameObject stone2 = Instantiate(Stone, enemy.Dir.position, enemy.Sight.rotation);
                    Rigidbody2D stone2_body = stone2.GetComponent<Rigidbody2D>();
                    stone2_body.AddForce(enemy.Sight.up * (12.7f + 0.22F * enemy.attackDamage) * Random.Range(0.95f, 1.04f), ForceMode2D.Impulse);

                    additionalStone -= 1f;
                }
            }

            additionalStone += 0.028f + enemy.attackDamage * 0.014f;
        }
        else
        {
            additionalStone += 0.07f + enemy.attackDamage * 0.028f;
        }

        Invoke("Throw", 3f + 0.4f * enemy.attackSpeed);
    }
}
