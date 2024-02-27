using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMP : MonoBehaviour
{
    public Bullet Grenade;

    public float stunDurationIncrease;

    void Update()
    {
        Grenade.stunDuration += stunDurationIncrease * Time.deltaTime;
    }
}
