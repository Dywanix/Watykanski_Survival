using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [Header("Scripts")]
    public Map map;

    [Header("Objects")]
    public Transform SpawnPoint;
    public GameObject[] Mobs;

    [Header("Stats")]
    public int strength;
    public int[] mobsStrength;
    public int roll;

    void Start()
    {
        //map = GameObject.FindGameObjectWithTag("Map").GetComponent(typeof(Map)) as Map;
        //map.playerStats.Nightfall();
        SpawnMobs();
    }

    void SpawnMobs()
    {
        while (strength > 0)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        do
        {
            SpawnPoint.position = new Vector3(Random.Range(-42f, 42f), Random.Range(-42f, 42f), 0f);
        } while (Vector3.Distance(transform.position, SpawnPoint.position) <= 12.5f);

        roll = Random.Range(0, Mobs.Length);

        Instantiate(Mobs[roll], SpawnPoint.position, transform.rotation);
        strength -= mobsStrength[roll];
    }
}
