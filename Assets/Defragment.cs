using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defragment : MonoBehaviour
{
    public GameObject Particle;
    public Rigidbody2D Dir;
    public Transform FragForm;
    public float frequency, frequencyGain, delay, particleForce, particleRange;

    void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0)
            Part();
    }

    void Part()
    {
        FragForm.rotation = Quaternion.Euler(FragForm.rotation.x, FragForm.rotation.y, Dir.rotation + Random.Range(-particleRange, particleRange));
        GameObject particle = Instantiate(Particle, FragForm.position, FragForm.rotation);
        Rigidbody2D particle_body = particle.GetComponent<Rigidbody2D>();
        particle_body.AddForce(FragForm.up * particleForce, ForceMode2D.Impulse);

        delay = frequency * Random.Range(0.8f, 1.2f);
        frequency /= frequencyGain;
    }
}
