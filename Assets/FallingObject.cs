using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public Transform form;
    public float jump, fall, duration;

    void Start()
    {
        fall = jump * 2 / duration;
    }

    void Update()
    {
        form.position = new Vector3(form.position.x, form.position.y + jump * Time.deltaTime, 0);
        jump -= fall * Time.deltaTime;
    }
}
