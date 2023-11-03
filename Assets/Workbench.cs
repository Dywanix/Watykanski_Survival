using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Workbench : MonoBehaviour
{
    public CraftingTable crafting;
    public GameObject Hud, Upgrades;
    public PlayerController playerStats;
    public TMPro.TextMeshProUGUI Tooltip, Parts;
    public TMPro.TextMeshProUGUI[] Cost, Info;
    public Button[] Buttons, gunButtons;
    public GameObject[] Guns;
    public Image[] images, gunImages;
    public Sprite[] sprites;

    bool active, viable, others;
    public int[] rolled;
    int current;

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
        UpdateSprites();
        UpdateInfo(playerStats.eq.equipped);
        playerStats.free = false;
        playerStats.menuOpened = true;
        Hud.SetActive(true);
        active = true;
    }

    public void SwitchTable()
    {
        Hud.SetActive(false);
        active = false;
        crafting.Open();
    }

    public void SwitchGun(int which)
    {
        playerStats.SwapGun(which);

        UpdateSprites();
        UpdateInfo(playerStats.eq.equipped);
    }

    void UpdateSprites()
    {
        for (int i = 0; i < 3; i++)
        {
            if (playerStats.eq.slotFilled[i])
            {
                Guns[i].SetActive(true);
                gunImages[i].sprite = playerStats.eq.guns[i].gunSprite;
            }
            else Guns[i].SetActive(false);
        }
    }

    public void UpdateInfo(int which)
    {
        current = which;

        for (int i = 0; i < 3; i++)
        {
            gunButtons[i].interactable = true;
        }
        gunButtons[current].interactable = false;

        GunInfo(which);

        for (int i = 0; i < 4; i++)
        {
            if (playerStats.tools + playerStats.eq.guns[which].parts >= playerStats.eq.guns[which].Costs[i])
                Buttons[i].interactable = true;
            else Buttons[i].interactable = false;
        }

        if (playerStats.tools >= playerStats.eq.guns[which].LevelCost)
            Buttons[4].interactable = true;
        else Buttons[4].interactable = false;
    }

    public void GunInfo(int which)
    {
        Parts.text = playerStats.eq.guns[which].parts.ToString("0");
        for (int i = 0; i < 4; i++)
        {
            Cost[i].text = playerStats.eq.guns[which].Costs[i].ToString("0");
        }
        Cost[4].text = playerStats.eq.guns[which].LevelCost.ToString("0");
        Info[0].text = playerStats.eq.guns[which].Damage().ToString("0.0");
        Info[1].text = (1 / playerStats.eq.guns[which].fireRate).ToString("0.00") + "/s";
        Info[2].text = (100 - playerStats.eq.guns[which].accuracy).ToString("0.00") + "%";
        Info[3].text = (playerStats.eq.guns[which].penetration * 100).ToString("0") + "%";
        Info[4].text = (playerStats.eq.guns[which].critChance * 100).ToString("0") + "%";
        Info[5].text = playerStats.eq.guns[which].reloadTime.ToString("0.00") + "s";
        Info[6].text = (playerStats.eq.guns[which].range * 100).ToString("0");
        Info[7].text = playerStats.eq.guns[which].force.ToString("0");
        Info[8].text = playerStats.eq.guns[which].MagazineTotalSize().ToString("0");
        Info[9].text = playerStats.eq.guns[which].maxAmmo.ToString("0");

        Info[10].text = (playerStats.eq.guns[which].critDamage * 100).ToString("0") + "%";
        Info[11].text = playerStats.eq.guns[which].BulletsFired().ToString("0");
        Info[12].text = playerStats.eq.guns[which].pierce.ToString("0");
        Info[13].text = (playerStats.eq.guns[which].pierceEfficiency * 100).ToString("0") + "%";
        Info[14].text = playerStats.eq.guns[which].overload.ToString("0");
        Info[15].text = playerStats.eq.guns[which].MaxSlots.ToString("0");
        Info[16].text = (playerStats.eq.guns[which].DoT * 100).ToString("0") + "%";
    }

    public void Upgrade(int which)
    {
        playerStats.eq.guns[current].parts -= playerStats.eq.guns[current].Costs[which];
        if (playerStats.eq.guns[current].parts < 0)
        {
            playerStats.SpendTools(-playerStats.eq.guns[current].parts);
            playerStats.eq.guns[current].parts = 0;
        }
        playerStats.eq.guns[current].Upgrade(which);
        UpdateInfo(current);
        playerStats.DisplayAmmo();
    }

    public void LevelUp()
    {
        playerStats.SpendTools(playerStats.eq.guns[current].LevelCost);
        playerStats.eq.guns[current].LevelUp();
        UpdateInfo(current);
        playerStats.DisplayAmmo();
    }

    public void GoldenWench()
    {
        for (int i = 0; i < 4; i++)
        {
            viable = false;
            while (viable == false)
            {
                rolled[i] = Random.Range(0, 12);
                if (rolled[i] == 11 && playerStats.eq.guns[current].pierce == 1)
                    viable = false;
                else
                {
                    if (i == 0)
                        viable = true;
                    else if (i == 1)
                    {
                        if (rolled[i] != rolled[i-1])
                            viable = true;
                    }
                    else if (i == 2)
                    {
                        if (rolled[i] != rolled[i - 1] && rolled[i] != rolled[i - 2])
                            viable = true;
                    }
                    else
                    {
                        if (rolled[i] != rolled[i - 1] && rolled[i] != rolled[i - 2] && rolled[i] != rolled[i - 3])
                            viable = true;
                    }
                }
            }
            images[i].sprite = sprites[rolled[i]];
        }
        UpdateInfo(current);
        Upgrades.SetActive(true);
    }

    public void SpecialUpgrade(int which)
    {
        playerStats.eq.guns[current].special--;
        //playerStats.eq.guns[current].SpecialUpgrade(rolled[which]);
        playerStats.eq.guns[current].MaxSlots++;
        Upgrades.SetActive(false);
        UpdateInfo(current);
        playerStats.DisplayAmmo();
    }

    public void TooltipOpen(int order)
    {
        Tooltip.text = playerStats.eq.guns[current].UpgradeInfo[order];
    }

    public void TooltipClose()
    {
        Tooltip.text = "";
    }
}
