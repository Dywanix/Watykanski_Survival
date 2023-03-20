using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Workbench : MonoBehaviour
{
    public GameObject Player, Glow, Hud;
    public PlayerController playerStats;
    public TMPro.TextMeshProUGUI gunInfo, upgradeCost, upgradesTotal;
    public Button wench;
    bool active;
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

            if (Input.GetKeyDown(KeyCode.Escape) && active)
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

        gunInfo.text = playerStats.eq.guns[which].gunName;
        upgradeCost.text = playerStats.eq.guns[which].upgradeCost.ToString("0");
        upgradesTotal.text = playerStats.eq.guns[which].upgrades.ToString("0");
        if (playerStats.scrap >= playerStats.eq.guns[which].upgradeCost)
        {
            wench.interactable = true;
        }
        else wench.interactable = false;
    }

    public void Upgrade()
    {
        playerStats.SpendScrap(playerStats.eq.guns[current].upgradeCost);
        playerStats.eq.guns[current].Upgrade();
        UpdateInfo(current);
    }
}
