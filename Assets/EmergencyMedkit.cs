using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyMedkit : MonoBehaviour
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

        if (Vector3.Distance(transform.position, Player.transform.position) <= 2.2f && playerStats.gold >= cost)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerStats.RestoreHealth(10f + playerStats.maxHealth * 0.2f);
                playerStats.SpendGold(cost);
                Destroy(gameObject);
            }
        }
        else Glow.SetActive(false);
    }
}
