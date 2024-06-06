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
    public GameObject[] Mobs;
    public GameObject VendingMachineObject;

    [Header("Stats")]
    public bool active;
    public int strength;
    public int[] mobsStrength;
    public int roll, tempi, rest;
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
                Spawn();
                delay += mobsStrength[roll] * frequency;
                rest -= mobsStrength[roll];
                if (rest <= 0)
                {
                    active = false;
                    Invoke("CheckForClear", 5f);
                }
            }
        }
    }

    void NextRound()
    {
        VendingMachineObject.SetActive(false);
        map.playerStats.Nightfall();
        active = true;
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
        tempi -= mobsStrength[roll];
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
        strength += 60 + strength / 10;
        time += 2f;
        Invoke("NextRound", 30f);
    }
}
