using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caster : MonoBehaviour
{
    public Enemy enemy;
    public GameObject[] Ability;
    public float[] abilityCastTime, abilityCooldown;

    int roll;

    void Start()
    {
        Invoke("Cast", 7.5f);
    }

    void Cast()
    {
        roll = Random.Range(0, Ability.Length);

        enemy.GainStun(abilityCastTime[roll]);
        Instantiate(Ability[roll], transform.position, enemy.Sight.rotation);

        Invoke("Cast", abilityCooldown[roll] / enemy.cooldownReduction);
    }
}
