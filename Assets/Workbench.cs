using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Workbench : MonoBehaviour
{
    public GameObject Player, Glow, Hud, Upgrades;
    public PlayerController playerStats;
    public TMPro.TextMeshProUGUI gunInfo, upgradeCost, upgradesTotal, specialUpgrades;
    public Button wench, Gwench;
    public Image[] images;
    public Sprite[] sprites;
    bool active, golden, viable;
    public int[] rolled;
    int current;

    void Update()
    {
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
    }

    public void UpdateInfo(int which)
    {
        current = which;

        GunInfo(which);

        upgradeCost.text = playerStats.eq.guns[which].upgradeCost.ToString("0");
        upgradesTotal.text = playerStats.eq.guns[which].upgrades.ToString("0");
        specialUpgrades.text = playerStats.eq.guns[which].specialUpgrades.ToString("0");
        if (!golden)
        {
            if (playerStats.scrap >= playerStats.eq.guns[which].upgradeCost)
            {
                wench.interactable = true;
            }
            else wench.interactable = false;

            if (playerStats.eq.guns[which].specialUpgrades > 0)
            {
                Gwench.interactable = true;
            }
            else Gwench.interactable = false;
        }
        else
        {
            wench.interactable = false;
            Gwench.interactable = false;
        }
    }

    void GunInfo(int which)
    {
        gunInfo.text = playerStats.eq.guns[which].gunName + "\n Damage: " + playerStats.eq.guns[which].damage.ToString("0.0") + "\n Fire Rate: " + playerStats.eq.guns[which].fireRate.ToString("0.000") +
            "\n Reload Time: " + playerStats.eq.guns[which].reloadTime.ToString("0.000") + "\n Penetration: " + (playerStats.eq.guns[which].penetration * 100).ToString("0") + 
            "% \n Accuracy: " + (100 - playerStats.eq.guns[which].accuracy).ToString("0.00") + "%";
    }

    public void Upgrade()
    {
        playerStats.SpendScrap(playerStats.eq.guns[current].upgradeCost);
        playerStats.eq.guns[current].Upgrade();
        UpdateInfo(current);
    }

    public void GoldenWench()
    {
        for (int i = 0; i < 3; i++)
        {
            viable = false;
            while (viable == false)
            {
                rolled[i] = Random.Range(0, 13);
                if (rolled[i] == 2 && playerStats.eq.guns[current].infiniteMagazine)
                    viable = false;
                else if (rolled[i] == 5 && playerStats.eq.guns[current].infiniteAmmo)
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
                    else
                    {
                        if (rolled[i] != rolled[i - 1] && rolled[i] != rolled[i - 2])
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
        playerStats.eq.guns[current].specialUpgrades--;
        playerStats.eq.guns[current].SpecialUpgrade(rolled[which]);

        Upgrades.SetActive(false);
        golden = false;
        UpdateInfo(current);
    }
}
