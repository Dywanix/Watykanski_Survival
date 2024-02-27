using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPick : MonoBehaviour
{
    public GameObject Player, All;
    public PlayerController playerStats;
    public GunsLibrary Library;

    public SpriteRenderer[] Guns, GunsGlow;
    public int[] rolls;
    bool viable;

    void Start()
    {
        Roll();
    }

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        }
        if (Input.GetKeyDown(KeyCode.O))
            Roll();
    }

    public void Roll()
    {
        rolls[0] = Random.Range(0, Library.guns.Length);

        viable = false;
        do
        {
            rolls[1] = Random.Range(0, Library.guns.Length);
            if (rolls[1] != rolls[0])
                viable = true;
        } while (!viable);

        viable = false;
        do
        {
            rolls[2] = Random.Range(0, Library.guns.Length);
            if (rolls[2] != rolls[0] && rolls[2] != rolls[1])
                viable = true;
        } while (!viable);

        Set();
    }

    void Set()
    {
        for (int i = 0; i < 3; i++)
        {
            Guns[i].sprite = Library.guns[rolls[i]].gunSprite;
            GunsGlow[i].sprite = Library.guns[rolls[i]].gunSprite;
        }
    }

    public void Choose(int position)
    {
        playerStats.PickUpGun(rolls[position]);
        All.SetActive(false);
    }
}
