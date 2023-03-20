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
    public GameObject[] mobs;
    public int[] mobWeights;
    public PlayerController playerStats;
    public Image DayBar;

    public int day, hordeSize, roll;
    public float time, maxTime, spawnGap, spawnTime;

    void Start()
    {
        day = 1;
        maxTime = 144f;
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
        CurrentState = TimeState.Night;
        maxTime = 230f;
        time = 0;
        hordeSize = 10 + day * 2;
        spawnGap = 2.5f / (1 + 0.12f * day);
        spawnTime = spawnGap * (2 + hordeSize * 0.5f);
        SummonHorde();
    }

    void StartDay()
    {
        day++;
        CurrentState = TimeState.Day;
        maxTime = 144f;
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
}
