using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TimeState
{
    Day,

    Night,

    Dawn
};

public class Day_Night_Cycle : MonoBehaviour
{
    public TimeState CurrentState = TimeState.Day;
    public Spawner[] spawners;
    private Spawner currentSpawner;
    public GameObject Player, LootCrate;
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
    float temp;

    void Start()
    {
        //Instantiate(Players[PlayerPrefs.GetInt("Class")]);
        Instantiate(Players[0]);

        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        //playerStats.SwapGun(PlayerPrefs.GetInt("Gun"));
        playerStats.SwapGun(0);

        day = 1;
        dayCount.text = day.ToString("0");
        maxTime = 1f;
    }

    void Update()
    {
        switch (CurrentState)
        {
            case (TimeState.Day):
                if (Input.GetKeyDown(KeyCode.H))
                    StartNight();
                break;
            case (TimeState.Night):
                if (bossNight == false)
                {
                    time += Time.deltaTime;
                    if (time >= maxTime)
                        StartDawn();

                    spawnTime -= Time.deltaTime;
                    if (spawnTime <= 0f)
                        Summon();

                    rareSpawnTime -= Time.deltaTime;
                    if (rareSpawnTime <= 0f)
                        SummonRare();
                }
                break;
            case (TimeState.Dawn):
                if (Input.GetKeyDown(KeyCode.H))
                    StartDay();
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
        maxTime = 75f + 5f * day;
        time = 0;

        if (day % bossFrequency == 0)
        {
            SummonBoss();
            bossNight = true;
        }
        else
        {
            bossNight = false;

            hordeSize = 16 + day * 8;

            temp = (day * (day + 1) * 0.8f + 3f * day + 5f) * 10f;
            spawnGap = maxTime / temp;

            temp = (day * (day + 1) * 1f + 4.2f * day + 5f) * 3.75f;
            rareSpawnGap = maxTime / temp;

            spawnTime = spawnGap * (1.5f + hordeSize * 0.5f);
            rareSpawnTime = rareSpawnGap * (1.4f + hordeSize * 0.2f);

            SummonHorde();
        }
    }

    public void StartDawn()
    {
        CurrentState = TimeState.Dawn;

        Invoke("Check", 0.5f);
    }

    public void Check()
    {
        //Check for enemies if Y -> Invoke again, else StartDay();
    }

    public void StartDay()
    {
        playerStats.NewDay();

        Instantiate(LootCrate);

        day++;

        dayCount.text = day.ToString("0");
        CurrentState = TimeState.Day;
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
            roll = Random.Range(0, endlessOne.RareMobs.Length);

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
            roll = Random.Range(0, waves[day - 1].RareMobs.Length);

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
        while (hordeSize > 0)
        {
            if (day > waves.Length)
            {
                roll = Random.Range(0, endlessOne.Mobs.Length);

                currentSpawner = spawners[Random.Range(0, spawners.Length)];

                for (int i = 0; i < day; i += endlessOne.weights[roll])
                {
                    currentSpawner.Spawn(endlessOne.Mobs[roll]);
                    hordeSize -= endlessOne.weights[roll];
                }
            }
            else
            {
                roll = Random.Range(0, waves[day - 1].Mobs.Length);

                currentSpawner = spawners[Random.Range(0, spawners.Length)];

                for (int i = 0; i < day; i += waves[day - 1].weights[roll])
                {
                    currentSpawner.Spawn(waves[day - 1].Mobs[roll]);
                    hordeSize -= waves[day - 1].weights[roll];
                }
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
