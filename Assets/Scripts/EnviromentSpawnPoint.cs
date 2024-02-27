using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentSpawnPoint : MonoBehaviour
{
    public GameObject[] possibleSpawns;

    void Start()
    {
        Instantiate(possibleSpawns[Random.Range(0, possibleSpawns.Length)], transform.position, transform.rotation);
    }
}
