using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homing : MonoBehaviour
{
    public GameObject Player;
    public Rigidbody2D Body, playerBody;
    public Transform Direction;

    public float frequency, force;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        playerBody = Player.GetComponent<Rigidbody2D>();
        Invoke("Home", frequency);
    }

    void FixedUpdate()
    {
        Vector2 lookDir = playerBody.position - Body.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        Body.rotation = angle;
    }

    void Home()
    {
        Body.AddForce(Direction.up * force, ForceMode2D.Impulse);
        Invoke("Home", frequency);
    }
}
