using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoFactory : MonoBehaviour
{
    public GameObject Player, Glow, Hud;
    public PlayerController playerStats;
    public TMPro.TextMeshProUGUI Production, ToCollect, UpgradeCost;
    public Button UpgradeButton, collect;

    public int level, upgradeCost, ammoToCollect;
    public float ammoProduction, ammoParts;
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

        ammoParts += ammoProduction * Time.deltaTime;
        if (ammoParts >= 1f)
        {
            ammoParts -= 1f;
            ammoToCollect++;
            UpdateInfo();
        }
    }

    void UpdateInfo()
    {
        Production.text = "1 / " + (1f / ammoProduction).ToString("0.0") + "s";
        UpgradeCost.text = upgradeCost.ToString("0");
        ToCollect.text = ammoToCollect.ToString("0");

        if (playerStats.electricity >= upgradeCost)
            UpgradeButton.interactable = true;
        else UpgradeButton.interactable = false;

        if (ammoToCollect > 0)
            collect.interactable = true;
        else collect.interactable = false;
    }

    public void Collect()
    {
        playerStats.PickedUpAmmo();
        playerStats.DisplayAmmo();
        ammoToCollect--;
        UpdateInfo();
    }

    public void Upgrade()
    {
        playerStats.SpendElectricity(upgradeCost);
        ammoProduction += 0.006f;
        upgradeCost += 24;
        level++;
        UpdateInfo();
    }
}
