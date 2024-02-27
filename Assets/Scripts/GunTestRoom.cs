using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTestRoom : MonoBehaviour
{
    public GameObject Player, Room;
    public PlayerController playerStats;
	public GunsLibrary GLib;
	public GameObject[] GunPick;
	public SpriteRenderer[] GunSprite;

	void Start()
	{
		for	(int i = 0; i < GLib.guns.Length; i++)
		{
			GunPick[i].SetActive(true);
			GunSprite[i].sprite = GLib.guns[i].gunSprite;
		}
	}

	void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        }
    }

	public void Choose(int which)
	{
		playerStats.PickUpGun(which);
        Room.SetActive(false);
	}
}
