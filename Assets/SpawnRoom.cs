using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoom : MonoBehaviour
{
    public GameObject[] Rooms;

    void Start()
    {
        Instantiate(Rooms[Random.Range(0, Rooms.Length)], transform.position, transform.rotation);
    }
}
