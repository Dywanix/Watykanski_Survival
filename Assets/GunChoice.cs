using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunChoice : MonoBehaviour
{
    public GunPick Pick;
    public GameObject Player, Glow;
    public int position;

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        if (Vector3.Distance(transform.position, Player.transform.position) <= 2.8f)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                Pick.Choose(position);
                Glow.SetActive(false);
            }
        }
        else Glow.SetActive(false);
    }
}
