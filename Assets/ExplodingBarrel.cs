using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrel : MonoBehaviour
{
    public GameObject ExplodeArea;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "PlayerProjectal")
        {
            Explode();
            Destroy(other.gameObject);
        }
        else if (other.transform.tag == "EnemyProjectal")
        {
            Explode();
            Destroy(other.gameObject);
        }
        /*else if (other.transform.tag == "PlayerProjectal")
        {
            Explode();
        }
        else if (other.transform.tag == "PlayerProjectal")
        {
            Explode();
        }*/
    }

    void Explode()
    {
        Instantiate(ExplodeArea, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
