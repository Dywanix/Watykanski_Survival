using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeEffects : MonoBehaviour
{
	public Bullet Grenade;
	public GameObject[] PoisonCloud;
	public PulletExplosion smallGrenades;
	public int venom, small;

	void Start()
	{
		if (venom > 0)
			Grenade.ExplosionRadius = PoisonCloud[venom - 1];
		if (small > 0)
        {
			Grenade.ShardExplosion = smallGrenades;
			smallGrenades.bulletsCount[0] = 2 + 1 * small;
			smallGrenades.damageEfficiency[0] = 0.4f + 0.15f * small;
		}
	}
}
