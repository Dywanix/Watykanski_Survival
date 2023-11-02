using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeEffects : MonoBehaviour
{
	public Bullet Grenade;
	public GameObject PoisonCloud;
	public PulletExplosion smallGrenades;
	public bool venom, small;

	void Start()
	{
		if (venom)
			Grenade.ExplosionRadius = PoisonCloud;
		if (small)
			Grenade.ShardExplosion = smallGrenades;
	}
}
