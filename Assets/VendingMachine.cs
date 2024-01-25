using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachine : MonoBehaviour
{
    public GameObject Hud;
    public PlayerController playerStats;
    public ItemsLibrary Lib;
    public AccessoryLibrary ALib;

    public bool[] slotFull;
    public int[] slots, items, costs;
    public TMPro.TextMeshProUGUI Tooltip;

    [Header("Cabinet")]
    public Image[] slotImage;
    public Sprite[] otherSprites;
    public TMPro.TextMeshProUGUI[] costValue;

    [Header("Usage")]
    public TMPro.TextMeshProUGUI playerGold;
    public TMPro.TextMeshProUGUI[] digitValue;
    public bool[] digitWritten;
    public int[] digits;

    bool active, viable;
    int tempi, roll, freeSlots;

    void Update()
    {
        if (!playerStats)
            playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent(typeof(PlayerController)) as PlayerController;

        if (Input.GetKeyDown(KeyCode.Escape) && active)
        {
            Hud.SetActive(false);
            active = false;
            playerStats.free = true;
        }
    }

    public void Open(bool opened)
    {
        if (!opened)
            SetCabinet();
        playerStats.free = false;
        playerStats.menuOpened = true;
        Hud.SetActive(true);
        active = true;
        UpdateCabinet();
    }

    void SetCabinet()
    {
        for (int i = 0; i < slotFull.Length; i++)
        {
            slotFull[i] = false;
            slotImage[i].enabled = true;
        }
        freeSlots = 20;

        tempi = Random.Range(2, 4);
        for (int i = 0; i < tempi; i++)
        {
            SetItem();
            freeSlots--;
        }

        tempi = Random.Range(2, 5);
        for (int i = 0; i < tempi; i++)
        {
            SetAccessory();
            freeSlots--;
        }

        SetOther(2);
        freeSlots--;

        SetOther(3);
        freeSlots--;

        tempi = Random.Range(1 + freeSlots / 3, freeSlots - 1);
        for (int i = 0; i < tempi; i++)
        {
            SetOther(Random.Range(2, 10));
            freeSlots--;
        }

        UpdateCabinet();
    }

    void UpdateCabinet()
    {
        playerGold.text = playerStats.gold.ToString("0");
        for (int i = 0; i < slotFull.Length; i++)
        {
            if (!slotFull[i])
                slotImage[i].enabled = false;
        }
        for (int i = 0; i < 2; i++)
        {
            if (digitWritten[i])
                digitValue[i].text = digits[i].ToString("0");
        }
    }

    public void WriteDigit(int which)
    {
        if (!digitWritten[0])
        {
            digitWritten[0] = true;
            digits[0] = which;
        }
        else if (!digitWritten[1])
        {
            digitWritten[1] = true;
            digits[1] = which;

            Invoke("Check", 0.3f);
        }
        UpdateCabinet();
    }

    void Check()
    {
        for (int i = 0; i < 2; i++)
        {
            digitWritten[i] = false;
            digitValue[i].text = "-";
        }

        tempi = digits[1] + digits[0] * 9 - 10;
        if (tempi < 20)
            GetItem(tempi);

        UpdateCabinet();
    }

    void GetItem(int which)
    {
        if (costs[which] <= playerStats.gold)
        {
            slotFull[tempi] = false;
            playerStats.SpendGold(costs[which]);
            if (slots[which] == 0)
                playerStats.eq.PickUpItem(items[which]);
            else if (slots[which] == 1)
                playerStats.eq.Accessories[items[which]]++;
            else
            {
                switch (slots[which])
                {
                    case 2:
                        playerStats.RestoreHealth(5 + playerStats.maxHealth * 0.1f);
                        break;
                    case 3:
                        playerStats.GainShield(10f);
                        break;
                    case 4:
                        playerStats.GainDMG(0.027f);
                        break;
                    case 5:
                        playerStats.GainFR(0.032f);
                        break;
                    case 6:
                        playerStats.GainMS(4f);
                        break;
                    case 7:
                        playerStats.GainCR(0.053f);
                        break;
                    case 8:
                        playerStats.GainTools(3);
                        break;
                    case 9:
                        playerStats.GainPotions(1);
                        break;
                }
            }
        }
    }

    void SetItem()
    {
        viable = false;
        do
        {
            roll = Random.Range(0, slots.Length);
            if (!slotFull[roll])
            {
                slotFull[roll] = true;
                slots[roll] = 0; // item
                items[roll] = Random.Range(0, Lib.ItemSprite.Length);
                viable = true;
            }
        } while (!viable);

        slotImage[roll].sprite = Lib.ItemSprite[items[roll]];
        costs[roll] = Random.Range(60, 75);
        costValue[roll].text = costs[roll].ToString("0");
    }

    void SetAccessory()
    {
        viable = false;
        do
        {
            roll = Random.Range(0, slots.Length);
            if (!slotFull[roll])
            {
                slotFull[roll] = true;
                slots[roll] = 1; // accessory
                items[roll] = Random.Range(0, ALib.AccessorySprite.Length);
                viable = true;
            }
        } while (!viable);

        slotImage[roll].sprite = ALib.AccessorySprite[items[roll]];
        if (items[roll] >= ALib.count)
            costs[roll] = Random.Range(36, 45);
        else costs[roll] = Random.Range(10, 13);
        costValue[roll].text = costs[roll].ToString("0");
    }

    void SetOther(int what)
    {
        viable = false;
        do
        {
            roll = Random.Range(0, slots.Length);
            if (!slotFull[roll])
            {
                slotFull[roll] = true;
                slots[roll] = what; // 2 - medkit, 3 - shield, 4-7 - stats, 8 - tools, 9 - potion
                viable = true;
            }
        } while (!viable);

        slotImage[roll].sprite = otherSprites[what - 2];
        costs[roll] = Random.Range(20, 24);
        costValue[roll].text = costs[roll].ToString("0");
    }

    public void SlotTooltipOpen(int slot)
    {
        if (slots[slot] == 0)
            Tooltip.text = Lib.ItemTooltip[items[slot]];
        else if (slots[slot] == 1)
            Tooltip.text = ALib.AccessoryTooltip[items[slot]];
        else
        {
                switch (slots[slot])
                {
                    case 2:
                        Tooltip.text = "Restore " + (5 + playerStats.maxHealth * 0.1f).ToString("0") + " Health";
                        break;
                    case 3:
                        Tooltip.text = "Gain 10 Shield";
                        break;
                    case 4:
                        Tooltip.text = "Gain 2,7% Damage Increase";
                        break;
                    case 5:
                        Tooltip.text = "Gain 3,2% Fire Rate Increase";
                        break;
                    case 6:
                        Tooltip.text = "Gain 4% Movement Speed Increase";
                        break;
                    case 7:
                        Tooltip.text = "Gain 5,3% Cooldown Reduction Increase";
                        break;
                    case 8:
                        Tooltip.text = "Gain 3 Tools";
                        break;
                    case 9:
                        Tooltip.text = "Gain 1 Potion";
                        break;
                }
        }
    }

    public void SlotTooltipClose()
    {
        Tooltip.text = "";
    }
}
