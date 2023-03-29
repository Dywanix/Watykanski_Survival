using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour
{
    public GameObject Player, Glow, Hud;
    public PlayerController playerStats;
    public TMPro.TextMeshProUGUI Production, UpgradeCost;
    public Button UpgradeButton;

    public int level, upgradeCost;
    public float electricityProduction, electricityParts;
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

        electricityParts += electricityProduction * Time.deltaTime;
        if (electricityParts >= 1f)
        {
            electricityParts -= 1f;
            playerStats.GainElectricity(1);
            UpdateInfo();
        }
    }

    void UpdateInfo()
    {
        Production.text = "1 / " + (1f / electricityProduction).ToString("0.00") + "s";
        UpgradeCost.text = upgradeCost.ToString("0");

        if (playerStats.scrap >= upgradeCost)
            UpgradeButton.interactable = true;
        else UpgradeButton.interactable = false;
    }

    public void Upgrade()
    {
        playerStats.SpendScrap(upgradeCost);
        electricityProduction += 0.12f;
        upgradeCost += 20;
        level++;
        UpdateInfo();
    }
}
