using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Usable : MonoBehaviour
{
    public GameObject Player, Glow;
    public PlayerController playerStats;

    public int thing;

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        }

        if (Vector3.Distance(transform.position, Player.transform.position) <= 2.8f) //&& playerStats.keys >= KeysRequired)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                //playerStats.SpendKeys(KeysRequired);
                Use();
            }
        }
        else Glow.SetActive(false);
    }

    void Use()
    {
        switch (thing)
        {
            case 0: // toxic sludge - restore health at cost of increased toxic level
                playerStats.GainToxicity(1);
                playerStats.RestoreHealth(6f);
                break;
        }
        Destroy(gameObject);
    }
}
