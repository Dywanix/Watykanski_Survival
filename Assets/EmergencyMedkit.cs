using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyMedkit : MonoBehaviour
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

        if (Vector3.Distance(transform.position, Player.transform.position) <= 2.8f && playerStats.scrap >= 25f)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerStats.RestoreHealth(5f + playerStats.maxHealth * 0.25f);
                playerStats.SpendScrap(25f);
                Destroy(gameObject);
            }
        }
        else Glow.SetActive(false);
    }
}
