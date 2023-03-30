using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summoner : MonoBehaviour
{
    public Enemy enemy;
    public GameObject Minion;

    int extraSummon = 3;
    public float summonTimer, summonRate;

    Vector2 spawnLocation;

    void Update()
    {
        if (Vector3.Distance(transform.position, enemy.Player.transform.position) <= 20f)
        {
            enemy.movementSpeed = 0.01f + 0.18f * Vector3.Distance(transform.position, enemy.Player.transform.position);
            summonTimer -= Time.deltaTime;
            if (summonTimer <= 0f)
                Summon();
        }
    }

    void Summon()
    {
        summonTimer += summonRate;
        Spawn();
        extraSummon++;
        if (extraSummon >= 3)
        {
            extraSummon -= 3;
            Spawn();
            summonTimer += 0.2f * summonRate;
        }

        summonRate *= 1.02f;
        enemy.vulnerable += 0.01f;
    }

    void Spawn()
    {
        spawnLocation.x = transform.position.x + Random.Range(-1f, 1f);
        spawnLocation.y = transform.position.y + Random.Range(-1f, 1f);
        Instantiate(Minion, spawnLocation, transform.rotation);
    }
}
