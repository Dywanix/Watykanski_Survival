using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaving : MonoBehaviour
{
    public GameObject ThingLeft;
    public float leaveTimer, leaveCooldown;

    void Update()
    {
        leaveTimer -= Time.deltaTime;
        if (leaveTimer <= 0f)
            Leave();
    }
    
    void Leave()
    {
        leaveTimer += leaveCooldown * Random.Range(0.92f, 1.08f);

        Instantiate(ThingLeft, transform.position, transform.rotation);
    }
}
