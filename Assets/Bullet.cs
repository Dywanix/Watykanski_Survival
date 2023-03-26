using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float duration, damage, penetration, armorShred, vulnerableApplied, pierceDamage, DoT, crateBonus;
    public int pierce;
    public bool crit;

    void Update()
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
            Destroy(gameObject);
    }

    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Enemy")
        {
            pierce--;
            damage *= pierceDamage;
            if (pierce <= 0)
                Destroy(gameObject);
        }
    }*/

    public void Struck()
    {
        pierce--;
        damage *= pierceDamage;
        if (pierce <= 0)
            Destroy(gameObject);
    }
}
