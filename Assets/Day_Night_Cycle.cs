using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TimeState
{
    Day,

    Night
};

public class Day_Night_Cycle : MonoBehaviour
{
    public TimeState CurrentState = TimeState.Day;
    public Spawner[] spawners;
    private Spawner currentSpawner;
    public GameObject Player;
    public GameObject[] Players, epics, bosses;
    public Wave endlessOne;
    public Wave[] waves;
    public Enemy current;
    public PlayerController playerStats;
    public Image DayBar;
    public TMPro.TextMeshProUGUI dayCount;

    public int day, bossFrequency, hordeSize, roll, epicCharges;
    public float time, maxTime, spawnGap, spawnTime, rareSpawnGap, rareSpawnTime;
    public bool bossNight;

    void Start()
    {
        Instantiate(Players[PlayerPrefs.GetInt("Class")]);

        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        playerStats.SwapGun(PlayerPrefs.GetInt("Gun"));

        day = 1;
        dayCount.text = day.ToString("0");
        maxTime = 50f;
        time = maxTime * 0.8f;
    }

    void Update()
    {
        switch (CurrentState)
        {
            case (TimeState.Day):
                time -= Time.deltaTime;
                if (time <= 0f)
                    StartNight();
                break;
            case (TimeState.Night):
                if (bossNight == false)
                {
                    time += Time.deltaTime;
                    if (time >= maxTime)
                        StartDay();

                    spawnTime -= Time.deltaTime;
                    if (spawnTime <= 0f)
                        Summon();

                    rareSpawnTime -= Time.deltaTime;
                    if (rareSpawnTime <= 0f)
                        SummonRare();
                }
                break;
        }
        if (Input.GetKeyDown(KeyCode.P))
            time += 16f;
        if (Input.GetKeyDown(KeyCode.M))
            time -= 16f;
        DayBar.fillAmount = time / maxTime;
    }

    void StartNight()
    {
        playerStats.day = false;
        CurrentState = TimeState.Night;
        maxTime = 88f + 8f * day;
        time = 0;

        if (day % bossFrequency == 0)
        {
            SummonBoss();
            bossNight = true;
        }
        else
        {
            bossNight = false;

            hordeSize = 16 + day * 7;

            spawnGap = 1.6f / (day * (day + 1) / 5 + 0.5f * day + 1f);
            rareSpawnGap = 4.6f / (day * (day + 1) / 4f + 0.7f * day + 1f);

            spawnTime = spawnGap * (1.5f + hordeSize * 0.5f);
            rareSpawnTime = rareSpawnGap * (1.4f + hordeSize * 0.2f);

            SummonHorde();
        }
    }

    public void StartDay()
    {
        playerStats.NewDay();

        day++;
        dayCount.text = day.ToString("0");
        CurrentState = TimeState.Day;
        maxTime = 60f + 5f * day;
        time = maxTime;
    }

    void Summon()
    {
        if (day > waves.Length)
        {
            roll = Random.Range(0, endlessOne.Mobs.Length);

            currentSpawner = spawners[Random.Range(0, spawners.Length)];

            for (int i = 0; i < day; i += endlessOne.weights[roll])
            {
                currentSpawner.Spawn(endlessOne.Mobs[roll]);
                spawnTime += spawnGap * endlessOne.weights[roll];
            }
        }
        else
        {
            roll = Random.Range(0, waves[day - 1].Mobs.Length);

            currentSpawner = spawners[Random.Range(0, spawners.Length)];

            for (int i = 0; i < day; i += waves[day - 1].weights[roll])
            {
                currentSpawner.Spawn(waves[day - 1].Mobs[roll]);
                spawnTime += spawnGap * waves[day - 1].weights[roll];
            }
        }
    }

    void SummonRare()
    {
        if (day > waves.Length)
        {
            roll = Random.Range(0, endlessOne.RareMobs.Length );

            currentSpawner = spawners[Random.Range(0, spawners.Length)];

            for (int i = 0; i < day; i += endlessOne.rareWeights[roll])
            {
                currentSpawner.Spawn(endlessOne.RareMobs[roll]);
                rareSpawnTime += rareSpawnGap * endlessOne.rareWeights[roll];
                GainEpicCharge(endlessOne.rareWeights[roll]);
            }
        }
        else
        {
            roll = Random.Range(0, waves[day - 1].RareMobs.Length + 1);

            currentSpawner = spawners[Random.Range(0, spawners.Length)];

            for (int i = 0; i < day; i += waves[day - 1].rareWeights[roll])
            {
                currentSpawner.Spawn(waves[day - 1].RareMobs[roll]);
                rareSpawnTime += rareSpawnGap * waves[day - 1].rareWeights[roll];
                GainEpicCharge(waves[day - 1].rareWeights[roll]);
            }
        }
    }

    void GainEpicCharge(int value)
    {
        epicCharges += value;
        epicCharges += day / 3;

        if (epicCharges >= 100)
        {
            currentSpawner = spawners[Random.Range(0, spawners.Length)];
            currentSpawner.Spawn(epics[Random.Range(0, epics.Length)]);
            epicCharges -= 100;
        }
    }

    void SummonHorde()
    {
        if (day > waves.Length)
        {
            roll = Random.Range(0, endlessOne.Mobs.Length);

            currentSpawner = spawners[Random.Range(0, spawners.Length)];

            for (int i = 0; i < hordeSize; i += endlessOne.weights[roll])
            {
                currentSpawner.Spawn(endlessOne.Mobs[roll]);
            }
        }
        else
        {
            roll = Random.Range(0, waves[day - 1].Mobs.Length);

            currentSpawner = spawners[Random.Range(0, spawners.Length)];

            for (int i = 0; i < hordeSize; i += waves[day - 1].weights[roll])
            {
                currentSpawner.Spawn(waves[day - 1].Mobs[roll]);
            }
        }
    }

    void SummonBoss()
    {
        roll = Random.Range(0, bosses.Length);

        currentSpawner = spawners[Random.Range(0, spawners.Length)];

        currentSpawner.Spawn(bosses[roll]);
    }
}
