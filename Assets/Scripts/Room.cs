using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Map map;
    public Transform SpawnPoint;
    public Transform[] PossibleSpawns;
    public GameObject Fog, LeftDoors, RightDoors, GunPrize, AccessoryPrize, Chest, BonusChest;
    public GameObject[] Mobs, Prizes, SndPrizes;

    bool fight;
    public int roundStrength, strengthIncrease, mobsCount, roundsCount, mobsLength, strength;
    public float countIncrease, positionX, positionY;
    //public float waveFrequency, spawnFrequency, roundTimer, spawnTimer;
    public int[] mobsStrength, rolled, fatigue;
    int roll, roll2, count, tempi;
    bool viable, started;
    //float roundDuration;

    [Header("Elites")]
    public int eliteMobsLength;
    public float eliteChance;
    public bool eliteEncounter;
    public GameObject[] EliteMobs;
    private Enemy enemySpawned;

    public Barrels[] BarrelSpawn;

    void Start()
    {
        map = GameObject.FindGameObjectWithTag("Map").GetComponent(typeof(Map)) as Map;
        if (Random.Range(0f, 100f) <= eliteChance)
            eliteEncounter = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player" && !started)
            StartRound();
    }

    void StartRound()
    {
        started = true;
        LeftDoors.SetActive(true);
        Fog.SetActive(false);
        fight = true;
        map.RoundBar.SetActive(true);
        for (int i = 0; i < BarrelSpawn.Length; i++)
        {
            BarrelSpawn[i].active = true;
            BarrelSpawn[i].Spawn();
        }
        map.playerStats.Nightfall();
        SpawnWave();
        Invoke("IncreaseLimit", countIncrease);
    }

    void SpawnWave()
    {
        if (eliteEncounter)
        {
            tempi = ((roundsCount - 1) * roundsCount) / 2;
            strength = roundStrength * roundsCount + strengthIncrease * tempi;
            roundsCount = 1;
            SpawnElite();
        }    
        else
        {
            strength = 0;
            count = 0;
            while (strength < roundStrength && count < mobsCount)
            {
                Spawn();
            }
        }

        Invoke("CheckForClear", 3f);
    }

    void Spawn()
    {
        roll = Random.Range(0, PossibleSpawns.Length);
        SpawnPoint.position = new Vector3(PossibleSpawns[roll].position.x + Random.Range(-3f, 3f), PossibleSpawns[roll].position.y + Random.Range(-2f, 2f), 0f);
        viable = false;
        do
        {
            roll = Random.Range(0, mobsLength);
            if (rolled[roll] < fatigue[roll])
                rolled[roll]++;
            else
            {
                viable = true;
                rolled[roll] = 0;
                fatigue[roll]++;
            }
        } while (!viable);
        Instantiate(Mobs[roll], SpawnPoint.position, transform.rotation);
        strength += mobsStrength[roll];
        count++;
    }

    void SpawnElite()
    {
        roll = Random.Range(0, PossibleSpawns.Length);
        SpawnPoint.position = new Vector3(PossibleSpawns[roll].position.x + Random.Range(-3f, 3f), PossibleSpawns[roll].position.y + Random.Range(-2f, 2f), 0f);

        roll = Random.Range(0, eliteMobsLength);
        GameObject elite = Instantiate(EliteMobs[roll], SpawnPoint.position, transform.rotation);
        enemySpawned = elite.GetComponent(typeof(Enemy)) as Enemy;
        enemySpawned.weight = strength;
    }

    void IncreaseLimit()
    {
        mobsCount++;
        countIncrease *= 1.25f;
        if (fight)
            Invoke("IncreaseLimit", countIncrease);
    }

    void CeaseSpawn()
    {
        //fight = false;
        map.RoundsCount.text = "";

        CheckForClear();
    }

    void CheckForClear()
    {
        if (strength < roundStrength)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length < mobsCount)
                Spawn();
            Invoke("CheckForClear", 0.65f);
        }
        else
        {
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
                EndRound();
            else Invoke("CheckForClear", 0.65f);
        }
    }

    void EndRound()
    {
        roundsCount--;
        if (roundsCount == 0)
        {
            fight = false;
            SpawnPrize();
            RightDoors.SetActive(false);
            map.level++;
            map.RoundBar.SetActive(false);
            for (int i = 0; i < BarrelSpawn.Length; i++)
            {
                BarrelSpawn[i].active = false;
            }
            map.playerStats.NewDay();
        }
        else
        {
            roundStrength += strengthIncrease;
            SpawnWave();
        }
    }

    void SpawnPrize()
    {
        Chest.SetActive(true);
        if (map.playerStats.eq.Items[18])
        {
            if (17f + map.luck * 1.4f >= Random.Range(0, 100f))
                BonusChest.SetActive(true);
        }
        /*Prizes[map.PrizeRarity()].SetActive(true);
        roll = Random.Range(0, 9);
        if (roll >= 7)
            SndPrizes[2].SetActive(true);
        else if (roll < 4)
            SndPrizes[0].SetActive(true);
        else SndPrizes[1].SetActive(true);

        if (map.GunCheck())
            GunPrize.SetActive(true);
        else AccessoryPrize.SetActive(true);*/
    }
}
