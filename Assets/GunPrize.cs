using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPrize : MonoBehaviour
{
    public Map map;
    public GameObject Player, Glow;

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
                PickGun();
        }
        else Glow.SetActive(false);
    }

    void PickGun()
    {
        map.GunPrize();
        Destroy(gameObject);
    }
}
