using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanningTable : MonoBehaviour
{
    public GameObject Player, Glow, Hud;
    public PlayerController playerStats;
    public TMPro.TextMeshProUGUI[] Cost, Count;
    public Button[] BuyButtons;

    public int[] costs, costsIncrease;
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
                    UpdateInfo();
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
        else Glow.SetActive(false);
    }

    void UpdateInfo()
    {
        for (int i = 0; i < Count.Length; i++)
        {
            Cost[i].text = costs[i].ToString("0");
            Count[i].text = playerStats.eq.Items[i].ToString("0");
            if (costs[i] > playerStats.tools)
                BuyButtons[i].interactable = false;
            else BuyButtons[i].interactable = true;
        }
    }

    public void Buy(int which)
    {
        playerStats.eq.Items[which]++;
        playerStats.SpendTools(costs[which]);
        costs[which] += costsIncrease[which];

        UpdateInfo();
    }
}
