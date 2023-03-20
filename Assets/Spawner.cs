using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform spawnPoint;

    Vector2 spawn_variable;

    public void Spawn(GameObject Mobile)
    {
        spawn_variable.x = spawnPoint.position.x + Random.Range(-2.1f, 2.1f);
        spawn_variable.y = spawnPoint.position.y + Random.Range(-2.1f, 2.1f);
        GameObject Enemy = Instantiate(Mobile, spawn_variable, spawnPoint.rotation);
    }
}
