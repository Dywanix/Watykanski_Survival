using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearProjectals : MonoBehaviour
{
	public int clears;

	private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "EnemyProjectal")
        {
            if (clears > 0)
            {
                clears--;
                Destroy(other.gameObject);
            }
        }
    }
}
