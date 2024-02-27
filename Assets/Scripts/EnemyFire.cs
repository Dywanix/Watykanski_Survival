using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
	public Enemy enemy;
	public GameObject Projectal;
	public Transform[] Barrels;

	public float frequency, force, fireTimer;

	void Update()
	{
		if (enemy.stun <= 0f)
		{
			fireTimer -= Time.deltaTime * enemy.SpeedEfficiency();
			if (fireTimer <= 0f)
				Fire();
		}
	}

	void Fire()
	{
		fireTimer += frequency;

		for (int i = 0; i < Barrels.Length; i++)
		{
			GameObject bullet = Instantiate(Projectal, Barrels[i].position, Barrels[i].rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrels[i].up * force, ForceMode2D.Impulse);
		}
	}
}
