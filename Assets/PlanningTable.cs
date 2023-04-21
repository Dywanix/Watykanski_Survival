using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanningTable : MonoBehaviour
{
    public GameObject Player, Glow, Hud;
    public PlayerController playerStats;
    public Button Buy;
    public Button[] Picks;

    public Image[] images;
    public Sprite[] sprites;
    public TMPro.TextMeshProUGUI[] Count;

    public int[] rolled;
    bool active, viable;

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
        if (playerStats.tools >= 5)
            Buy.interactable = true;
        else Buy.interactable = false;
    }

    public void Roll()
    {
        playerStats.SpendTools(5);
        UpdateInfo();

        for (int i = 0; i < 2; i++)
        {
            viable = false;
            do
            {
                rolled[i] = Random.Range(0, sprites.Length);
                if (playerStats.eq.Items[rolled[i]] < 5)
                {
                    if (i > 0)
                    {
                        if (i > 1)
                        {
                            if (rolled[i] != rolled[0] && rolled[i] != rolled[1]) viable = true;
                        }
                        else
                        {
                            if (rolled[i] != rolled[0]) viable = true;
                        }
                    }
                    else viable = true;
                }
            } while (viable == false);

            images[i].sprite = sprites[rolled[i]];
            Count[i].text = playerStats.eq.Items[rolled[i]].ToString("0") + "/5";
            Picks[i].interactable = true;
        }
    }

    public void Pick(int which)
    {
        playerStats.eq.Items[rolled[which]]++;
        Count[which].text = playerStats.eq.Items[rolled[which]].ToString("0") + "/5";
        UpdateInfo();

        for (int i = 0; i < 2; i++)
        {
            Picks[i].interactable = false;
        }

        switch (which)
        {
            case 3:
                playerStats.eq.itemsActivationRate *= 1.22f;
                break;
        }
    }
}
