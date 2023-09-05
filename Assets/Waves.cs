using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Waves : MonoBehaviour
{
    public Day_Night_Cycle GN;
    public GameObject StartButton, Glow, Player;
    public WavesSets[] WaveSet;
    public Wave[] waves;
    public Transform[] SpawnPoints;
    public Image DurationFill;
    public TMPro.TextMeshProUGUI WavesLeftCounter;
    public float bonusUnit;
    public float waveDuration, waveTimer, waveStrength, currentStrength;
    public int round, wavesCount, rolled;
    bool fight;

    void Start()
    {
        bonusUnit = -1f;
    }

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            //playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        }

        if (Vector3.Distance(transform.position, Player.transform.position) <= 1.8f)
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
            if (waveTimer > 0f)
                waveTimer -= Time.deltaTime;
            else
            {
                if (wavesCount > 0)
                {
                    SummonWave();
                }
                else EndRound();
            }
            DurationFill.fillAmount = waveTimer / waveDuration;
        }
    }

    void StartRound()
    {
        GN.StartNight();
        fight = true;
        StartButton.SetActive(false);
        round++;
        wavesCount = 5 + round / 4;
        waveStrength = 26.5f + 6f * round + round * (round + 2);
        bonusUnit -= 0.6f + 0.12f * waveStrength;
        //waveDuration = 14f + 0.8f * round;
        SummonWave();
        BonusGain();
    }

    void SummonWave()
    {
        wavesCount--;
        WavesLeftCounter.text = wavesCount.ToString("");

        rolled = Random.Range(0, WaveSet[round - 1].Mobs.Length);
        currentStrength = WaveSet[round - 1].weights[rolled];
        waveDuration = 12f + 0.1f * currentStrength;
        Instantiate(WaveSet[round - 1].Mobs[rolled], transform.position, transform.rotation);

        bonusUnit += waveStrength - currentStrength;
        /*if (waveStrength > currentStrength)
            bonusUnit += (waveStrength - currentStrength) * (1f + 0.005f * currentStrength);
        else bonusUnit -= (currentStrength - waveStrength) * (0.97f + 0.003f * currentStrength);*/
        BonusCheck();

        waveTimer = waveDuration;
    }

    void BonusGain()
    {
        if (fight)
        {
            bonusUnit += 0.2f + 0.022f * waveStrength;
            Invoke("BonusGain", 3.7f);
            BonusCheck();
        }
    }

    void BonusCheck()
    {
        if (bonusUnit >= 0f)
        {
            rolled = Random.Range(0, waves[round - 1].Mobs.Length);
            Instantiate(waves[round - 1].Mobs[rolled], SpawnPoints[Random.Range(0, SpawnPoints.Length)].position, transform.rotation);
            bonusUnit -= waves[round - 1].weights[rolled];
        }
    }

    void EndRound()
    {
        fight = false;
        StartButton.SetActive(true);
        GN.StartDay();
    }
}
