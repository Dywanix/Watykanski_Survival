using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explody : MonoBehaviour
{
    public Enemy enemy;
    public GameObject ExplodeInto;

    void Update()
    {
        if (Vector3.Distance(transform.position, enemy.Player.transform.position) <= 14f)
            enemy.movementSpeed += 1.02f * Time.deltaTime;
        if (Vector3.Distance(transform.position, enemy.Player.transform.position) <= 1.4f)
            Explode();
    }

    void Explode()
    {
        Instantiate(ExplodeInto, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
