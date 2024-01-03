using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunPickHud : MonoBehaviour
{
    public GameObject Hud;
    public GunsLibrary Lib;

    public Gun[] gun;
    public int[] rolls;
    int upgrades, roll;
    bool viable;

    public Image[] GunImage;
    public TMPro.TextMeshProUGUI[] DamageValue, FireRateValue, CritValue, AccuracyValue, ReloadValue, MagazineValue, AmmoValue;

    public void Open(int level)
    {
        upgrades = 4 + level;

        rolls[0] = Random.Range(0, Lib.guns.Length);
        gun[0] = Lib.guns[rolls[0]];

        viable = false;
        while (!viable)
        {
            rolls[1] = Random.Range(0, Lib.guns.Length);
            if (rolls[1] != rolls[0])
                viable = true;
        }
        gun[1] = Lib.guns[rolls[1]];

        viable = false;
        while (!viable)
        {
            rolls[2] = Random.Range(0, Lib.guns.Length);
            if (rolls[2] != rolls[0] && rolls[2] != rolls[1])
                viable = true;
        }
        gun[2] = Lib.guns[rolls[2]];

        SetGuns();
        DisplayGuns();
        Hud.SetActive(true);
    }

    void SetGuns()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < upgrades; j++)
            {
                roll = Random.Range(0, 6);
                switch (roll)
                {
                    case 0:
                        gun[i].damage *= 1.01f;
                        break;
                    case 1:
                        gun[i].fireRate *= 0.988f;
                        break;
                    case 2:
                        gun[i].critChance += 0.01f;
                        break;
                    case 3:
                        gun[i].accuracy *= 0.982f;
                        break;
                    case 4:
                        gun[i].reloadTime *= 0.976f;
                        break;
                    case 5:
                        gun[i].maxAmmo += gun[i].maxAmmo / 40;
                        break;
                }
            }
        }
    }

    void DisplayGuns()
    {
        for (int i = 0; i < 3; i++)
        {
            GunImage[i].sprite = gun[i].gunSprite;
            if (gun[i].bulletSpread > 1) DamageValue[i].text = gun[i].damage.ToString("0.0") + "x" + gun[i].bulletSpread.ToString("0");
            else DamageValue[i].text = gun[i].damage.ToString("0.0");
            FireRateValue[i].text = (1f / gun[i].fireRate).ToString("0.00") + "/s";
            CritValue[i].text = (gun[i].critChance * 100).ToString("0") + "%";
            AccuracyValue[i].text = ((100f - gun[i].accuracy)).ToString("0.0") + "%";
            ReloadValue[i].text = gun[i].reloadTime.ToString("0.00") + "s";
            MagazineValue[i].text = gun[i].magazineSize.ToString("0");
            AmmoValue[i].text = gun[i].maxAmmo.ToString("0");
        }
    }


}
