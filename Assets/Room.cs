using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Map map;
    public Transform SpawnPoint;
    public GameObject StartButton, Glow, Player, LeftDoors, RightDoors, GunPrize, AccessoryPrize, Chest;
    public GameObject[] Mobs, Prizes, SndPrizes;

    bool fight;
    public int roundStrength, strengthIncrease, mobsCount, roundsCount;
    public float countIncrease;
    //public float waveFrequency, spawnFrequency, roundTimer, spawnTimer;
    public int[] mobsStrength;
    public float[] WidthRange, HeightRange;
    int roll, roll2, strength, count;
    //float roundDuration;

    public Barrels[] BarrelSpawn;

    void Start()
    {
        map = GameObject.FindGameObjectWithTag("Map").GetComponent(typeof(Map)) as Map;
    }
    
    void Update()
    {
        if (!Player)
            Player = GameObject.FindGameObjectWithTag("Player");

        if (Vector3.Distance(transform.position, Player.transform.position) <= 2.5f)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartRound();
            }
        }
        else Glow.SetActive(false);

        /*if (fight)
        {
            roundTimer -= Time.deltaTime;
            map.RoundBarFill.fillAmount = roundTimer / roundDuration;

            if (roundTimer <= 0f)
            {
                if (roundsCount == 0)
                    CeaseSpawn();
                else SummonWave();
            }

            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
                Spawn();
        }*/
    }

    void StartRound()
    {
        StartButton.SetActive(false);
        LeftDoors.SetActive(true);
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
        //spawnTimer = 4f;
        //SummonWave();
    }

    void SpawnWave()
    {
        strength = 0;
        count = 0;
        while (strength < roundStrength && count < mobsCount)
        {
            Spawn();
        }

        Invoke("CheckForClear", 3f);
    }

    void Spawn()
    {
        SpawnPoint.position = new Vector3(transform.position.x + Random.Range(WidthRange[0], WidthRange[1]), transform.position.y + Random.Range(HeightRange[0], HeightRange[1]), 0f);
        roll = Random.Range(0, Mobs.Length);
        Instantiate(Mobs[roll], SpawnPoint.position, transform.rotation);
        //spawnTimer += (0.4f + mobsStrength[roll]) / spawnFrequency;
        strength += mobsStrength[roll];
        count++;
    }

    void IncreaseLimit()
    {
        mobsCount++;
        countIncrease *= 1.25f;
        if (fight)
            Invoke("IncreaseLimit", countIncrease);
    }

    /*void SummonWave()
    {
        roll = Random.Range(0, Waves.Length);
        Instantiate(Waves[roll], transform.position, transform.rotation);
        roundDuration = (16f + wavesStrength[roll]) / waveFrequency;
        spawnTimer += (0.8f * wavesStrength[roll]) / waveFrequency;
        roundTimer = roundDuration;
        roundsCount--;
        map.RoundsCount.text = (roundsCount + 1).ToString("0");
    }*/

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
        Prizes[map.PrizeRarity()].SetActive(true);
        roll = Random.Range(0, 9);
        if (roll >= 7)
            SndPrizes[2].SetActive(true);
        else if (roll < 4)
            SndPrizes[0].SetActive(true);
        else SndPrizes[1].SetActive(true);

        if (map.GunCheck())
            GunPrize.SetActive(true);
        else AccessoryPrize.SetActive(true);
    }
}
