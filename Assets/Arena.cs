using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [Header("Scripts")]
    public Map map;
    public VendingMachine VaningMachineScript;

    [Header("Objects")]
    public Transform SpawnPoint;
    public GameObject[] Mobs, Elites;
    public GameObject VendingMachineObject;

    [Header("Stats")]
    public bool active;
    public int round, strength;
    public int[] mobsStrength, elitesStrength;
    public int roll, tempi, rest, elite, amount;
    public float time, frequency, delay;

    void Start()
    {
        //map = GameObject.FindGameObjectWithTag("Map").GetComponent(typeof(Map)) as Map;
        Invoke("NextRound", 5f);
    }

    void Update()
    {
        if (active)
        {
            delay -= Time.deltaTime;
            if (delay < 0f)
            {
                if (GameObject.FindGameObjectsWithTag("Enemy").Length > 50)
                {
                    Elite(5);
                    delay += 2.5f * frequency;
                    rest--;
                }
                else
                {
                    Spawn();
                    Elite(1);
                    delay += mobsStrength[roll] * frequency;
                    rest -= mobsStrength[roll];
                    if (rest <= 0)
                    {
                        time += 1f;
                        strength += 15 + strength / 12;
                        NextRound();
                        //active = false;
                        //Invoke("CheckForClear", 5f);
                    }
                }
            }
        }
    }

    void NextRound()
    {
        VendingMachineObject.SetActive(false);
        map.playerStats.Nightfall();
        active = true;
        round++;
        SpawnMobs();
    }

    void SpawnMobs()
    {
        tempi = strength / 5;
        rest = strength - tempi;

        frequency = time / rest;
        delay = frequency * tempi * 0.8f;

        while (tempi > 0)
        {
            Spawn();
            tempi -= mobsStrength[roll];
        }

        //Invoke("CheckForClear", 15f);
    }

    void Spawn()
    {
        do
        {
            SpawnPoint.position = new Vector3(Random.Range(-41f, 41f), Random.Range(-41f, 41f), 0f);
        } while (Vector3.Distance(transform.position, SpawnPoint.position) <= 13.5f);

        roll = Random.Range(0, Mobs.Length);

        Instantiate(Mobs[roll], SpawnPoint.position, transform.rotation);
    }

    void Elite(int cooldownReduction)
    {
        elite -= cooldownReduction;
        if (elite <= 0)
        {
            roll = Random.Range(0, Elites.Length);

            Instantiate(Elites[roll], SpawnPoint.position, transform.rotation);

            elite += 10 + elitesStrength[roll] * 3;
        }
    }

    void CheckForClear()
    {
        if (GameObject.FindGameObjectWithTag("Enemy") == null)
            EndRound();
        else Invoke("CheckForClear", 2f);
    }

    void EndRound()
    {
        //fight = false;
        //SpawnPrize();
        //RightDoors.SetActive(false);
        //map.level++;
        //map.RoundBar.SetActive(false);
        map.playerStats.NewDay();
        VendingMachineObject.SetActive(true);
        VaningMachineScript.SetCabinet();
        strength += 60 + strength / 9;
        time += 2f;
        Invoke("NextRound", 30f);
    }
}
