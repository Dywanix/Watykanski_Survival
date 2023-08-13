using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini_Boss1 : MonoBehaviour
{
    public GameObject Rocket;
    public Transform[] ShoulderBarrels;

    int currentShoulder;

    void Start()
    {
        Invoke("ShoulderFire", 4f);
    }

    void ShoulderFire()
    {
        currentShoulder = Random.Range(0, 2);

        FireFromShoulder();

        if (currentShoulder == 0)
            currentShoulder = 1;
        else currentShoulder = 0;

        Invoke("FireFromShoulder", 0.75f);

        Invoke("ShoulderFire", 9.15f);
    }

    void FireFromShoulder()
    {
        GameObject bullet = Instantiate(Rocket, ShoulderBarrels[currentShoulder].position, ShoulderBarrels[currentShoulder].rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(ShoulderBarrels[currentShoulder].up * 8f * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
    }
}
