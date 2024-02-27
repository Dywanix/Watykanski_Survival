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
    public GunPickHud Guns;
    public AccessoryPickHud Accessories;
    public int level, rareChance, epicChance, luck, gunChance, range;
    int roll; //range;

    void Start()
    {
        Instantiate(Players[PlayerPrefs.GetInt("Class")]);
        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        playerStats.SwapGun(0);
        gunChance = 5;
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
            epicChance = 4 + luck / 5;
            return 2;
        }
        else
        {
            if (roll <= epicChance + rareChance)
            {
                // rare
                epicChance += 2 + (epicChance + luck) / 4;
                rareChance = 20 + (2 * luck) / 3;
                return 1;
            }
            else
            {
                // common
                epicChance += 2 + (epicChance + luck) / 4;
                rareChance += 6 + (2 * rareChance + 3 * luck) / 4;
                return 0;
            }
        }
    }

    public bool GunCheck()
    {
        gunChance++;
        range = gunChance * gunChance;
        if (range > Random.Range(0, range + 24))
        {
            gunChance = 0;
            return true;
        }
        else return false;
    }

    public void ChoosePrize(int rarity)
    {
        Prizes.Open(rarity);
    }

    public void GunPrize()
    {
        playerStats.free = false;
        playerStats.menuOpened = true;
        Guns.Open(level);
    }

    public void AccessoryPrize()
    {
        playerStats.free = false;
        playerStats.menuOpened = true;
        Accessories.Open();
    }

    public void PickGun(int which)
    {
        playerStats.PickUpGun(Guns.rolls[which]);
        Guns.taken[Guns.rolls[which]] = true;
        playerStats.free = true;
        playerStats.menuOpened = false;
        Guns.Hud.SetActive(false);
    }

    public void PickAccessory(int which)
    {
        playerStats.eq.Accessories[Accessories.rolls[which]]++;
        playerStats.SpendTools(8);
        playerStats.free = true;
        playerStats.menuOpened = false;
        Accessories.Hud.SetActive(false);
    }

    public void PickTools()
    {
        playerStats.GainTools(3);
        playerStats.free = true;
        playerStats.menuOpened = false;
        Accessories.Hud.SetActive(false);
    }
}
