using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour
{
    public PlayerController player;
    public Equipment eq;
    public AccessoryLibrary ALibrary;

    public GameObject Tab;
    public TMPro.TextMeshProUGUI[] StatsText;
    public GameObject[] Guns, CollectedItems, StoredAccessories, EquippedAccesssories, GunSlots;
    public Button[] GunButton, StoredButton, EquippedButton;
    public Image[] GunsImage, CollectedImages, StoredImages, EquippedImages;
    public Image CurrentGunImage;

    public int[] StoredAccessory, EquippedAccessory;
    int currentGun, currentAccessory;

    public void OpenBackpack()
    {
        Tab.SetActive(true);

        UpdateInfo();
    }

    void UpdateInfo()
    {
        StatsText[0].text = ((player.cooldownReduction * 100f) - 100f).ToString("0.0") + "%";
        StatsText[1].text = ((player.damageBonus * 100f) - 100f).ToString("0.0") + "%";
        StatsText[2].text = ((player.fireRateBonus * 100f) - 100f).ToString("0.0") + "%";
        StatsText[3].text = ((player.movementSpeed / 4f) - 100f).ToString("0.0") + "%";

        for (int i = 0; i < 2; i++)
        {
            if (eq.slotFilled[i + 1])
                Guns[i].SetActive(true);
            else Guns[i].SetActive(false);
        }

        for (int i = 0; i < 3; i++)
        {
            GunButton[i].interactable = true;
            if (eq.slotFilled[i])
                GunsImage[i].sprite = eq.guns[i].gunSprite;
        }
        GunButton[currentGun].interactable = false;
        CurrentGunImage.sprite = eq.guns[currentGun].gunSprite;

        for (int i = 0; i < eq.itemsCollected; i++)
        {
            CollectedItems[i].SetActive(true);
            CollectedImages[i].sprite = eq.ILibrary.ItemSprite[eq.ItemList[i]];
        }

        UpdateAccessories();
    }

    void UpdateAccessories()
    {
        for (int i = 0; i < StoredImages.Length; i++)
        {
            StoredImages[i].enabled = false;
        }

        for (int i = 0; i < EquippedImages.Length; i++)
        {
            EquippedImages[i].enabled = false;
            GunSlots[i].SetActive(false);
        }

        for (int i = 0; i < eq.guns[currentGun].MaxSlots; i++)
        {
            GunSlots[i].SetActive(true);
        }

        currentAccessory = 0;
        for (int i = 0; i < eq.Accessories.Length; i++)
        {
            if (eq.Accessories[i] > 0)
            {
                for (int j = 0; j < eq.Accessories[i]; j++)
                {
                    StoredImages[currentAccessory].sprite = ALibrary.AccessorySprite[i];
                    StoredImages[currentAccessory].enabled = true;
                    StoredAccessory[currentAccessory] = i;

                    /*if (delition)
                    {
                        EQButtons[current].interactable = true;
                    }
                    else
                    {
                        if (playerStats.eq.guns[playerStats.eq.equipped].MaxSlots - playerStats.eq.guns[playerStats.eq.equipped].TakenSlots > 0)
                            EQButtons[current].interactable = true;
                        else EQButtons[current].interactable = false;
                    }*/

                    StoredButton[currentAccessory].interactable = true;

                    currentAccessory++;
                }
            }
        }

        if (eq.guns[currentGun].TakenSlots >= eq.guns[currentGun].MaxSlots)
        {
            for (int i = 0; i < currentAccessory; i++)
            {
                StoredButton[i].interactable = false;
            }
        }

        currentAccessory = 0;
        for (int i = 0; i < eq.Accessories.Length; i++)
        {
            if (eq.guns[currentGun].Accessories[i] > 0)
            {
                for (int j = 0; j < eq.guns[currentGun].Accessories[i]; j++)
                {
                    EquippedImages[currentAccessory].sprite = ALibrary.AccessorySprite[i];
                    EquippedImages[currentAccessory].enabled = true;
                    EquippedAccessory[currentAccessory] = i;
                    currentAccessory++;
                }
            }
        }
    }

    public void ChooseGun(int which)
    {
        currentGun = which;

        UpdateInfo();
    }

    public void EquipAccessory(int placement)
    {
        eq.Accessories[StoredAccessory[placement]]--;
        eq.guns[currentGun].Accessories[StoredAccessory[placement]]++;
        eq.guns[currentGun].TakenSlots += 1;

        GainEffect(StoredAccessory[placement]);
        TooltipClose();

        UpdateInfo();
    }

    public void RemoveAccessory(int placement)
    {
        eq.Accessories[EquippedAccessory[placement]]++;
        eq.guns[currentGun].Accessories[EquippedAccessory[placement]]--;
        eq.guns[currentGun].TakenSlots -= 1;

        LoseEffect(EquippedAccessory[placement]);
        TooltipClose();

        UpdateInfo();
    }

    void GainEffect(int whatEffect)
    {
        switch (whatEffect)
        {
            case 0:
                eq.guns[currentGun].damageMultiplier *= 1.18f;
                break;
            case 1:
                eq.guns[currentGun].fireRate /= 1.24f;
                break;
            case 2:
                eq.guns[currentGun].accuracy /= 1.48f;
                eq.guns[currentGun].range += 0.03f;
                break;
            case 3:
                eq.guns[currentGun].poisonBulletChance += 0.25f;
                break;
            case 4:
                eq.guns[currentGun].critChance += 0.12f;
                eq.guns[currentGun].critDamage += 0.18f;
                break;
            case 5:
                eq.guns[currentGun].reloadTime *= 0.64f;
                break;
            case 6:
                eq.guns[currentGun].magazineMultiplier *= 2;
                eq.guns[currentGun].reloadTime *= 1.08f;
                player.DisplayAmmo();
                break;
            case 7:
                eq.guns[currentGun].spreadMultiplyer *= 2;
                eq.guns[currentGun].damageMultiplier *= 0.72f;
                eq.guns[currentGun].fireRate /= 0.86f;
                break;
            case 8:
                eq.guns[currentGun].critChance += 0.15f;
                // +1 pierce & +10% pierce efficiency on crit
                break;
            case 9:
                eq.guns[currentGun].pierce += 1;
                eq.guns[currentGun].pierceEfficiency += 0.15f;
                break;
            case 10:
                // overload
                break;
            case 11:
                eq.guns[currentGun].plasmaBulletChance += 0.25f;
                break;
            case 12:
                eq.guns[currentGun].damageMultiplier *= 1.09f;
                eq.guns[currentGun].critDamage += 0.18f;
                break;
            case 13:
                eq.guns[currentGun].fireRate /= 1.18f;
                eq.guns[currentGun].critChance += 0.09f;
                break;
            case 14:
                // chance not to consume ammo
                break;
            case 15:
                // chance to fire 2 additional bullets in cone
                break;
            case 16:
                eq.guns[currentGun].damageMultiplier *= 1.06f;
                // increase damage further with base damage
                break;
            case 17:
                eq.guns[currentGun].range += 0.04f;
                eq.guns[currentGun].damageGain += 0.15f;
                break;
            case 18:
                eq.guns[currentGun].fireRate /= 1.08f;
                // + Ammo
                break;
            case 19:
                // Dasing fires wave of bullets
                break;
            case 20:
                // additional bullets fired every 6 shots
                break;
            case 21:
                eq.guns[currentGun].fireRate /= 1.06f;
                // increased fire rate with missing mag
                break;
            case 22:
                eq.guns[currentGun].damageMultiplier *= 1.06f;
                // increase damage based on magazine size
                break;
            case 23:
                // Peacemaker - On Hit
                break;
            case 24:
                // Boomerang - On Hit
                break;
            case 25:
                // Wave - On Hit
                break;
            case 26:
                eq.guns[currentGun].fireRate /= 1.09f;
                // On Hit Rate Increase
                break;
            case 27:
                // Gun gains 5 parts after each room
                break;
            case 28:
                eq.guns[currentGun].curse += 0.28f;
                break;
            case 29:
                // Dread Orb - On Hit
                break;
        }
    }

    void LoseEffect(int whatEffect)
    {
        switch (whatEffect)
        {
            case 0:
                eq.guns[currentGun].damageMultiplier /= 1.18f;
                break;
            case 1:
                eq.guns[currentGun].fireRate *= 1.24f;
                break;
            case 2:
                eq.guns[currentGun].accuracy *= 1.48f;
                eq.guns[currentGun].range -= 0.03f;
                break;
            case 3:
                eq.guns[currentGun].poisonBulletChance -= 0.25f;
                break;
            case 4:
                eq.guns[currentGun].critChance -= 0.12f;
                eq.guns[currentGun].critDamage -= 0.18f;
                break;
            case 5:
                eq.guns[currentGun].reloadTime /= 0.64f;
                break;
            case 6:
                eq.guns[currentGun].magazineMultiplier /= 2;
                eq.guns[currentGun].reloadTime /= 1.08f;
                player.DisplayAmmo();
                break;
            case 7:
                eq.guns[currentGun].spreadMultiplyer /= 2;
                eq.guns[currentGun].damageMultiplier /= 0.72f;
                eq.guns[currentGun].fireRate *= 0.86f;
                break;
            case 8:
                eq.guns[currentGun].critChance -= 0.15f;
                // +1 pierce & +10% pierce efficiency on crit
                break;
            case 9:
                eq.guns[currentGun].pierce -= 1;
                eq.guns[currentGun].pierceEfficiency -= 0.15f;
                break;
            case 10:
                // overload
                break;
            case 11:
                eq.guns[currentGun].plasmaBulletChance -= 0.25f;
                break;
            case 12:
                eq.guns[currentGun].damageMultiplier /= 1.09f;
                eq.guns[currentGun].critDamage -= 0.18f;
                break;
            case 13:
                eq.guns[currentGun].fireRate *= 1.18f;
                eq.guns[currentGun].critChance -= 0.09f;
                break;
            case 14:
                // chance not to consume ammo
                break;
            case 15:
                // chance to fire 2 additional bullets in cone
                break;
            case 16:
                eq.guns[currentGun].damageMultiplier /= 1.06f;
                // increase damage further with base damage
                break;
            case 17:
                eq.guns[currentGun].range -= 0.04f;
                eq.guns[currentGun].damageGain -= 0.15f;
                break;
            case 18:
                eq.guns[currentGun].fireRate *= 1.08f;
                // + Ammo
                break;
            case 19:
                // Dasing fires wave of bullets
                break;
            case 20:
                // additional bullets fired every 6 shots
                break;
            case 21:
                eq.guns[currentGun].fireRate *= 1.06f;
                // increased fire rate with missing mag
                break;
            case 22:
                eq.guns[currentGun].damageMultiplier /= 1.06f;
                // increase damage based on magazine size
                break;
            case 23:
                // Peacemaker - On Hit
                break;
            case 24:
                // Boomerang - On Hit
                break;
            case 25:
                // Wave - On Hit
                break;
            case 26:
                eq.guns[currentGun].fireRate *= 1.09f;
                // On Hit Rate Increase
                break;
            case 27:
                // Gun gains 5 parts after each room
                break;
            case 28:
                eq.guns[currentGun].curse -= 0.28f;
                break;
            case 29:
                // Dread Orb - On Hit
                break;
        }
    }

    public void TooltipOpen(int placement, bool equipped)
    {
        if (equipped)
            eq.Tooltip.text = ALibrary.AccessoryTooltip[EquippedAccessory[placement]];
        else eq.Tooltip.text = ALibrary.AccessoryTooltip[StoredAccessory[placement]];
    }

    public void TooltipClose()
    {
        eq.Tooltip.text = "";
    }

    public void CloseBackpack()
    {
        Tab.SetActive(false);
    }
}
