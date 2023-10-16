using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    public GameObject Player, Hud;
    public PlayerController playerStats;
    public Button Lever;
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

        if (Input.GetKeyDown(KeyCode.Escape) && active)
        {
            Hud.SetActive(false);
            active = false;
            playerStats.free = true;
        }
    }

    public void Open()
    {
        UpdateInfo();
        playerStats.free = false;
        playerStats.menuOpened = true;
        Hud.SetActive(true);
        active = true;
    }

    void UpdateInfo()
    {
        if (playerStats.scrap >= 12)
            Lever.interactable = true;
        else Lever.interactable = false;
    }

    public void Roll()
    {
        playerStats.SpendScrap(12);

        for (int i = 0; i < 3; i++)
        {
            if (!locked[i])
            {
                rolled[i] = Random.Range(0, sprites.Length);
                images[i].sprite = sprites[rolled[i]];
            }
        }

        if (rolled[0] == rolled[1] && rolled[0] == rolled[2])
        {
            prize = rolled[0];
            CollectPrize();
        }
        else if ((rolled[0] == rolled[1]) || (rolled[0] == rolled[2]))
        {
            prize = rolled[0] + 4;
            CollectPrize();
        }
        else if (rolled[1] == rolled[2])
        {
            prize = rolled[1] + 4;
            CollectPrize();
        }

        UpdateInfo();
    }

    public void CollectPrize()
    {
        switch (prize)
        {
            case 0:
                playerStats.GainTokens(4);
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
                playerStats.GainTokens(1);
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
