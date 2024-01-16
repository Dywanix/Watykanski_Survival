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
    public Image[] ChoicesBackground, Choices;
    public TMPro.TextMeshProUGUI[] Tooltips;
    public Sprite BaseBG, RareBG, ItemBG;
    //public int accessoryChance;

    public int[] rolls; //accessories, items;
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
        switch (Rarity)
        {
            case 0:
                SetBaseAccessories();
                break;
            case 1:
                SetBaseAccessories();
                playerStats.GainKeys(1);
                //SetRareAccessories();
                break;
            case 2:
                SetItems();
                break;
        }
    }

    /*void SetCommons()
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
            ChoicesBackground[i].sprite = BaseBG;
            Choices[i].sprite = CommonSprites[rolls[i]];
        }
    }*/

    /*void SetRares()
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
            ChoicesBackground[i].sprite = RareBG;
            if (rolls[i] >= RareSprites.Length)
                Choices[i].sprite = AccLib.AccessorySprite[accessories[i]];
            else
                Choices[i].sprite = RareSprites[rolls[i]];
        }
    }*/

    void SetBaseAccessories()
    {
        do
        {
            rolls[0] = Random.Range(0, AccLib.AccessorySprite.Length);
        } while (playerStats.eq.Items[rolls[0]]);

        do
        {
            rolls[1] = Random.Range(0, AccLib.AccessorySprite.Length);
        } while (rolls[1] == rolls[0]);

        do
        {
            rolls[2] = Random.Range(0, AccLib.AccessorySprite.Length);
        } while (rolls[2] == rolls[0] || rolls[2] == rolls[1]);

        for (int i = 0; i < 3; i++)
        {
            ChoicesBackground[i].sprite = BaseBG;
            Choices[i].sprite = AccLib.AccessorySprite[rolls[i]];
            Tooltips[i].text = AccLib.AccessoryTooltip[rolls[i]];
        }
    }

    void SetRareAccessories()
    {
        do
        {
            rolls[0] = Random.Range(0, AccLib.AccessorySprite.Length);
        } while (playerStats.eq.Items[rolls[0]]);

        do
        {
            rolls[1] = Random.Range(0, AccLib.AccessorySprite.Length);
        } while (rolls[1] == rolls[0]);

        do
        {
            rolls[2] = Random.Range(0, AccLib.AccessorySprite.Length);
        } while (rolls[2] == rolls[0] || rolls[2] == rolls[1]);

        for (int i = 0; i < 3; i++)
        {
            ChoicesBackground[i].sprite = RareBG;
            Choices[i].sprite = AccLib.AccessorySprite[rolls[i]];
            Tooltips[i].text = AccLib.AccessoryTooltip[rolls[i]];
        }
    }

    void SetItems()
    {
        do
        {
            rolls[0] = Random.Range(0, ItemLib.ItemSprite.Length);
        } while (playerStats.eq.Items[rolls[0]]);

        do
        {
            rolls[1] = Random.Range(0, ItemLib.ItemSprite.Length);
        } while (rolls[1] == rolls[0] || playerStats.eq.Items[rolls[1]]);

        do
        {
            rolls[2] = Random.Range(0, ItemLib.ItemSprite.Length);
        } while (rolls[2] == rolls[0] || rolls[2] == rolls[1] || playerStats.eq.Items[rolls[2]]);

        for (int i = 0; i < 3; i++)
        {
            ChoicesBackground[i].sprite = ItemBG;
            Choices[i].sprite = ItemLib.ItemSprite[rolls[i]];
            Tooltips[i].text = ItemLib.ItemTooltip[rolls[i]];
        }
    }

    public void ChoosePrize(int choice)
    {
        switch (Rarity)
        {
            case 0:
                playerStats.eq.Accessories[rolls[choice]]++;
                break;
            case 1:
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
