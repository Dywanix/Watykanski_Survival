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
    public Sprite[] sprites, accessorySprites, itemSprites;
    public PlayerController playerStats;

    public int[] Costs, Rolls, Accessory, Item;
    public int accessoryChance, itemChance;
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
        for (int i = 0; i < 7; i++)
        {
            if (Vector3.Distance(ItemForm[i].position, Player.transform.position) <= 1f)
            {
                if (playerStats.gold >= Costs[i])
                    Check(i, true);
                else Check(i, false);
            }
            else Check(i, false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            for (int i = 0; i < 7; i++)
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
        //Rolls[0] = Random.Range(0, sprites.Length + accessoryChance);

        for (int i = 0; i < 7; i++)
        {
            Rolls[i] = Random.Range(0, sprites.Length + accessoryChance + itemChance);
        }
        /*do
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
        } while ((Rolls[3] == Rolls[0] || Rolls[3] == Rolls[1] || Rolls[3] == Rolls[2]) && (Rolls[1] < sprites.Length));*/

        for (int i = 0; i < 7; i++)
        {
            Items[i].SetActive(true);
            Texts[i].SetActive(true);
            if (Rolls[i] >= sprites.Length + accessoryChance)
            {
                Item[i] = Random.Range(0, itemSprites.Length);
                ItemsImages[i].sprite = itemSprites[Item[i]];
                Costs[i] = Random.Range(40, 49 + 1);
                CostText[i].text = Costs[i].ToString("0");
            }
            else if (Rolls[i] >= sprites.Length)
            {
                Accessory[i] = Random.Range(0, accessorySprites.Length);
                ItemsImages[i].sprite = accessorySprites[Accessory[i]];
                Costs[i] = Random.Range(25, 34 + 1);
                CostText[i].text = Costs[i].ToString("0");
            }
            else
            {
                ItemsImages[i].sprite = sprites[Rolls[i]];
                Costs[i] = 25;
                CostText[i].text = Costs[i].ToString("0");
            }
        }
    }

    void Bought(int what)
    {
        playerStats.SpendGold(Costs[what]);
        if (Rolls[what] >= sprites.Length + accessoryChance)
        {
            playerStats.eq.PickUpItem(Item[what]);
        }
        else if (Rolls[what] >= sprites.Length)
        {
            playerStats.eq.Accessories[Accessory[what]]++;
        }
        else
        {
            switch (Rolls[what])
            {
                case 0:
                    playerStats.RestoreHealth(70f);
                    break;
                case 1:
                    playerStats.GainHP(20);
                    break;
                case 2:
                    playerStats.GainDMG(0.027f);
                    break;
                case 3:
                    playerStats.GainFR(0.032f);
                    break;
                case 4:
                    playerStats.GainMS(20f);
                    break;
                case 5:
                    playerStats.GainTools(10);
                    break;
                case 6:
                    playerStats.GainShield(50f);
                    break;
                case 7:
                    playerStats.GainKeys(4);
                    break;
                case 8:
                    playerStats.cooldownReduction += 0.053f;
                    break;
            }
        }
        Items[what].SetActive(false);
        Texts[what].SetActive(false);
    }
}
