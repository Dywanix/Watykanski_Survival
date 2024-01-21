using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomToBuy : MonoBehaviour
{
    public Map map;
    public GameObject Player, Glow;
    public PlayerController playerStats;
    public int cost, effect, left;

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
            {
                GainEffect();
                playerStats.SpendGold(cost);
                left--;
                if (left <= 0)
                    Destroy(gameObject);
            }
        }
        else Glow.SetActive(false);
    }

    void GainEffect()
    {
        switch (effect)
        {
            case 0:
                playerStats.GainHP(5);
                break;
            case 1:
                playerStats.GainDMG(0.013f);
                break;
            case 2:
                playerStats.GainFR(0.016f);
                break;
            case 3:
                playerStats.GainMS(2f);
                break;
            case 4:
                playerStats.GainCR(0.027f);
                break;
            case 5:
                playerStats.GainTools(3);
                break;
            case 6:
                playerStats.GainPotions(1);
                break;
            case 7:
                map.ChoosePrize(0);
                break;
        }
    }
}
