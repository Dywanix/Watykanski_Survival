using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoom : MonoBehaviour
{
    public GameObject[] Rooms;
    GameObject SpawnedRoom;
    public Room room;
    public int waveStrength, strengthGrow, maxMobsCount, wavesCount;
    public float CountIncrease;

    void Start()
    {
        SpawnedRoom = Instantiate(Rooms[Random.Range(0, Rooms.Length)], transform.position, transform.rotation);
        room = SpawnedRoom.GetComponent(typeof(Room)) as Room;
        room.roundStrength = waveStrength;
        room.strengthIncrease = strengthGrow;
        room.mobsCount = maxMobsCount;
        room.roundsCount = wavesCount;
        room.countIncrease = CountIncrease;
    }
}
