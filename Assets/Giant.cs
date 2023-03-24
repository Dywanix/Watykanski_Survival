using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Giant : MonoBehaviour
{
    public Enemy enemy;
    public GameObject Boulder;

    public float boulderTossCooldown = 3.5f;

    void Update()
    {
        if (Vector3.Distance(transform.position, enemy.Player.transform.position) <= 9.3f + 0.1F * enemy.attackDamage)
        {
            if (!enemy.attackTimer)
            {
                boulderTossCooldown -= Time.deltaTime;
                if (boulderTossCooldown <= 0f)
                    TossBoulder();
            }
        }
        else
        {
            if (boulderTossCooldown > 0)
                boulderTossCooldown -= Time.deltaTime * 0.16f;
        }
    }

    void TossBoulder()
    {
        enemy.movementSpeed += 0.16f;
        enemy.attackSpeed *= 0.94f;
        enemy.attackDamage *= 1.05f;
        enemy.vulnerable += 0.02f;

        enemy.StartCoroutine(enemy.attackTime());

        Invoke("Toss", enemy.attackSpeed * 0.22f);

        boulderTossCooldown += 4f + 1.8f * enemy.attackSpeed;
    }

    void Toss()
    {
        enemy.Sight.rotation = Quaternion.Euler(enemy.Sight.rotation.x, enemy.Sight.rotation.y, enemy.Dir.rotation + Random.Range(-4.8f, 4.8f));
        GameObject boulder = Instantiate(Boulder, enemy.Dir.position, enemy.Sight.rotation);
        Rigidbody2D boulder_body = boulder.GetComponent<Rigidbody2D>();
        boulder_body.AddForce(enemy.Sight.up * (20.1f + 0.15F * enemy.attackDamage), ForceMode2D.Impulse);
    }
}
