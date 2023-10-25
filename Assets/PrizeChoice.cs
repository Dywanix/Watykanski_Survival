using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrizeChoice : MonoBehaviour
{
    public Map map;
    public AccessoryLibrary AccLib;
    public ItemsLibrary ItemLib;
    public PlayerController playerStats;
    public GameObject Hud;
    public Image[] Gems, Choices;
    public TMPro.TextMeshProUGUI[] Tooltips;
    public Sprite[] GemsSprites, CommonSprites, RareSprites;
    public string[] CommonTooltips, RareTooltips;
    public int accessoryChance;

    public int[] rolls, accessories, items;
    int Rarity;

    void Update()
    {
        if (!playerStats)
            playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent(typeof(PlayerController)) as PlayerController;
    }

    public void Open(int rarity)
    {
        Rarity = rarity;
        playerStats.free = false;
        playerStats.menuOpened = true;
        Hud.SetActive(true);
        SetChoices();
    }

    void SetChoices()
    {
        for (int i = 0; i < 4; i++)
        {
            Gems[i].sprite = GemsSprites[Rarity];
        }
        switch (Rarity)
        {
            case 0:
                SetCommons();
                break;
            case 1:
                SetRares();
                break;
            case 2:
                SetEpics();
                break;
        }
    }

    void SetCommons()
    {
        rolls[0] = Random.Range(0, CommonSprites.Length);

        do
        {
            rolls[1] = Random.Range(0, CommonSprites.Length);
        } while (rolls[1] == rolls[0]);

        do
        {
            rolls[2] = Random.Range(0, CommonSprites.Length);
        } while (rolls[2] == rolls[0] || rolls[2] == rolls[1]);

        for (int i = 0; i < 3; i++)
        {
            Choices[i].sprite = CommonSprites[rolls[i]];
            Tooltips[i].text = CommonTooltips[rolls[i]];
        }
    }

    void SetRares()
    {
        rolls[0] = Random.Range(0, RareSprites.Length + accessoryChance);
        if (rolls[0] >= RareSprites.Length)
            accessories[0] = Random.Range(0, AccLib.AccessorySprite.Length);

        do
        {
            rolls[1] = Random.Range(0, RareSprites.Length + accessoryChance);
        } while (rolls[1] == rolls[0] && rolls[1] < RareSprites.Length);

        if (rolls[1] >= RareSprites.Length)
        {
            do
            {
                accessories[1] = Random.Range(0, AccLib.AccessorySprite.Length);
            } while (accessories[1] == accessories[0]);
        }

        do
        {
            rolls[2] = Random.Range(0, RareSprites.Length + accessoryChance);
        } while ((rolls[2] == rolls[0] || rolls[2] == rolls[1]) && rolls[2] < RareSprites.Length);

        if (rolls[2] >= RareSprites.Length)
        {
            do
            {
                accessories[2] = Random.Range(0, AccLib.AccessorySprite.Length);
            } while (accessories[2] == accessories[0] || accessories[2] == accessories[1]);
        }

        for (int i = 0; i < 3; i++)
        {
            if (rolls[i] >= RareSprites.Length)
            {
                Choices[i].sprite = AccLib.AccessorySprite[accessories[i]];
                Tooltips[i].text = AccLib.AccessoryTooltip[accessories[i]];
            }
            else
            {
                Choices[i].sprite = RareSprites[rolls[i]];
                Tooltips[i].text = RareTooltips[rolls[i]];
            }
        }
    }

    void SetEpics()
    {
        rolls[0] = Random.Range(0, ItemLib.ItemSprite.Length);

        do
        {
            rolls[1] = Random.Range(0, ItemLib.ItemSprite.Length);
        } while (rolls[1] == rolls[0]);

        do
        {
            rolls[2] = Random.Range(0, ItemLib.ItemSprite.Length);
        } while (rolls[2] == rolls[0] || rolls[2] == rolls[1]);

        for (int i = 0; i < 3; i++)
        {
            Choices[i].sprite = ItemLib.ItemSprite[rolls[i]];
            Tooltips[i].text = ItemLib.ItemTooltip[rolls[i]];
        }
    }

    public void ChoosePrize(int choice)
    {
        switch (Rarity)
        {
            case 0:
                switch (rolls[choice])
                {
                    case 0:
                        playerStats.GainGold(15);
                        break;
                    case 1:
                        playerStats.GainTools(5);
                        break;
                    case 2:
                        playerStats.GainKeys(2);
                        break;
                    case 3:
                        playerStats.RestoreHealth(35);
                        break;
                    case 4:
                        playerStats.GainShield(25);
                        break;
                    case 5:
                        map.rareChance += 7 + map.rareChance / 3;
                        map.epicChance += 3 + map.epicChance / 5;
                        break;
                }
                break;
            case 1:
                if (rolls[choice] >= RareSprites.Length)
                    playerStats.eq.Accessories[accessories[choice]]++;
                else
                {
                    switch (rolls[choice])
                    {
                        case 0:
                            playerStats.GainGold(30);
                            break;
                        case 1:
                            playerStats.GainTools(10);
                            break;
                        case 2:
                            playerStats.GainKeys(4);
                            break;
                        case 3:
                            playerStats.GainHP(20);
                            break;
                        case 4:
                            playerStats.damageBonus += 0.027f;
                            break;
                        case 5:
                            playerStats.fireRateBonus += 0.032f;
                            break;
                        case 6:
                            playerStats.movementSpeed += 20f;
                            break;
                    }
                }
                break;
            case 2:
                playerStats.eq.PickUpItem(rolls[choice]);
                break;
        }
        playerStats.free = true;
        playerStats.menuOpened = false;
        Hud.SetActive(false);
    }
}
