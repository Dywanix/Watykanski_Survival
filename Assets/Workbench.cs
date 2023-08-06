using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Workbench : MonoBehaviour
{
    public GameObject Player, Glow, Hud, Upgrades, Others;
    public PlayerController playerStats;
    public TMPro.TextMeshProUGUI specialUpgrades;
    public TMPro.TextMeshProUGUI[] Cost, Info, OthersInfo;
    public Button Gwench;
    public Button[] Buttons;
    public Image special;
    public Image[] images;
    public Sprite[] sprites;

    bool active, golden, viable, others;
    public int[] rolled;
    int current;

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        }
        if (Vector3.Distance(transform.position, Player.transform.position) <= 4.2f)
        {
            if (playerStats.day)
            {
                Glow.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E) && !active)
                {
                    UpdateInfo(playerStats.eq.equipped);
                    playerStats.free = false;
                    Hud.SetActive(true);
                    active = true;
                }
            }
            else Glow.SetActive(false);

            if (Input.GetKeyDown(KeyCode.Escape) && active && !golden)
            {
                playerStats.free = true;
                Hud.SetActive(false);
                active = false;
            }
        }
        else Glow.SetActive(false);
    }

    public void UpdateInfo(int which)
    {
        current = which;

        GunInfo(which);
        GunOtherInfo(which);

        if (!golden)
        {
            for (int i = 0; i < 4; i++)
            {
                if (playerStats.scrap >= playerStats.eq.guns[which].Costs[i])
                    Buttons[i].interactable = true;
                else Buttons[i].interactable = false;
            }

            if (playerStats.eq.guns[which].special >= 1f)
                Gwench.interactable = true;
            else Gwench.interactable = false;
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                Buttons[i].interactable = false;
            }
            Gwench.interactable = false;
        }
    }

    public void OtherStats()
    {
        if (!others)
        {
            Others.SetActive(true);
            GunOtherInfo(current);
            others = true;
        }
        else
        {
            Others.SetActive(false);
            others = false;
        }
    }

    public void GunOtherInfo(int which)
    {
        OthersInfo[0].text = "Reload Time: " + playerStats.eq.guns[which].reloadTime.ToString("0.000") + "s";
        OthersInfo[1].text = "Crit: " + (playerStats.eq.guns[which].critChance * 100f).ToString("0.0") + "% " + (playerStats.eq.guns[which].critDamage * 100f).ToString("0.0");
        OthersInfo[2].text = "Armor Shred " + (playerStats.eq.guns[which].armorShred * 100f).ToString("0.00") + "%";
        OthersInfo[3].text = "Vulnerable " + (playerStats.eq.guns[which].vulnerableApplied * 100f).ToString("0.00") + "%";
        OthersInfo[4].text = "Magazine Size: " + playerStats.eq.guns[which].magazineSize.ToString("0");
        OthersInfo[5].text = "Overload +" + playerStats.eq.guns[which].overload.ToString("0");
        OthersInfo[6].text = "Ammo pack +" + playerStats.eq.guns[which].ammoFromPack.ToString("0");
        OthersInfo[7].text = "Pierce:" + (playerStats.eq.guns[which].pierce - 1).ToString("0") + " - " + (playerStats.eq.guns[which].pierceEfficiency * 100f).ToString("0.0") + "%";
        OthersInfo[8].text = "DoT " + (playerStats.eq.guns[which].DoT * 100f).ToString("0.00") + "%";
        OthersInfo[9].text = "Slow " + playerStats.eq.guns[which].slowDuration.ToString("0.000") + "s";
        OthersInfo[10].text = "Stun " + (playerStats.eq.guns[which].stunChance * 100f).ToString("0.0") + "% " + playerStats.eq.guns[which].stunDuration.ToString("0.00") + "s";
        OthersInfo[11].text = "Slots: " + playerStats.eq.guns[which].MaxSlots.ToString("0");
    }

    void GunInfo(int which)
    {
        for (int i = 0; i < 4; i++)
        {
            Cost[i].text = playerStats.eq.guns[which].Costs[i].ToString("0");
        }
        Info[0].text = playerStats.eq.guns[which].damage.ToString("0.0") + " x " + playerStats.eq.guns[which].BulletsFired().ToString("0");
        Info[1].text = (1 / playerStats.eq.guns[which].fireRate).ToString("0.00") + "/s";
        Info[2].text = (100 - playerStats.eq.guns[which].accuracy).ToString("0.00") + "%";
        Info[3].text = (playerStats.eq.guns[which].penetration * 100).ToString("0") + "%";

        specialUpgrades.text = playerStats.eq.guns[which].special.ToString("0");
        special.fillAmount = playerStats.eq.guns[which].specialCharge;
    }

    public void Upgrade(int which)
    {
        playerStats.SpendScrap(playerStats.eq.guns[current].Costs[which]);
        playerStats.eq.guns[current].Upgrade(which);
        UpdateInfo(current);
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

        golden = true;
        UpdateInfo(current);
        Upgrades.SetActive(true);
    }

    public void SpecialUpgrade(int which)
    {
        playerStats.eq.guns[current].special--;
        playerStats.eq.guns[current].SpecialUpgrade(rolled[which]);
        playerStats.eq.guns[current].MaxSlots++;
        Upgrades.SetActive(false);
        golden = false;
        UpdateInfo(current);
        playerStats.DisplayAmmo();
    }
}
