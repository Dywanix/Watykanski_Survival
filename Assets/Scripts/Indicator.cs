using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public GameObject[] Bullets;
    public float[] delays;

    int bullet;

    void Start()
    {
        for (int i = 0; i < Bullets.Length; i++)
        {
            Invoke("Area", delays[i]);
        }
    }

    void Area()
    {
        Instantiate(Bullets[bullet], transform.position, transform.rotation);
        bullet++;
    }
}
