using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public GameObject Player;
    public PlayerController playerStats;

    public float chaseSpeed, chaseRange;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
    }

    void Update()
    {
        if (playerStats.day)
        {
            if (Vector3.Distance(transform.position, Player.transform.position) <= chaseRange * 1.8f)
                transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, ((5f + chaseSpeed * 2.5f) / (Vector3.Distance(transform.position, Player.transform.position) + 0.15f) + chaseSpeed) * Time.deltaTime);
            else transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, chaseSpeed * Time.deltaTime);
            chaseSpeed *= 1f + (0.15f * Time.deltaTime);
            chaseRange *= 1f + (0.05f * Time.deltaTime);
        }
        else if (Vector3.Distance(transform.position, Player.transform.position) <= chaseRange)
            transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, ((2f + chaseSpeed * 2f) / (Vector3.Distance(transform.position, Player.transform.position) + 0.3f) + chaseSpeed * 0.1f) * Time.deltaTime);
    }
}
