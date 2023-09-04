using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangAxe : MonoBehaviour
{
    public Chase chase;
    //public Rotation rotation;
    public float delay;

    bool returning;

    void Start()
    {
        
    }

    void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0f)
            Return();
    }

    void Return()
    {
        returning = true;
        chase.active = true;
        //rotation.zAngle = 2.6f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player" && returning)
            Destroy(gameObject);
    }
}
