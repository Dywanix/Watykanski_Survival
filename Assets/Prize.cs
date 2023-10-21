using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prize : MonoBehaviour
{
    public Map map;
    public GameObject Player, Glow;
    public int rarity;

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            map = GameObject.FindGameObjectWithTag("Map").GetComponent(typeof(Map)) as Map;
        }

        if (Vector3.Distance(transform.position, Player.transform.position) <= 3.5f)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
                ClaimPrize();
        }
        else Glow.SetActive(false);
    }

    void ClaimPrize()
    {
        map.ChoosePrize(rarity);
        Destroy(gameObject);
    }
}
