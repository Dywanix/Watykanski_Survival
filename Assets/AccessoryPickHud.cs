using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccessoryPickHud : MonoBehaviour
{
    public Map map;
    public GameObject Hud;
    public AccessoryLibrary Lib;

    public int[] rolls;

    public Image[] AccessoryImage;
    public TMPro.TextMeshProUGUI[] AccessoryTooltip;

    public void Open()
    {
        rolls[0] = Random.Range(0, Lib.AccessorySprite.Length);

        do
        {
            rolls[1] = Random.Range(0, Lib.AccessorySprite.Length);
        } while (rolls[1] == rolls[0]);

        for (int i = 0; i < 2; i++)
        {
            AccessoryImage[i].sprite = Lib.AccessorySprite[rolls[i]];
            AccessoryTooltip[i].text =  Lib.AccessoryTooltip[rolls[i]];
            if (map.playerStats.tools >= 8)
                rolls[2] = 4;
        }
    }
}
