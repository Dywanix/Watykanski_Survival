using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float xAngle, yAngle, zAngle;
    public Transform body;
    void Update()
    {
        body.Rotate(xAngle * Time.deltaTime * 360f, yAngle * Time.deltaTime * 360f, zAngle * Time.deltaTime * 360f, Space.Self);
    }
}
