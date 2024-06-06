using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeEffects : MonoBehaviour
{
	public Bullet Grenade;
	public GameObject PoisonCloud;
	public PulletExplosion smallGrenades;
	public int venom, small;

	void Start()
	{
		if (venom > 0)
			Grenade.ExplosionRadius = PoisonCloud;
		if (small > 0)
			Grenade.ShardExplosion = smallGrenades;
	}
}
