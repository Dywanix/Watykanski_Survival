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
    public TMPro.TextMeshProUGUI[] Tooltips, Levels;
    public Sprite BaseBG, RareBG, ItemBG, EffectBG;
    //public int accessoryChance;

    [Header("Stats")]
    public Sprite[] StatSprites;
    public string[] StatTooltips;

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
                //SetRareAccessories();
                SetStats();
                break;
            case 2:
                SetItems();
                break;
            case 3:
                SetEffects();
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
        rolls[0] = Random.Range(0, AccLib.count);

        do
        {
            rolls[1] = Random.Range(0, AccLib.count);
        } while (rolls[1] == rolls[0]);

        do
        {
            rolls[2] = Random.Range(0, AccLib.count);
        } while (rolls[2] == rolls[0] || rolls[2] == rolls[1]);

        for (int i = 0; i < 3; i++)
        {
            ChoicesBackground[i].sprite = BaseBG;
            Choices[i].sprite = AccLib.AccessorySprite[rolls[i]];
            Tooltips[i].text = AccLib.AccessoryTooltip[rolls[i]];
            Levels[i].text = "";
        }
    }

    void SetRareAccessories()
    {
        rolls[0] = Random.Range(AccLib.count, AccLib.count * 2);

        do
        {
            rolls[1] = Random.Range(AccLib.count, AccLib.count * 2);
        } while (rolls[1] == rolls[0]);

        do
        {
            rolls[2] = Random.Range(AccLib.count, AccLib.count * 2);
        } while (rolls[2] == rolls[0] || rolls[2] == rolls[1]);

        for (int i = 0; i < 3; i++)
        {
            ChoicesBackground[i].sprite = RareBG;
            Choices[i].sprite = AccLib.AccessorySprite[rolls[i]];
            Tooltips[i].text = AccLib.AccessoryTooltip[rolls[i]];
            Levels[i].text = "";
        }
    }

    void SetStats()
    {
        rolls[0] = Random.Range(0, StatSprites.Length);

        do
        {
            rolls[1] = Random.Range(0, StatSprites.Length);
        } while (rolls[1] == rolls[0]);

        do
        {
            rolls[2] = Random.Range(0, StatSprites.Length);
        } while (rolls[2] == rolls[0] || rolls[2] == rolls[1]);

        for (int i = 0; i < 3; i++)
        {
            ChoicesBackground[i].sprite = BaseBG;
            Choices[i].sprite = StatSprites[rolls[i]];
            Tooltips[i].text = StatTooltips[rolls[i]];
            Levels[i].text = "";
        }
    }

    void SetItems()
    {
        do
        {
            rolls[0] = Random.Range(0, ItemLib.ItemSprite.Length);
        } while (playerStats.eq.Items[rolls[0]] > 4);

        do
        {
            rolls[1] = Random.Range(0, ItemLib.ItemSprite.Length);
        } while (rolls[1] == rolls[0] || playerStats.eq.Items[rolls[1]] > 4);

        do
        {
            rolls[2] = Random.Range(0, ItemLib.ItemSprite.Length);
        } while (rolls[2] == rolls[0] || rolls[2] == rolls[1] || playerStats.eq.Items[rolls[2]] > 4);

        for (int i = 0; i < 3; i++)
        {
            ChoicesBackground[i].sprite = ItemBG;
            Choices[i].sprite = ItemLib.ItemSprite[rolls[i]];
            Tooltips[i].text = ItemLib.ItemTooltip[rolls[i]];
            if (playerStats.eq.Items[rolls[i]] == 0)
                Levels[i].text = "New";
            else Levels[i].text = playerStats.eq.Items[rolls[i]].ToString("0");
        }
    }

    void SetEffects()
    {
        do
        {
            rolls[0] = Random.Range(0, ItemLib.Effects.Length);
        } while (playerStats.eq.Effects[rolls[0]] > 5);

        do
        {
            rolls[1] = Random.Range(0, ItemLib.Effects.Length);
        } while (rolls[1] == rolls[0] || playerStats.eq.Effects[rolls[1]] > 5);

        do
        {
            rolls[2] = Random.Range(0, ItemLib.Effects.Length);
        } while (rolls[2] == rolls[0] || rolls[2] == rolls[1] || playerStats.eq.Effects[rolls[2]] > 5);

        for (int i = 0; i < 3; i++)
        {
            ChoicesBackground[i].sprite = EffectBG;
            Choices[i].sprite = ItemLib.Effects[rolls[i]].EffectSprite;
            Tooltips[i].text = ItemLib.Effects[rolls[i]].EffectTooltips[playerStats.eq.Effects[rolls[i]]];
            if (playerStats.eq.Effects[rolls[i]] == 0)
                Levels[i].text = "New";
            else Levels[i].text = playerStats.eq.Effects[rolls[i]].ToString("0");
        }
    }

    public void ChoosePrize(int choice)
    {
        if (Rarity == 1)
            playerStats.PickUpStat(rolls[choice]);
        else if (Rarity == 2)
            playerStats.eq.PickUpItem(rolls[choice]);
        else if (Rarity == 3)
            playerStats.eq.PickUpEffect(rolls[choice]);
        else playerStats.eq.Accessories[rolls[choice]]++;
        playerStats.free = true;
        playerStats.menuOpened = false;
        Time.timeScale = 1f;
        Hud.SetActive(false);
    }
}
