using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentSpawnPoint : MonoBehaviour
{
    public GameObject[] possibleSpawns;
    public float chanceToSpawn;

    void Start()
    {
        if (chanceToSpawn > Random.Range(0f, 100f))
            Instantiate(possibleSpawns[Random.Range(0, possibleSpawns.Length)], transform.position, transform.rotation);
    }
}
