using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeKing : MonoBehaviour
{
	public Enemy enemy;
	public Rotation rotation;
	public GameObject Projectal;
	public Transform AoEOrigin, AutoBarrel;
	public Transform[] Barrels;

	int roll, lastCast;

	void Start()
	{
		Invoke("AbilityCast", 5f);
		Invoke("AutoFire", 4f);
		Invoke("RotationSpeedCheck", 8f);
	}

	float HealthProcent()
	{
		return (enemy.health / enemy.maxHealth);
	}

	void AutoFire()
	{
		GameObject bullet = Instantiate(Projectal, AutoBarrel.position, AutoBarrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(AutoBarrel.up * 7.7f, ForceMode2D.Impulse);

		Invoke("AutoFire", 0.15f + 0.1f * HealthProcent());
	}

	void RotationSpeedCheck()
	{
		rotation.zAngle = 0.25f - 0.1f * HealthProcent();
		Invoke("RotationSpeedCheck", 8f);
	}

	public void AbilityCast()
	{
		/*do
		{
			roll = Random.Range(0, 2);
		} while (roll == lastCast);
		lastCast = roll;*/

		switch (roll)
		{
			case 0:
				AoE();
				if (HealthProcent() < 0.5f)
				{
					Invoke("AoE", 0.6f);
					Invoke("AbilityCast", 3.5f + 4.5f * HealthProcent());
				}
				else Invoke("AbilityCast", 1.75f + 3.25f * HealthProcent());
				break;
			case 1:
				
				break;
		}
	}

	void AoE()
	{
		AoEOrigin.rotation = Quaternion.Euler(AoEOrigin.rotation.x, AoEOrigin.rotation.y, enemy.Dir.rotation + Random.Range(0f, 360f));
		for (int i = 0; i < Barrels.Length; i++)
		{
			GameObject bullet = Instantiate(Projectal, Barrels[i].position, Barrels[i].rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrels[i].up * 6.8f, ForceMode2D.Impulse);
		}
	}
}
