using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Map map;
    public Transform SpawnPoint;
    public Transform[] ChestLocation;
    public GameObject StartButton, Glow, Player, LeftDoors, RightDoors;
    public GameObject[] Waves, Mobs, Chests, ChestsSpawned;

    bool fight;
    public int roundsCount;
    public float waveFrequency, spawnFrequency, roundTimer, spawnTimer;
    public float[] wavesStrength, mobsStrength;
    public float[] WidthRange, HeightRange;
    int roll, roll2;
    float roundDuration;
    bool spawned;

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

        if (fight)
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
        }

        if (spawned)
        {
            for (int i = 0; i < 3; i++)
            {
                if (ChestsSpawned[i] == null)
                    DeSpawnChests();
            }
        }
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
        spawnTimer = 4f;
        SummonWave();
    }

    void Spawn()
    {
        SpawnPoint.position = new Vector3(transform.position.x + Random.Range(WidthRange[0], WidthRange[1]), transform.position.y + Random.Range(HeightRange[0], HeightRange[1]), 0f);
        roll = Random.Range(0, Mobs.Length);
        Instantiate(Mobs[roll], SpawnPoint.position, transform.rotation);
        spawnTimer += (0.4f + mobsStrength[roll]) / spawnFrequency;
    }

    void SummonWave()
    {
        roll = Random.Range(0, Waves.Length);
        Instantiate(Waves[roll], transform.position, transform.rotation);
        roundDuration = (16f + wavesStrength[roll]) / waveFrequency;
        spawnTimer += (0.8f * wavesStrength[roll]) / waveFrequency;
        roundTimer = roundDuration;
        roundsCount--;
        map.RoundsCount.text = (roundsCount + 1).ToString("0");
    }

    void CeaseSpawn()
    {
        fight = false;
        map.RoundsCount.text = "";

        CheckForClear();
    }

    void CheckForClear()
    {
        if (GameObject.FindGameObjectWithTag("Enemy") == null)
            EndRound();
        else Invoke("CheckForClear", 0.6f);
    }

    void EndRound()
    {
        SpawnChests();
        RightDoors.SetActive(false);
        map.RoundBar.SetActive(false);
        for (int i = 0; i < BarrelSpawn.Length; i++)
        {
            BarrelSpawn[i].active = false;
        }
        map.playerStats.NewDay();
    }

    void SpawnChests()
    {
        ChestsSpawned[0] = Instantiate(Chests[0], ChestLocation[0].position, transform.rotation);
        roll = Random.Range(1, Chests.Length);
        ChestsSpawned[1] = Instantiate(Chests[roll], ChestLocation[1].position, transform.rotation);
        do
        {
            roll2 = Random.Range(1, Chests.Length);
        } while (roll2 == roll);
        ChestsSpawned[2] = Instantiate(Chests[roll2], ChestLocation[2].position, transform.rotation);
        spawned = true;
    }

    void DeSpawnChests()
    {
        spawned = false;
        for (int i = 0; i < 3; i++)
        {
            if (ChestsSpawned[i] != null)
                Destroy(ChestsSpawned[i]);
        }
    }
}
