using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    public GameObject Player, Glow, Hud;
    public PlayerController playerStats;
    public Button Lever, Collect;
    public Image[] images;
    public Sprite[] sprites;

    public int[] rolled;
    int prize;
    bool active, secondTry;
    public bool[] locked;

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
        if (playerStats.tokens >= 1)
            Lever.interactable = true;
        else Lever.interactable = false;
    }

    public void Roll()
    {
        playerStats.SpendTokens(1);

        for (int i = 0; i < 3; i++)
        {
            if (!locked[i])
            {
                rolled[i] = Random.Range(0, sprites.Length);
                images[i].sprite = sprites[rolled[i]];
            }
        }

        Collect.interactable = false;

        if (rolled[0] == rolled[1] && rolled[0] == rolled[2])
        {
            Collect.interactable = true;
            prize = rolled[0];
            CollectPrize();
        }
        else if ((rolled[0] == rolled[1]) || (rolled[0] == rolled[2]))
        {
            Collect.interactable = true;
            prize = rolled[0] + 4;
            CollectPrize();
        }
        else if (rolled[1] == rolled[2])
        {
            Collect.interactable = true;
            prize = rolled[1] + 4;
            CollectPrize();
        }

        UpdateInfo();
    }

    public void CollectPrize()
    {
        Collect.interactable = false;

        switch (prize)
        {
            case 0:
                playerStats.GainScrap(50);
                break;
            case 1:
                playerStats.GainTools(12);
                break;
            case 2:
                for (int i = 0; i < 4; i++)
                {
                    playerStats.LevelUp();
                }
                break;
            case 3:
                for (int i = 0; i < 4; i++)
                {
                    playerStats.eq.Accessories[Random.Range(0, playerStats.eq.Accessories.Length)]++;
                }
                break;
            case 4:
                playerStats.GainScrap(12);
                break;
            case 5:
                playerStats.GainTools(3);
                break;
            case 6:
                playerStats.LevelUp();
                break;
            case 7:
                playerStats.eq.Accessories[Random.Range(0, playerStats.eq.Accessories.Length)]++;
                break;
        }

        UpdateInfo();
    }

}
