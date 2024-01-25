using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachineObject : MonoBehaviour
{
    public GameObject Player, Glow;
    public VendingMachine vendingMachine;

    void Start()
    {
        vendingMachine = GameObject.FindGameObjectWithTag("Map").GetComponent(typeof(VendingMachine)) as VendingMachine;
    }

    void Update()
    {
        if (!Player)
            Player = GameObject.FindGameObjectWithTag("Player");

        if (Vector3.Distance(transform.position, Player.transform.position) <= 2.7f)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                vendingMachine.Open();
            }
        }
        else Glow.SetActive(false);
    }
}
