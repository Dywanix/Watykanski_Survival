using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public GameObject[] Glows, Items, Texts;
    public GameObject Player, Shop;
    public Transform[] ItemForm;
    public TMPro.TextMeshProUGUI[] CostText;
    public SpriteRenderer[] ItemsImages;
    public Sprite[] sprites, accessorySprites;
    public PlayerController playerStats;

    public int[] Costs, MinCosts, MaxCosts, Rolls, Accessory;
    public int accessoryChance;
    int roll;
    public bool[] aviable;

    void Start()
    {
        SetShop();
    }

    public void Open()
    {
        Shop.SetActive(true);
        SetShop();
    }

    public void Close()
    {
        Shop.SetActive(false);
    }

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        }
        for (int i = 0; i < 4; i++)
        {
            if (Vector3.Distance(ItemForm[i].position, Player.transform.position) <= 1f)
            {
                if (playerStats.scrap >= Costs[i])
                    Check(i, true);
                else Check(i, false);
            }
            else Check(i, false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            for (int i = 0; i < 4; i++)
            {
                if (aviable[i])
                {
                    Bought(i);
                }
            }
        }
    }

    void Check(int which, bool able)
    {
        if (able)
        {
            Glows[which].SetActive(true);
            aviable[which] = true;
        }
        else 
        {
            Glows[which].SetActive(false);
            aviable[which] = false;
        }
    }

    void SetShop()
    {
        Rolls[0] = Random.Range(0, sprites.Length + accessoryChance);

        do
        {
            Rolls[1] = Random.Range(0, sprites.Length + accessoryChance);
        } while ((Rolls[1] == Rolls[0]) && (Rolls[1] < sprites.Length));

        do
        {
            Rolls[2] = Random.Range(0, sprites.Length + accessoryChance);
        } while ((Rolls[2] == Rolls[0] || Rolls[2] == Rolls[1]) && (Rolls[1] < sprites.Length));

        do
        {
            Rolls[3] = Random.Range(0, sprites.Length + accessoryChance);
        } while ((Rolls[3] == Rolls[0] || Rolls[3] == Rolls[1] || Rolls[3] == Rolls[2]) && (Rolls[1] < sprites.Length));

        for (int i = 0; i < 4; i++)
        {
            Items[i].SetActive(true);
            Texts[i].SetActive(true);
            if (Rolls[i] >= sprites.Length)
            {
                Accessory[i] = Random.Range(0, accessorySprites.Length);
                ItemsImages[i].sprite = accessorySprites[Accessory[i]];
                Costs[i] = Random.Range(19, 25 + 1);
                CostText[i].text = Costs[i].ToString("0");
            }
            else
            {
                ItemsImages[i].sprite = sprites[Rolls[i]];
                Costs[i] = Random.Range(MinCosts[i], MaxCosts[i] + 1);
                CostText[i].text = Costs[i].ToString("0");
            }
        }
    }

    void Bought(int what)
    {
        playerStats.SpendScrap(Costs[what]);
        if (Rolls[what] >= sprites.Length)
        {
            playerStats.eq.Accessories[Accessory[what]]++;
        }
        else
        {
            switch (Rolls[what])
            {
                case 0:
                    playerStats.RestoreHealth(9f + playerStats.maxHealth * 0.3f);
                    break;
                case 1:
                    playerStats.GainHP(8);
                    break;
                case 2:
                    playerStats.damageBonus += 0.02f;
                    break;
                case 3:
                    playerStats.fireRateBonus += 0.03f;
                    break;
                case 4:
                    playerStats.movementSpeed += 0.05f;
                    break;
                case 5:
                    playerStats.GainTools(5);
                    break;
                case 6:
                    playerStats.GainShield(playerStats.maxShield * 0.25f);
                    break;
                case 7:
                    playerStats.AmmoPack();
                    break;
                case 8:
                    playerStats.LevelUp();
                    break;
            }
        }
        Items[what].SetActive(false);
        Texts[what].SetActive(false);
    }
}
