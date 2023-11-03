using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prize : MonoBehaviour
{
    public Map map;
    public GameObject Player, Glow;
    public PlayerController playerStats;
    public int rarity, keysRequired;

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
            map = GameObject.FindGameObjectWithTag("Map").GetComponent(typeof(Map)) as Map;
        }

        if (Vector3.Distance(transform.position, Player.transform.position) <= 3.5f && playerStats.keys >= keysRequired)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
                ClaimPrize();
        }
        else Glow.SetActive(false);
    }

    void ClaimPrize()
    {
        playerStats.SpendKeys(keysRequired);
        map.ChoosePrize(rarity);
        Destroy(gameObject);
    }
}
