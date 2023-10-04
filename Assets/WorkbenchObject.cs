using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkbenchObject : MonoBehaviour
{
    public GameObject Player, Glow;
    public Workbench workbench;

    void Start()
    {
        workbench = GameObject.FindGameObjectWithTag("Map").GetComponent(typeof(Workbench)) as Workbench;
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
                workbench.Open();
            }
        }
        else Glow.SetActive(false);
    }
}
