using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grow : MonoBehaviour
{
    public Vector3 scaleChange;

    void Update()
    {
        transform.localScale += scaleChange * Time.deltaTime;
    }
}
