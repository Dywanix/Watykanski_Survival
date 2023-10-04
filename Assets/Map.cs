using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public GameObject Player, RoundBar;
    public GameObject[] Players;
    public PlayerController playerStats;
    public Image RoundBarFill;
    public TMPro.TextMeshProUGUI RoundsCount;

    void Start()
    {
        Instantiate(Players[PlayerPrefs.GetInt("Class")]);
        Player = GameObject.FindGameObjectWithTag("Player");
        playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        playerStats.SwapGun(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
