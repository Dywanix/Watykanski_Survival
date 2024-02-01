using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoom : MonoBehaviour
{
    public GameObject[] Rooms, Mobs;
    GameObject SpawnedRoom;
    public Room room;
    public int waveStrength, strengthGrow, maxMobsCount, wavesCount;
    public int[] mobsStrength;
    public float CountIncrease, spawnDelay, positionX, positionY;

    public bool combatRoom;
    public Transform nextSpawnPoint;

    void Start()
    {
        Invoke("InstantiateRoom", spawnDelay);
    }

    void InstantiateRoom()
    {
        SpawnedRoom = Instantiate(Rooms[Random.Range(0, Rooms.Length)], transform.position, transform.rotation);
        if (combatRoom)
        {
            room = SpawnedRoom.GetComponent(typeof(Room)) as Room;
            for (int i = 0; i < Mobs.Length; i++)
            {
                room.Mobs[i] = Mobs[i];
                room.mobsStrength[i] = mobsStrength[i];
            }
            room.mobsLength = Mobs.Length;
            room.roundStrength = waveStrength;
            room.strengthIncrease = strengthGrow;
            room.mobsCount = maxMobsCount;
            room.roundsCount = wavesCount;
            room.countIncrease = CountIncrease;

            if (nextSpawnPoint)
                nextSpawnPoint.position = new Vector2(transform.position.x + room.positionX + positionX, transform.position.y + room.positionY + positionY);
        }
        else
        {
            if (nextSpawnPoint)
                nextSpawnPoint.position = new Vector2(transform.position.x + positionX, transform.position.y + positionY);
        }
    }
}
