using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessoryDispenser : MonoBehaviour
{
    public GameObject Player, Glow;
    public PlayerController playerStats;
    public float cost;

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        }

        if (Vector3.Distance(transform.position, Player.transform.position) <= 2.8f && playerStats.gold >= cost)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                playerStats.eq.Accessories[Random.Range(0, playerStats.eq.Accessories.Length)]++;
                playerStats.SpendGold(cost);
                cost += 4f;
                //Destroy(gameObject);
            }
        }
        else Glow.SetActive(false);
    }
}
