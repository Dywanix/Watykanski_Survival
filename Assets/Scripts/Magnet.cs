using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public GameObject Player;
    public Collider2D coll;
    public PlayerController playerStats;

    public float chaseSpeed, chaseRange, activationTimer = 0.75f;
    bool active, chasing;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        if (playerStats.eq.Items[45] > 0)
            chaseRange *= 1f + 0.25f * playerStats.eq.Items[45];
        Invoke("Activate", activationTimer);
    }

    void Activate()
    {
        coll.enabled = true;
        active = true;
    }

    void Update()
    {
        if (active)
        {
            if (playerStats.magnetizing > 0f && !chasing)
                chasing = true;
            if (chasing)
            {
                transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, (chaseSpeed + (1f + chaseSpeed) / (Vector3.Distance(transform.position, Player.transform.position) + 0.25f)) * Time.deltaTime);
                chaseSpeed *= 1f + (0.25f * Time.deltaTime);
            }
            /*{
                if (Vector3.Distance(transform.position, Player.transform.position) <= chaseRange * 1.8f)
                    transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, ((7f + chaseSpeed * 4f) / (Vector3.Distance(transform.position, Player.transform.position) + 0.15f) + chaseSpeed) * Time.deltaTime);
                else transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, chaseSpeed * Time.deltaTime);
                chaseSpeed *= 1f + (0.15f * Time.deltaTime);
                chaseRange *= 1f + (0.05f * Time.deltaTime);
            }
            else if (Vector3.Distance(transform.position, Player.transform.position) <= chaseRange)
                transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, ((3f + chaseSpeed * 2.5f) / (Vector3.Distance(transform.position, Player.transform.position) + 0.3f) + chaseSpeed * 0.1f) * Time.deltaTime);*/
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "PickUp")
            chasing = true;
    }
}
