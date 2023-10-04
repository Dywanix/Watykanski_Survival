using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Map map;
    public Transform SpawnPoint;
    public GameObject StartButton, Glow, Player, LeftDoors, RightDoors;
    public GameObject[] Waves, Mobs;

    bool fight;
    public int roundsCount;
    public float waveFrequency, spawnFrequency, roundTimer, spawnTimer;
    public float[] wavesStrength, mobsStrength;
    public float[] WidthRange, HeightRange;
    int roll;
    float roundDuration;

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
    }

    void StartRound()
    {
        StartButton.SetActive(false);
        LeftDoors.SetActive(true);
        fight = true;
        map.RoundBar.SetActive(true);
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
        map.RoundsCount.text = roundsCount.ToString("0");
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
        RightDoors.SetActive(false);
        map.RoundBar.SetActive(false);
        map.playerStats.NewDay();
    }
}
