using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachineObject : MonoBehaviour
{
    public GameObject Player, Glow;
    public SlotMachine slotMachine;

    void Start()
    {
        slotMachine = GameObject.FindGameObjectWithTag("Map").GetComponent(typeof(SlotMachine)) as SlotMachine;
    }

    void Update()
    {
        if (!Player)
            Player = GameObject.FindGameObjectWithTag("Player");

        if (Vector3.Distance(transform.position, Player.transform.position) <= 3.5f)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                slotMachine.Open();
            }
        }
        else Glow.SetActive(false);
    }
}
