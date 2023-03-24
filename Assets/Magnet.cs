using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public GameObject Player;
    public PlayerController playerStats;

    public float chaseSpeed;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
    }

    void Update()
    {
        if (playerStats.day)
        {
            if (Vector3.Distance(transform.position, Player.transform.position) <= 7f)
                transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, (14f / (Vector3.Distance(transform.position, Player.transform.position) + 0.2f) + chaseSpeed) * Time.deltaTime);
            else transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, chaseSpeed * Time.deltaTime);
            chaseSpeed += 0.2f * Time.deltaTime;
        }
        else if (Vector3.Distance(transform.position, Player.transform.position) <= 5f)
            transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, (10f / (Vector3.Distance(transform.position, Player.transform.position) + 0.3f) + chaseSpeed * 0.1f) * Time.deltaTime);
    }
}
