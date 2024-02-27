using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burst : MonoBehaviour
{
	public Enemy enemy;
	public GameObject Bullet;
	public Transform[] Barrels;

	public float burstCooldown, burstMaxCooldown;

	void Update()
	{
		burstCooldown -= Time.deltaTime;
		if (burstCooldown <= 0f)
        {
			enemy.GainStun(0.2f);
			Invoke("Proc", 0.16f);
			burstCooldown += burstMaxCooldown;
        }
	}

	void Proc()
	{
		for (int i = 0; i < Barrels.Length; i++)
        {
            GameObject bullet = Instantiate(Bullet, Barrels[i].position, Barrels[i].rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrels[i].up * 10f, ForceMode2D.Impulse);
        }
	}
}
