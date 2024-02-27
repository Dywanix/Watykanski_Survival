using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prize : MonoBehaviour
{
    public Map map;
    public GameObject Player, Glow;
    public PlayerController playerStats;
    public int rarity, cost;

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
            map = GameObject.FindGameObjectWithTag("Map").GetComponent(typeof(Map)) as Map;
        }

        if (Vector3.Distance(transform.position, Player.transform.position) <= 2.2f && playerStats.gold >= cost)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
                ClaimPrize();
        }
        else Glow.SetActive(false);
    }

    void ClaimPrize()
    {
        playerStats.SpendGold(cost);
        map.ChoosePrize(rarity);
        Destroy(gameObject);
    }
}
