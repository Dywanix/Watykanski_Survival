using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrels : MonoBehaviour
{
    public GameObject[] BarrelPrefab;
    public Transform[] Origins;
    int roll;

    public float frequency;
    public bool active;

    public void Spawn()
    {
        if (active)
        {
            roll = Random.Range(0, Origins.Length);

            GameObject barrel = Instantiate(BarrelPrefab[Random.Range(0, BarrelPrefab.Length)], Origins[roll].position, Origins[roll].rotation);
            Rigidbody2D barrel_body = barrel.GetComponent<Rigidbody2D>();
            barrel_body.AddForce(Origins[roll].up * 6f, ForceMode2D.Impulse);


            Invoke("Spawn", frequency);
        }
    }
}
