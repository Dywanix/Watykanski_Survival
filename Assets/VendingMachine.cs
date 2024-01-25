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

    public Image[] slotImage;
    public Sprite[] otherSprites;
    public TMPro.TextMeshProUGUI[] costValue;

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

    public void Open()
    {
        SetCabinet();
        playerStats.free = false;
        playerStats.menuOpened = true;
        Hud.SetActive(true);
        active = true;
    }

    void SetCabinet()
    {
        for (int i = 0; i < slotFull.Length; i++)
        {
            slotFull[i] = false;
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
        for (int i = 0; i < slotFull.Length; i++)
        {
            if (!slotFull[i])
                slotImage[i].enabled = false;
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
}
