using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalGrinder : MonoBehaviour
{/*
    public GameObject Player, Glow, Hud;
    public PlayerController playerStats;
    public TMPro.TextMeshProUGUI Production, ToCollect, UpgradeCost;
    public Button UpgradeButton, collect;

    public int level, upgradeCost, scrapToCollect;
    public float scrapProduction, scrapParts;
    bool active;

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
                    playerStats.free = false;
                    Hud.SetActive(true);
                    UpdateInfo();
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
        else Glow.SetActive(false);

        scrapParts += scrapProduction * Time.deltaTime;
        if (scrapParts >= 1f)
        {
            scrapParts -= 1f;
            scrapToCollect++;
            UpdateInfo();
        }
    }

    void UpdateInfo()
    {
        Production.text = "1 / " + (1f / scrapProduction).ToString("0.00") + "s";
        UpgradeCost.text = upgradeCost.ToString("0");
        ToCollect.text = scrapToCollect.ToString("0");

        if (playerStats.electricity >= upgradeCost)
            UpgradeButton.interactable = true;
        else UpgradeButton.interactable = false;

        if (scrapToCollect > 0)
            collect.interactable = true;
        else collect.interactable = false;
    }

    public void Collect()
    {
        playerStats.GainScrap(scrapToCollect);
        scrapToCollect = 0;
        UpdateInfo();
    }

    public void Upgrade()
    {
        playerStats.SpendElectricity(upgradeCost);
        scrapProduction += 0.1f;
        upgradeCost += 25;
        level++;
        UpdateInfo();
    }*/
}
