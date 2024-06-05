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
    public int strength;
    public int[] mobsStrength;
    public int roll, tempi;

    void Start()
    {
        //map = GameObject.FindGameObjectWithTag("Map").GetComponent(typeof(Map)) as Map;
        Invoke("NextRound", 5f);
    }

    void NextRound()
    {
        VendingMachineObject.SetActive(false);
        map.playerStats.Nightfall();
        SpawnMobs();
    }

    void SpawnMobs()
    {
        tempi = strength;
        while (tempi > 0)
        {
            Spawn();
        }
        Invoke("CheckForClear", 15f);
    }

    void Spawn()
    {
        do
        {
            SpawnPoint.position = new Vector3(Random.Range(-42f, 42f), Random.Range(-42f, 42f), 0f);
        } while (Vector3.Distance(transform.position, SpawnPoint.position) <= 12.5f);

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
        strength += 50 + strength / 10;
        Invoke("NextRound", 30f);
    }
}
