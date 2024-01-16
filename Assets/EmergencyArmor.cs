using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyArmor : MonoBehaviour
{
    public GameObject Player, Glow;
    public PlayerController playerStats;
    public int cost;

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        }

        if (Vector3.Distance(transform.position, Player.transform.position) <= 3.5f && playerStats.tools >= cost)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerStats.GainShield(20);
                playerStats.SpendTools(cost);
                Destroy(gameObject);
            }
        }
        else Glow.SetActive(false);
    }
}
