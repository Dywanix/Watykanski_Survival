using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public GameObject Player, Open;
    public PlayerControllerLobby playerStats;

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerControllerLobby)) as PlayerControllerLobby;
        }

        if (Vector3.Distance(transform.position, Player.transform.position) <= 4.2f)
        {
            Open.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
                playerStats.EnterElevator();
        }
        else Open.SetActive(false);
    }
}
