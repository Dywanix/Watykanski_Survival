using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float xAngle, yAngle, zAngle;
    public Transform body;
    void Update()
    {
        body.Rotate(xAngle, yAngle, zAngle, Space.Self);
    }
}
