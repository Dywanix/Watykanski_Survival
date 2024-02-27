using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassSelect : MonoBehaviour
{
    public GameObject Player, thisObject, Glow;
    public PlayerControllerLobby playerStats;
    public int classID;

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerControllerLobby)) as PlayerControllerLobby;
        }

        if (Vector3.Distance(transform.position, Player.transform.position) <= 2.5f)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerStats.ChangeClass(classID);
                thisObject.SetActive(false);
                Glow.SetActive(false);
            }
        }
        else Glow.SetActive(false);
    }
}
