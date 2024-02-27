using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class expire : MonoBehaviour
{
    public float duration = 1f;

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
            Destroy(gameObject);
    }
}
