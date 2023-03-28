using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Workbench : MonoBehaviour
{
    public GameObject Player, Glow, Hud, Upgrades;
    public PlayerController playerStats;
    public TMPro.TextMeshProUGUI specialUpgrades;
    public TMPro.TextMeshProUGUI[] Cost, Info;
    public Button Gwench;
    public Button[] Buttons;
    public Image special;
    public Image[] images;
    public Sprite[] sprites;
    bool active, golden, viable;
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

    void GunInfo(int which)
    {
        for (int i = 0; i < 4; i++)
        {
            Cost[i].text = playerStats.eq.guns[which].Costs[i].ToString("0");
        }
        Info[0].text = playerStats.eq.guns[which].damage.ToString("0.0") + " x " + playerStats.eq.guns[which].bulletSpread.ToString("0");
        Info[1].text = (1 / playerStats.eq.guns[which].fireRate).ToString("0.00") + "/s";
        Info[2].text = (100 - playerStats.eq.guns[which].accuracy).ToString("0.00") + "%";
        Info[3].text = (playerStats.eq.guns[which].penetration * 100).ToString("0") + "%";

        specialUpgrades.text = playerStats.eq.guns[which].special.ToString("0");
        special.fillAmount = playerStats.eq.guns[which].specialCharge;
    }

    public void Upgrade(int which)
    {
        playerStats.SpendScrap(playerStats.eq.guns[current].Costs[which]);
        for (int i = 0; i < playerStats.eq.guns.Length; i++)
        {
            playerStats.eq.guns[i].GainSpecialCharge(0.025f + playerStats.eq.guns[current].Costs[which] * 0.00002f);
        }
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
                rolled[i] = Random.Range(0, 15);
                if (rolled[i] == 2 && playerStats.eq.guns[current].infiniteMagazine)
                    viable = false;
                else if (rolled[i] == 5 && playerStats.eq.guns[current].infiniteAmmo)
                    viable = false;
                else if ((rolled[i] == 8 && playerStats.eq.guns[current].infiniteMagazine) || (rolled[i] == 8 && playerStats.eq.guns[current].infiniteAmmo))
                    viable = false;
                else if (rolled[i] == 14 && playerStats.eq.guns[current].pierce == 1)
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

        Upgrades.SetActive(false);
        golden = false;
        UpdateInfo(current);
        playerStats.DisplayAmmo();
    }
}
