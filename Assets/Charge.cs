using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{
    public Enemy enemy;

    public float chargeEfficiency, chargeDuration, chargeCooldown, chargeMaxCooldown, chargeStun;

    void Start()
    {
        chargeCooldown = chargeMaxCooldown;
    }

    void Update()
    {
        if (enemy.attackTimer < 0f && enemy.stun <= 0f)
        {
            chargeCooldown -= Time.deltaTime;
            if (chargeCooldown <= 0f)
                ChargeStart();
        }
    }

    void ChargeStart()
    {
        enemy.movementSpeed *= 1f + chargeEfficiency;

        chargeCooldown += chargeMaxCooldown;

        Invoke("ChargeEnd", chargeDuration);
    }

    void ChargeEnd()
    {
        enemy.movementSpeed /= 1f + chargeEfficiency;

        enemy.GainStun(chargeStun);
    }
}
