using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour
{
    public PlayerController player;
    public Equipment eq;
    public AccessoryLibrary ALibrary;

    public GameObject Tab, RerollHud, GunHud;
    public TMPro.TextMeshProUGUI ToolsCost;
    public TMPro.TextMeshProUGUI[] StatsText;
    public GameObject[] Guns, CollectedItems, StoredAccessories, EquippedAccesssories, GunSlots;
    public Button WorkbenchButton, RerollButton, UpgradeButton;
    public Button[] GunButton, StoredButton, EquippedButton;
    public Image[] GunsImage, CollectedImages, StoredImages, EquippedImages, RerolledImages;
    public Image CurrentGunImage;

    public int[] StoredAccessory, EquippedAccessory, RerolledAccessory;
    public bool[] rerollSlots;
    int currentGun, currentAccessory, tempi;
    bool reroll, viable;

    [Header("Gun Bonus Info")]
    public GameObject InfoTab;
    public TMPro.TextMeshProUGUI[] GunStatsText;
    bool infoTabOpened;

    public void OpenBackpack()
    {
        Tab.SetActive(true);

        UpdateInfo();
    }

    void UpdateInfo()
    {
        StatsText[0].text = ((player.damageBonus * 100f) - 100f).ToString("0.0") + "%";
        StatsText[1].text = ((player.fireRateBonus * 100f) - 100f).ToString("0.0") + "%";
        StatsText[2].text = ((player.movementSpeed / 4f) - 100f).ToString("0.0") + "%";
        StatsText[3].text = ((player.cooldownReduction * 100f) - 100f).ToString("0.0") + "%";

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
        if (!reroll)
        {
            GunButton[currentGun].interactable = false;
            WorkbenchButton.interactable = true;
            CurrentGunImage.sprite = eq.guns[currentGun].gunSprite;
            GunHud.SetActive(true);
            RerollHud.SetActive(false);
        }
        else
        {
            WorkbenchButton.interactable = false;
            ToolsCost.text = UpgradeToolsCost().ToString("0");
            if (AllSlotsFilled())
                RerollButton.interactable = true;
            else RerollButton.interactable = false;
            if (player.tools >= UpgradeToolsCost() && NumberOfSlotsFilled() > 0)
            {
                if (CheckForRareAccessories())
                    UpgradeButton.interactable = true;
                else UpgradeButton.interactable = false;
            }
            else UpgradeButton.interactable = false;
            GunHud.SetActive(false);
            RerollHud.SetActive(true);
        }

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

        if (!reroll)
        {
            for (int i = 0; i < eq.guns[currentGun].MaxSlots; i++)
            {
                GunSlots[i].SetActive(true);
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
        else
        {
            for (int i = 0; i < 3; i++)
            {
                if (rerollSlots[i])
                {
                    RerolledImages[i].sprite = ALibrary.AccessorySprite[RerolledAccessory[i]];
                    RerolledImages[i].enabled = true;
                }
                else RerolledImages[i].enabled = false;
            }

            if (AllSlotsFilled())
            {
                for (int i = 0; i < currentAccessory; i++)
                {
                    StoredButton[i].interactable = false;
                }
            }
        }
    }

    bool AllSlotsFilled()
    {
        if (rerollSlots[0] && rerollSlots[1] && rerollSlots[2])
            return true;
        else return false;
    }

    int NumberOfSlotsFilled()
    {
        tempi = 0;
        for (int i = 0; i < 3; i++)
        {
            if (rerollSlots[i])
                tempi++;
        }
        return tempi;
    }

    bool CheckForRareAccessories()
    {
        viable = true;
        for (int i = 0; i < 3; i++)
        {
            if (rerollSlots[i] && RerolledAccessory[i] > ALibrary.count)
                viable = false;
        }
        return viable;
    }

    int UpgradeToolsCost()
    {
        return NumberOfSlotsFilled() * 30;
    }

    public void ChooseGun(int which)
    {
        currentGun = which;
        reroll = false;

        UpdateInfo();
    }

    public void ChooseReroll()
    {
        reroll = true;

        UpdateInfo();
    }

    public void Reroll()
    {
        for (int i = 0; i < 3; i++)
        {
            rerollSlots[i] = false;
        }

        player.map.ChoosePrize(player.map.PrizeRarity());

        UpdateInfo();
        player.CloseTab();
    }

    public void Upgrade()
    {
        player.SpendTools(UpgradeToolsCost());

        for (int i = 0; i < 3; i++)
        {
            if (rerollSlots[i])
                eq.Accessories[RerolledAccessory[i] + ALibrary.count]++;
            rerollSlots[i] = false;
        }

        UpdateInfo();
    }

    public void EquipAccessory(int placement)
    {
        eq.Accessories[StoredAccessory[placement]]--;

        if (!reroll)
        {
            eq.guns[currentGun].Accessories[StoredAccessory[placement]]++;
            eq.guns[currentGun].TakenSlots += 1;

            if (StoredAccessory[placement] >= ALibrary.count)
                GainUncommonEffect(StoredAccessory[placement] - ALibrary.count);
            else GainEffect(StoredAccessory[placement]);
        }
        else
        {
            if (!rerollSlots[0])
            {
                RerolledAccessory[0] = StoredAccessory[placement];
                rerollSlots[0] = true;
            }
            else if (!rerollSlots[1])
            {
                RerolledAccessory[1] = StoredAccessory[placement];
                rerollSlots[1] = true;
            }
            else
            {
                RerolledAccessory[2] = StoredAccessory[placement];
                rerollSlots[2] = true;
            }
        }
        TooltipClose();

        UpdateInfo();
    }

    public void RemoveAccessory(int placement)
    {
        if (!reroll)
        {
            eq.Accessories[EquippedAccessory[placement]]++;
            eq.guns[currentGun].Accessories[EquippedAccessory[placement]]--;
            eq.guns[currentGun].TakenSlots -= 1;

            if (EquippedAccessory[placement] >= ALibrary.count)
                LoseUncommonEffect(EquippedAccessory[placement] - ALibrary.count);
            else LoseEffect(EquippedAccessory[placement]);
        }
        else
        {
            eq.Accessories[RerolledAccessory[placement]]++;
            rerollSlots[placement] = false;
        }
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
                eq.guns[currentGun].accuracy *= 0.52f;
                eq.guns[currentGun].range += 0.03f;
                break;
            case 3:
                eq.guns[currentGun].specialBulletChance[0] += 0.25f;
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
                eq.guns[currentGun].specialBulletChance[1] += 0.25f;
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
                eq.guns[currentGun].specialBulletChance[2] += 0.25f;
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
                // Laser - On Hit
                break;
            case 28:
                eq.guns[currentGun].DoT += 0.30f;
                break;
            case 29:
                eq.guns[currentGun].damageMultiplier *= 1.06f;
                eq.guns[currentGun].shatter += 0.30f;
                break;
            case 30:
                eq.guns[currentGun].DoT += 0.12f;
                eq.guns[currentGun].specialBulletChance[0] += 0.15f;
                break;
            case 31:
                eq.guns[currentGun].fireRate /= 1.096f;
                eq.guns[currentGun].specialBulletChance[1] += 0.15f;
                break;
            case 32:
                eq.guns[currentGun].damageMultiplier *= 1.072f;
                eq.guns[currentGun].specialBulletChance[2] += 0.15f;
                break;
            case 33:
                for (int i = 0; i < 4; i++)
                {
                    eq.guns[currentGun].specialBulletChance[i] += 0.09f;
                }
                break;
            case 34:
                eq.guns[currentGun].critChance += 0.06f;
                // crits increase Special Bullet Chance
                break;
            case 35:
                // Every Non-Effect gives chance for next shot
                break;
            case 36:
                // Every Effect gives chance for other Effects
                break;
            case 37:
                eq.guns[currentGun].specialBulletChance[3] += 0.25f;
                break;
            case 38:
                eq.guns[currentGun].shatter += 0.18f;
                eq.guns[currentGun].specialBulletChance[3] += 0.15f;
                break;
            case 39:
                eq.guns[currentGun].specialBulletChance[0] += 0.125f;
                eq.guns[currentGun].specialBulletChance[1] += 0.125f;
                break;
            case 40:
                eq.guns[currentGun].specialBulletChance[2] += 0.125f;
                eq.guns[currentGun].specialBulletChance[3] += 0.125f;
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
                eq.guns[currentGun].accuracy /= 0.52f;
                eq.guns[currentGun].range -= 0.03f;
                break;
            case 3:
                eq.guns[currentGun].specialBulletChance[0] -= 0.25f;
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
                eq.guns[currentGun].specialBulletChance[1] -= 0.25f;
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
                eq.guns[currentGun].specialBulletChance[2] -= 0.25f;
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
                // Laser - On Hit
                break;
            case 28:
                eq.guns[currentGun].DoT -= 0.30f;
                break;
            case 29:
                eq.guns[currentGun].damageMultiplier /= 1.06f;
                eq.guns[currentGun].shatter -= 0.30f;
                break;
            case 30:
                eq.guns[currentGun].DoT -= 0.12f;
                eq.guns[currentGun].specialBulletChance[0] -= 0.15f;
                break;
            case 31:
                eq.guns[currentGun].fireRate *= 1.096f;
                eq.guns[currentGun].specialBulletChance[1] -= 0.15f;
                break;
            case 32:
                eq.guns[currentGun].damageMultiplier /= 1.072f;
                eq.guns[currentGun].specialBulletChance[2] -= 0.15f;
                break;
            case 33:
                for (int i = 0; i < 4; i++)
                {
                    eq.guns[currentGun].specialBulletChance[i] -= 0.09f;
                }
                break;
            case 34:
                eq.guns[currentGun].critChance -= 0.06f;
                // crits increase Special Bullet Chance
                break;
            case 35:
                // Every Non-Effect gives chance for next shot
                break;
            case 36:
                // Every Effect gives chance for other Effects
                break;
            case 37:
                eq.guns[currentGun].specialBulletChance[3] -= 0.25f;
                break;
            case 38:
                eq.guns[currentGun].shatter -= 0.18f;
                eq.guns[currentGun].specialBulletChance[3] -= 0.15f;
                break;
            case 39:
                eq.guns[currentGun].specialBulletChance[0] -= 0.125f;
                eq.guns[currentGun].specialBulletChance[1] -= 0.125f;
                break;
            case 40:
                eq.guns[currentGun].specialBulletChance[2] -= 0.125f;
                eq.guns[currentGun].specialBulletChance[3] -= 0.125f;
                break;
        }
    }

    void GainUncommonEffect(int whatEffect)
    {
        switch (whatEffect)
        {
            case 0:
                eq.guns[currentGun].damageMultiplier *= 1.288f;
                break;
            case 1:
                eq.guns[currentGun].fireRate /= 1.384f;
                break;
            case 2:
                eq.guns[currentGun].accuracy *= 0.325f;
                eq.guns[currentGun].range += 0.05f;
                break;
            case 3:
                eq.guns[currentGun].specialBulletChance[0] += 0.4f;
                break;
            case 4:
                eq.guns[currentGun].critChance += 0.192f;
                eq.guns[currentGun].critDamage += 0.288f;
                break;
            case 5:
                eq.guns[currentGun].reloadTime *= 0.4f;
                break;
            case 6:
                eq.guns[currentGun].magazineMultiplier *= 3;
                eq.guns[currentGun].reloadTime *= 1.125f;
                player.DisplayAmmo();
                break;
            case 7:
                eq.guns[currentGun].spreadMultiplyer *= 3;
                eq.guns[currentGun].damageMultiplier *= 0.58f;
                eq.guns[currentGun].fireRate /= 0.79f;
                break;
            case 8:
                eq.guns[currentGun].critChance += 0.27f;
                // +1 pierce & +10% pierce efficiency on crit
                break;
            case 9:
                eq.guns[currentGun].pierce += 2;
                eq.guns[currentGun].pierceEfficiency += 0.18f;
                break;
            case 10:
                // overload
                break;
            case 11:
                eq.guns[currentGun].specialBulletChance[1] += 0.4f;
                break;
            case 12:
                eq.guns[currentGun].damageMultiplier *= 1.144f;
                eq.guns[currentGun].critDamage += 0.288f;
                break;
            case 13:
                eq.guns[currentGun].fireRate /= 1.288f;
                eq.guns[currentGun].critChance += 0.144f;
                break;
            case 14:
                // chance not to consume ammo
                break;
            case 15:
                // chance to fire 2 additional bullets in cone
                break;
            case 16:
                eq.guns[currentGun].damageMultiplier *= 1.096f;
                // increase damage further with base damage
                break;
            case 17:
                eq.guns[currentGun].specialBulletChance[2] += 0.4f;
                break;
            case 18:
                eq.guns[currentGun].fireRate /= 1.128f;
                // + Ammo
                break;
            case 19:
                // Dasing fires wave of bullets
                break;
            case 20:
                // additional bullets fired every 3,125 shots - On Hit
                break;
            case 21:
                eq.guns[currentGun].fireRate /= 1.096f;
                // increased fire rate with missing mag
                break;
            case 22:
                eq.guns[currentGun].damageMultiplier *= 1.096f;
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
                eq.guns[currentGun].fireRate /= 1.144f;
                // On Hit Rate Increase
                break;
            case 27:
                // Laser - On Hit
                break;
            case 28:
                eq.guns[currentGun].DoT += 0.48f;
                break;
            case 29:
                eq.guns[currentGun].damageMultiplier *= 1.096f;
                eq.guns[currentGun].shatter += 0.48f;
                break;
            case 30:
                eq.guns[currentGun].DoT += 0.192f;
                eq.guns[currentGun].specialBulletChance[0] += 0.24f;
                break;
            case 31:
                eq.guns[currentGun].fireRate /= 1.154f;
                eq.guns[currentGun].specialBulletChance[1] += 0.24f;
                break;
            case 32:
                eq.guns[currentGun].damageMultiplier *= 1.115f;
                eq.guns[currentGun].specialBulletChance[2] += 0.24f;
                break;
            case 33:
                for (int i = 0; i < 4; i++)
                {
                    eq.guns[currentGun].specialBulletChance[i] += 0.14f;
                }
                break;
            case 34:
                eq.guns[currentGun].critChance += 0.096f;
                // crits increase Special Bullet Chance
                break;
            case 35:
                // Every Non-Effect gives chance for next shot
                break;
            case 36:
                // Every Effect gives chance for other Effects
                break;
            case 37:
                eq.guns[currentGun].specialBulletChance[3] += 0.40f;
                break;
            case 38:
                eq.guns[currentGun].shatter += 0.288f;
                eq.guns[currentGun].specialBulletChance[3] += 0.24f;
                break;
            case 39:
                eq.guns[currentGun].specialBulletChance[0] += 0.2f;
                eq.guns[currentGun].specialBulletChance[1] += 0.2f;
                break;
            case 40:
                eq.guns[currentGun].specialBulletChance[2] += 0.2f;
                eq.guns[currentGun].specialBulletChance[3] += 0.2f;
                break;
        }
    }

    void LoseUncommonEffect(int whatEffect)
    {
        switch (whatEffect)
        {
            case 0:
                eq.guns[currentGun].damageMultiplier /= 1.288f;
                break;
            case 1:
                eq.guns[currentGun].fireRate *= 1.384f;
                break;
            case 2:
                eq.guns[currentGun].accuracy /= 0.325f;
                eq.guns[currentGun].range -= 0.05f;
                break;
            case 3:
                eq.guns[currentGun].specialBulletChance[0] -= 0.4f;
                break;
            case 4:
                eq.guns[currentGun].critChance -= 0.192f;
                eq.guns[currentGun].critDamage -= 0.288f;
                break;
            case 5:
                eq.guns[currentGun].reloadTime /= 0.4f;
                break;
            case 6:
                eq.guns[currentGun].magazineMultiplier /= 3;
                eq.guns[currentGun].reloadTime /= 1.125f;
                player.DisplayAmmo();
                break;
            case 7:
                eq.guns[currentGun].spreadMultiplyer /= 3;
                eq.guns[currentGun].damageMultiplier /= 0.58f;
                eq.guns[currentGun].fireRate *= 0.79f;
                break;
            case 8:
                eq.guns[currentGun].critChance -= 0.27f;
                // +1 pierce & +10% pierce efficiency on crit
                break;
            case 9:
                eq.guns[currentGun].pierce -= 2;
                eq.guns[currentGun].pierceEfficiency -= 0.18f;
                break;
            case 10:
                // overload
                break;
            case 11:
                eq.guns[currentGun].specialBulletChance[1] -= 0.4f;
                break;
            case 12:
                eq.guns[currentGun].damageMultiplier /= 1.144f;
                eq.guns[currentGun].critDamage /= 0.288f;
                break;
            case 13:
                eq.guns[currentGun].fireRate *= 1.288f;
                eq.guns[currentGun].critChance -= 0.144f;
                break;
            case 14:
                // chance not to consume ammo
                break;
            case 15:
                // chance to fire 2 additional bullets in cone
                break;
            case 16:
                eq.guns[currentGun].damageMultiplier /= 1.096f;
                // increase damage further with base damage
                break;
            case 17:
                eq.guns[currentGun].specialBulletChance[2] -= 0.4f;
                break;
            case 18:
                eq.guns[currentGun].fireRate *= 1.128f;
                // + Ammo
                break;
            case 19:
                // Dasing fires wave of bullets
                break;
            case 20:
                // additional bullets fired every 3,125 shots - On Hit
                break;
            case 21:
                eq.guns[currentGun].fireRate *= 1.096f;
                // increased fire rate with missing mag
                break;
            case 22:
                eq.guns[currentGun].damageMultiplier /= 1.096f;
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
                eq.guns[currentGun].fireRate *= 1.144f;
                // On Hit Rate Increase
                break;
            case 27:
                // Laser - On Hit
                break;
            case 28:
                eq.guns[currentGun].DoT -= 0.48f;
                break;
            case 29:
                eq.guns[currentGun].damageMultiplier /= 1.096f;
                eq.guns[currentGun].shatter -= 0.48f;
                break;
            case 30:
                eq.guns[currentGun].DoT -= 0.192f;
                eq.guns[currentGun].specialBulletChance[0] -= 0.24f;
                break;
            case 31:
                eq.guns[currentGun].fireRate *= 1.154f;
                eq.guns[currentGun].specialBulletChance[1] -= 0.24f;
                break;
            case 32:
                eq.guns[currentGun].damageMultiplier /= 1.115f;
                eq.guns[currentGun].specialBulletChance[2] -= 0.24f;
                break;
            case 33:
                for (int i = 0; i < 4; i++)
                {
                    eq.guns[currentGun].specialBulletChance[i] -= 0.14f;
                }
                break;
            case 34:
                eq.guns[currentGun].critChance -= 0.096f;
                // crits increase Special Bullet Chance
                break;
            case 35:
                // Every Non-Effect gives chance for next shot
                break;
            case 36:
                // Every Effect gives chance for other Effects
                break;
            case 37:
                eq.guns[currentGun].specialBulletChance[3] -= 0.40f;
                break;
            case 38:
                eq.guns[currentGun].shatter -= 0.288f;
                eq.guns[currentGun].specialBulletChance[3] -= 0.24f;
                break;
            case 39:
                eq.guns[currentGun].specialBulletChance[0] -= 0.2f;
                eq.guns[currentGun].specialBulletChance[1] -= 0.2f;
                break;
            case 40:
                eq.guns[currentGun].specialBulletChance[2] -= 0.2f;
                eq.guns[currentGun].specialBulletChance[3] -= 0.2f;
                break;
        }
    }

    public void TooltipOpen(int placement, bool equipped, bool rerolled)
    {
        if (equipped)
            eq.Tooltip.text = ALibrary.AccessoryTooltip[EquippedAccessory[placement]];
        else if (rerolled)
            eq.Tooltip.text = ALibrary.AccessoryTooltip[RerolledAccessory[placement]];
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

    public void GunInfoClicked()
    {
        if (!infoTabOpened)
        {
            infoTabOpened = true;
            InfoTab.SetActive(infoTabOpened);
            if (eq.guns[currentGun].BulletsFired() > 1) GunStatsText[0].text = eq.guns[currentGun].Damage().ToString("0.0") + "x" + eq.guns[currentGun].BulletsFired().ToString("0");
            else GunStatsText[0].text = eq.guns[currentGun].Damage().ToString("0.0");
            GunStatsText[1].text = (1f / eq.guns[currentGun].fireRate).ToString("0.00") + "/s";
            GunStatsText[2].text = (eq.guns[currentGun].critChance * 100).ToString("0") + "%";
            GunStatsText[3].text = ((100f - eq.guns[currentGun].accuracy)).ToString("0.0") + "%";
            GunStatsText[4].text = (eq.guns[currentGun].range * 100f).ToString("0,0");
            GunStatsText[5].text = eq.guns[currentGun].reloadTime.ToString("0.00") + "s";
            GunStatsText[6].text = eq.guns[currentGun].MagazineTotalSize().ToString("0");
            GunStatsText[7].text = eq.guns[currentGun].maxAmmo.ToString("0");
            GunStatsText[8].text = eq.guns[currentGun].info;
        }
        else
        {
            infoTabOpened = false;
            InfoTab.SetActive(infoTabOpened);
        }
    }
}
