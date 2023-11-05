using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public GameObject Player, RoundBar;
    public GameObject[] Players;
    public PlayerController playerStats;
    public Image RoundBarFill;
    public TMPro.TextMeshProUGUI RoundsCount;

    public PrizeChoice Prizes;
    public int rareChance, epicChance;
    int roll;

    void Start()
    {
        Instantiate(Players[PlayerPrefs.GetInt("Class")]);
        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        playerStats.SwapGun(0);
    }

    void Update()
    {
        
    }

    public int PrizeRarity()
    {
        roll = Random.Range(1, 101);
        if (roll <= epicChance)
        {
            // epic
            epicChance = 2;
            return 2;
        }
        else
        {
            if (roll <= epicChance + rareChance)
            {
                // rare
                epicChance += 2 + epicChance / 5;
                rareChance = 11;
                return 1;
            }
            else
            {
                // common
                epicChance += 2 + epicChance / 5;
                rareChance += 4 + rareChance / 3;
                return 0;
            }
        }
    }

    public void ChoosePrize(int rarity)
    {
        Prizes.Open(rarity);
    }
}
