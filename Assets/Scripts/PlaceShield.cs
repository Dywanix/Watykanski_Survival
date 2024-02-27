using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceShield : MonoBehaviour
{
    public Enemy enemy;
    public GameObject Shield;
    public float cooldown, frequency;

    void Update()
    {
        if (enemy.stun <= 0f)
        {
            cooldown -= Time.deltaTime;
            if (cooldown <= 0f)
                Place();
        }
    }

    void Place()
    {
        enemy.Sight.rotation = Quaternion.Euler(enemy.Sight.rotation.x, enemy.Sight.rotation.y, enemy.Dir.rotation);
        GameObject shield = Instantiate(Shield, enemy.Dir.position, enemy.Sight.rotation);
        Rigidbody2D shield_body = shield.GetComponent<Rigidbody2D>();
        shield_body.AddForce(enemy.Sight.up * 7.5f, ForceMode2D.Impulse);

        cooldown = frequency;
    }
}
