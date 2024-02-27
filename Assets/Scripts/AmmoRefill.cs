using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoRefill : MonoBehaviour
{
    public GameObject Player, Glow;
    public PlayerController playerStats;

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        }

        if (Vector3.Distance(transform.position, Player.transform.position) <= 2.8f)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerStats.AmmoRefill();
            }
        }
        else Glow.SetActive(false);
    }
}
