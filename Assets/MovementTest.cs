using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public Rigidbody2D Body;
    public float movementSpeed;

    float xInput, yInput;

    void Update()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");

        Move();
    }

    void Move()
    {
        Body.AddForce(transform.up * movementSpeed * yInput, ForceMode2D.Impulse);
        Body.AddForce(transform.right * movementSpeed * xInput, ForceMode2D.Impulse);
    }
}
