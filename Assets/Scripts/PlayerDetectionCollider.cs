using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectionCollider : MonoBehaviour
{
    public PlayerController playerStats;

    private void OnTriggerEnter2D(Collider2D other)
    {
        playerStats.Collided(other);
    }
}
