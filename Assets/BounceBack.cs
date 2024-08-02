using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBack : MonoBehaviour
{
    public float bounceTimer, accuracy;
    public Rigidbody2D Body, playerBody;

    public Bullet thisBullet;

    void Start()
    {
        Invoke("Bounce", bounceTimer);
        playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
    }

    void Bounce()
    {
        Body.velocity = new Vector2(0f, 0f);
        Vector2 lookDir = playerBody.position - Body.position + new Vector2(Random.Range(-accuracy, +accuracy), Random.Range(-accuracy, +accuracy));
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        Body.rotation = angle;
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, Body.rotation);
        Body.AddForce(transform.up * 15f * Random.Range(1f, 1.02f), ForceMode2D.Impulse);

        if (thisBullet)
        {
            thisBullet.pierce++;
            thisBullet.damage *= 1.09f;
        }
    }
}
