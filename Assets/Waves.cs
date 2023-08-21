using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Waves : MonoBehaviour
{
    public Day_Night_Cycle GN;
    public GameObject StartButton, Glow, Player;
    public WavesSets[] WaveSet;
    public Image DurationFill;
    public TMPro.TextMeshProUGUI WavesLeftCounter;
    float waveDuration, waveTimer;
    int round, wavesCount;
    bool fight;

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
        wavesCount = 5 + round / 3;
        waveDuration = 12f + 0.6f * round;
        SummonWave();
    }

    void SummonWave()
    {
        wavesCount--;
        waveTimer = waveDuration;
        Instantiate(WaveSet[round - 1].Mobs[Random.Range(0, WaveSet[round - 1].Mobs.Length)], transform.position, transform.rotation);
        WavesLeftCounter.text = wavesCount.ToString("");
    }

    void EndRound()
    {
        fight = false;
        StartButton.SetActive(true);
        GN.StartDay();
    }
}
