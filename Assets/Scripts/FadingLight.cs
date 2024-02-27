using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingLight : MonoBehaviour
{
	public UnityEngine.Rendering.Universal.Light2D lightSource;
	public float duration, startingPower;
	float fading;

	void Start()
	{
		fading = startingPower / duration;
	}

	void Update()
	{
		startingPower -= fading * Time.deltaTime;
		lightSource.intensity = startingPower;
	}
}
