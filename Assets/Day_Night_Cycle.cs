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
    public GameObject Player, Gunslinger, Berserker, SteamGolem;
    public GameObject[] bosses;
    public Wave endlessOne;
    public Wave[] waves;
    public Enemy current;
    public PlayerController playerStats;
    public Image DayBar;
    public TMPro.TextMeshProUGUI dayCount;

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
            case "SteamGolem":
                Instantiate(SteamGolem);
                break;
        }

        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;

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
                time += Time.deltaTime;
                if (time >= maxTime)
                    StartDay();
                spawnTime -= Time.deltaTime;
                if (spawnTime <= 0f)
                    Summon();
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

        hordeSize = 9 + day * 3;
        spawnGap = 2.4f / (0.995f + 0.125f * day);
        spawnTime = spawnGap * (2 + hordeSize * 0.5f);

        if (day % 5 == 0)
        {
            while (hordeSize >= 13)
            {
                hordeSize -= 13;
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
