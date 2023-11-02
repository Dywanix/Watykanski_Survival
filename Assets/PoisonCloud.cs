using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonCloud : MonoBehaviour
{
	public Bullet Explosion, Cloud;
	public GameObject CloudPrefab;

	void Start()
	{
		GameObject bullet = Instantiate(CloudPrefab, transform.position, transform.rotation);
        Cloud = bullet.GetComponent(typeof(Bullet)) as Bullet;
		Cloud.damage =  Explosion.damage * 0.155f;
	}
}
