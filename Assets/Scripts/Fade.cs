using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
	public SpriteRenderer image;
	public float duration, startingFade, red, green, blue;
	float fading;

	void Start()
	{
		fading = startingFade / duration;
	}

	void Update()
	{
		startingFade -= fading * Time.deltaTime;
		image.color = new Color(red, green, blue, startingFade);
	}
}
