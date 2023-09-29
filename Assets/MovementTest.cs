using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTest : MonoBehaviour
{
    public Rigidbody2D Body;
    public float movementSpeed;

    Vector2 move;

    void Update()
    {
        move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    void FixedUpdate()
    {
        Body.velocity = move * movementSpeed * Time.deltaTime;
    }
}
