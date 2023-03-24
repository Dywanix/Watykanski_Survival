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
    public GameObject Player, Gunslinger, Berserker;
    public GameObject[] mobs, bosses;
    public int[] mobWeights;
    public PlayerController playerStats;
    public Image DayBar;

    public int day, hordeSize, roll;
    public float time, maxTime, spawnGap, spawnTime;

    void Start()
    {
        switch (PlayerPrefs.GetString("Class"))
        {
            case "Gunslinger":
                Instantiate(Gunslinger);
                break;
            case "Berserker":
                Instantiate(Berserker);
                break;
        }

        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;

        day = 1;
        maxTime = 100f;
        time = maxTime * 0.6f;
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
                time += Time.deltaTime;
                if (time >= maxTime)
                    StartDay();
                spawnTime -= Time.deltaTime;
                if (spawnTime <= 0f)
                    Summon();
                break;
        }
        if (Input.GetKeyDown(KeyCode.P))
            time += 15f;
        if (Input.GetKeyDown(KeyCode.M))
            time -= 15f;
        DayBar.fillAmount = time / maxTime;
    }

    void StartNight()
    {
        playerStats.day = false;
        CurrentState = TimeState.Night;
        maxTime = 120f + 10f * day;
        time = 0;

        hordeSize = 9 + day * 3;
        spawnGap = 2.4f / (1 + 0.12f * day);
        spawnTime = spawnGap * (2 + hordeSize * 0.5f);

        if (day % 5 == 0)
        {
            while (hordeSize > 12)
            {
                hordeSize -= 12;
                SummonBoss();
            }
        }
        SummonHorde();
    }

    void StartDay()
    {
        playerStats.LevelUp();
        playerStats.day = true;
        day++;
        CurrentState = TimeState.Day;
        maxTime = 80f + 8f * day;
        time = maxTime;
    }

    void SummonHorde()
    {
        roll = Random.Range(0, mobs.Length);

        currentSpawner = spawners[Random.Range(0, spawners.Length)];

        for (int i = 0; i < hordeSize; i += mobWeights[roll])
        {
            currentSpawner.Spawn(mobs[roll]);
        }
    }

    void Summon()
    {
        roll = Random.Range(0, mobs.Length);

        spawnTime += spawnGap * mobWeights[roll];

        currentSpawner = spawners[Random.Range(0, spawners.Length)];

        currentSpawner.Spawn(mobs[roll]);
    }

    void SummonBoss()
    {
        roll = Random.Range(0, bosses.Length);

        currentSpawner = spawners[Random.Range(0, spawners.Length)];

        currentSpawner.Spawn(bosses[roll]);
    }
}
