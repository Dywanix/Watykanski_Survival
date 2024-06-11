using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormCloud : MonoBehaviour
{
    public GameObject Enemy;
    public Rigidbody2D enemyBody;
    public float movementSpeed;

    void Update()
    {
        if (!Enemy)
        {
            if (GameObject.FindGameObjectWithTag("Enemy"))
            {
                Enemy = GameObject.FindGameObjectWithTag("Enemy");
                enemyBody = Enemy.GetComponent<Rigidbody2D>();
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, Enemy.transform.position, movementSpeed * Time.deltaTime);
        }
    }
}
