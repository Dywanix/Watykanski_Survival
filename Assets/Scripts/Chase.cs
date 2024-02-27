using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase : MonoBehaviour
{
    public GameObject Player;
    public Rigidbody2D Body, playerBody, Dir;

    public float movementSpeed, accelerationFlat, accelerationProc;
    public bool active;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerBody = Player.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 lookDir = playerBody.position - Body.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        Dir.rotation = angle;
    }

    void Update()
    {
        if (active)
        {
            transform.position = Vector2.MoveTowards(transform.position, Player.transform.position, movementSpeed * Time.deltaTime);
            movementSpeed += (accelerationFlat * Time.deltaTime);
            movementSpeed *= 1f + (accelerationProc * Time.deltaTime);
        }
    }
}
