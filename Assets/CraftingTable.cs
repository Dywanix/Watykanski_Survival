using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTable : MonoBehaviour
{
    public Workbench bench;
    public GameObject Hud;
    // accessory shit
    public Image[] AInEq, AEquipped, gunImages;
    public Image ThrashCan;
    public Button[] EQButtons, OnButtons, gunButtons;
    public Sprite[] AccessorySprite;
    public Sprite BCan, RCan;
    public int[] AEQValues, ONValues;
    public GameObject[] tooltips, Guns;

    public PlayerController playerStats;
    public TMPro.TextMeshProUGUI EqippedSlots;

    int current, tempi;
    bool active, delition;

    void Update()
    {
        if (!playerStats)
            playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent(typeof(PlayerController)) as PlayerController;

        if (Input.GetKeyDown(KeyCode.Escape) && active)
        {
            Quit();
        }
    }

    public void Open()
    {
        UpdateSprites();
        playerStats.free = false;
        playerStats.menuOpened = true;
        Hud.SetActive(true);
        UpdateInfo();
        active = true;
    }

    public void SwitchTable()
    {
        Hud.SetActive(false);
        active = false;

        for (int i = 0; i < tooltips.Length; i++)
        {
            tooltips[i].SetActive(false);
        }
        bench.Open();
    }

    public void SwitchGun(int which)
    {
        playerStats.SwapGun(which);
        UpdateInfo();
    }

    void UpdateSprites()
    {
        for (int i = 0; i < 3; i++)
        {
            if (playerStats.eq.slotFilled[i])
            {
                Guns[i].SetActive(true);
                gunImages[i].sprite = playerStats.eq.guns[i].gunSprite;
            }
            else Guns[i].SetActive(false);
        }
    }

    public void Thrash()
    {
        if (!delition)
        {
            delition = true;
            ThrashCan.sprite = RCan;
        }
        else
        {
            delition = false;
            ThrashCan.sprite = BCan;
        }
        UpdateInfo();
    }

    void UpdateInfo()
    {
        for (int i = 0; i < 3; i++)
        {
            gunButtons[i].interactable = true;
        }
        gunButtons[playerStats.eq.equipped].interactable = false;

        // Eq
        for (int i = 0; i < AInEq.Length; i++)
        {
            AInEq[i].enabled = false;
        }
        // Equipped
        for (int i = 0; i < AEquipped.Length; i++)
        {
            AEquipped[i].enabled = false;
        }

        // --Eq--
        current = 0;
        for (int i = 0; i < playerStats.eq.Accessories.Length; i++)
        {
            if (playerStats.eq.Accessories[i] > 0)
            {
                for (int j = 0; j < playerStats.eq.Accessories[i]; j++)
                {
                    AInEq[current].sprite = AccessorySprite[i];
                    AInEq[current].enabled = true;
                    AEQValues[current] = i;

                    if (delition)
                    {
                        EQButtons[current].interactable = true;
                    }
                    else
                    {
                        if (playerStats.eq.guns[playerStats.eq.equipped].MaxSlots - playerStats.eq.guns[playerStats.eq.equipped].TakenSlots > 0)
                            EQButtons[current].interactable = true;
                        else EQButtons[current].interactable = false;
                    }

                    current++;
                }
            }
        }

        // --Gun--
        current = 0;
        for (int i = 0; i < playerStats.eq.Accessories.Length; i++)
        {
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[i] > 0)
            {
                for (int j = 0; j < playerStats.eq.guns[playerStats.eq.equipped].Accessories[i]; j++)
                {
                    AEquipped[current].sprite = AccessorySprite[i];
                    AEquipped[current].enabled = true;
                    ONValues[current] = i;
                    current++;
                }
            }
        }
        EqippedSlots.text = playerStats.eq.guns[playerStats.eq.equipped].TakenSlots.ToString("0") + "/" + playerStats.eq.guns[playerStats.eq.equipped].MaxSlots.ToString("0");
        /*
        switch (selected)
        {
            case 0:
                for (int i = 0; i < playerStats.accessoriesPerType; i++)
                {
                    if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[i] > 0)
                    {
                        for (int j = 0; j < playerStats.eq.guns[playerStats.eq.equipped].Accessories[i]; j++)
                        {
                            AEquipped[current].sprite = AccessorySprite[i];
                            AEquipped[current].enabled = true;
                            FValues[current] = i;
                            current++;
                        }
                    }
                }
                EqippedSlots.text = playerStats.eq.guns[playerStats.eq.equipped].TakenSlots.ToString("0") + "/" + playerStats.eq.guns[playerStats.eq.equipped].MaxSlots.ToString("0");
                break;
            case 1:
                for (int i = playerStats.accessoriesPerType; i < playerStats.accessoriesPerType * 2; i++)
                {
                    if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[i] > 0)
                    {
                        for (int j = 0; j < playerStats.eq.guns[playerStats.eq.equipped].Accessories[i]; j++)
                        {
                            AEquipped[current].sprite = AccessorySprite[i];
                            AEquipped[current].enabled = true;
                            TValues[current] = i;
                            current++;
                        }
                    }
                }
                EqippedSlots.text = playerStats.eq.guns[playerStats.eq.equipped].TakenSlots.ToString("0") + "/" + playerStats.eq.guns[playerStats.eq.equipped].MaxSlots.ToString("0");
                break;
            case 2:
                for (int i = playerStats.accessoriesPerType * 2; i < playerStats.accessoriesPerType * 3; i++)
                {
                    if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[i] > 0)
                    {
                        for (int j = 0; j < playerStats.eq.guns[playerStats.eq.equipped].Accessories[i]; j++)
                        {
                            AEquipped[current].sprite = AccessorySprite[i];
                            AEquipped[current].enabled = true;
                            BValues[current] = i;
                            current++;
                        }
                    }
                }
                EqippedSlots.text = playerStats.eq.guns[playerStats.eq.equipped].TakenSlots.ToString("0") + "/" + playerStats.eq.guns[playerStats.eq.equipped].MaxSlots.ToString("0");
                break;
            case 3:
                for (int i = playerStats.accessoriesPerType * 3; i < playerStats.accessoriesPerType * 4; i++)
                {
                    if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[i] > 0)
                    {
                        for (int j = 0; j < playerStats.eq.guns[playerStats.eq.equipped].Accessories[i]; j++)
                        {
                            AEquipped[current].sprite = AccessorySprite[i];
                            AEquipped[current].enabled = true;
                            RValues[current] = i;

                            current++;
                        }
                    }
                }
                EqippedSlots.text = playerStats.eq.guns[playerStats.eq.equipped].TakenSlots.ToString("0") + "/" + playerStats.eq.guns[playerStats.eq.equipped].MaxSlots.ToString("0");
                break;
        }*/
    }

    public void TooltipOpen(string pack, int order)
    {
        switch (pack)
        {
            case "eq":
                tempi = AEQValues[order];
                break;
            case "equipped":
                tempi = ONValues[order];
                /*switch (selected)
                {
                    case 0:
                        tempi = FValues[order];
                        break;
                    case 1:
                        tempi = TValues[order];
                        break;
                    case 2:
                        tempi = BValues[order];
                        break;
                    case 3:
                        tempi = RValues[order];
                        break;
                }*/
                break;
        }
        tooltips[tempi].SetActive(true);
    }

    public void TooltipClose(string pack, int order)
    {
        switch (pack)
        {
            case "eq":
                tempi = AEQValues[order];
                break;
            case "equipped":
                tempi = ONValues[order];
                /*switch (selected)
                {
                    case 0:
                        tempi = FValues[order];
                        break;
                    case 1:
                        tempi = TValues[order];
                        break;
                    case 2:
                        tempi = BValues[order];
                        break;
                    case 3:
                        tempi = RValues[order];
                        break;
                }*/
                break;
        }
        tooltips[tempi].SetActive(false);
    }

    public void Equip(int which)
    {
        if (!delition)
        {
            playerStats.eq.Accessories[AEQValues[which]]--;
            playerStats.eq.guns[playerStats.eq.equipped].Accessories[AEQValues[which]]++;
            playerStats.eq.guns[playerStats.eq.equipped].TakenSlots += 1;

            /*if (AEQValues[which] < playerStats.accessoriesPerType)
            {
                playerStats.eq.guns[playerStats.eq.equipped].TakenSlots += Weights[AEQValues[which]];
            }
            else if (AEQValues[which] < playerStats.accessoriesPerType * 2)
            {
                playerStats.eq.guns[playerStats.eq.equipped].TakenSlots += Weights[AEQValues[which]];
            }
            else if (AEQValues[which] < playerStats.accessoriesPerType * 3)
            {
                playerStats.eq.guns[playerStats.eq.equipped].TakenSlots += Weights[AEQValues[which]];
            }
            else
            {
                playerStats.eq.guns[playerStats.eq.equipped].TakenSlots += Weights[AEQValues[which]];
            }*/

            GainEffect(AEQValues[which]);
            tooltips[AEQValues[which]].SetActive(false);
        }
        else
        {
            playerStats.eq.Accessories[AEQValues[which]]--;
            playerStats.GainGold(8);
            playerStats.GainTools(3);
        }

        UpdateInfo();
    }

    public void UnEquip(int which)
    {
        playerStats.eq.Accessories[ONValues[which]]++;
        playerStats.eq.guns[playerStats.eq.equipped].Accessories[ONValues[which]]--;

        playerStats.eq.guns[playerStats.eq.equipped].TakenSlots -= 1;

        LoseEffect(ONValues[which]);
        tooltips[ONValues[which]].SetActive(false);

        UpdateInfo();
        /*switch (selected)
        {
            case 0:
                playerStats.eq.Accessories[FValues[which]]++;
                playerStats.eq.guns[playerStats.eq.equipped].Accessories[FValues[which]]--;

                playerStats.eq.guns[playerStats.eq.equipped].TakenSlots -= Weights[FValues[which]];

                LoseEffect(FValues[which]);
                tooltips[FValues[which]].SetActive(false);

                UpdateInfo();
                break;
            case 1:
                playerStats.eq.Accessories[TValues[which]]++;
                playerStats.eq.guns[playerStats.eq.equipped].Accessories[TValues[which]]--;

                playerStats.eq.guns[playerStats.eq.equipped].TakenSlots -= Weights[TValues[which]];

                LoseEffect(TValues[which]);
                tooltips[TValues[which]].SetActive(false);

                UpdateInfo();
                break;
            case 2:
                playerStats.eq.Accessories[BValues[which]]++;
                playerStats.eq.guns[playerStats.eq.equipped].Accessories[BValues[which]]--;

                playerStats.eq.guns[playerStats.eq.equipped].TakenSlots -= Weights[BValues[which]];

                LoseEffect(BValues[which]);
                tooltips[BValues[which]].SetActive(false);

                UpdateInfo();
                break;
            case 3:
                playerStats.eq.Accessories[RValues[which]]++;
                playerStats.eq.guns[playerStats.eq.equipped].Accessories[RValues[which]]--;

                playerStats.eq.guns[playerStats.eq.equipped].TakenSlots -= Weights[RValues[which]];

                LoseEffect(RValues[which]);
                tooltips[RValues[which]].SetActive(false);

                UpdateInfo();
                break;
        }*/
    }

    void GainEffect(int which)
    {
        switch (which)
        {
            case 0:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier *= 1.18f;
                break;
            case 1:
                playerStats.eq.guns[playerStats.eq.equipped].fireRate /= 1.24f;
                break;
            case 2:
                playerStats.eq.guns[playerStats.eq.equipped].accuracy /= 1.48f;
                playerStats.eq.guns[playerStats.eq.equipped].range += 0.03f;
                break;
            case 3:
                playerStats.eq.guns[playerStats.eq.equipped].poisonBulletChance += 0.25f;
                break;
            case 4:
                playerStats.eq.guns[playerStats.eq.equipped].critChance += 0.12f;
                playerStats.eq.guns[playerStats.eq.equipped].critDamage += 0.18f;
                break;
            case 5:
                playerStats.eq.guns[playerStats.eq.equipped].reloadTime *= 0.64f;
                break;
            case 6:
                playerStats.eq.guns[playerStats.eq.equipped].magazineMultiplier *= 2;
                playerStats.eq.guns[playerStats.eq.equipped].reloadTime *= 1.08f;
                playerStats.DisplayAmmo();
                break;
            case 7:
                playerStats.eq.guns[playerStats.eq.equipped].spreadMultiplyer *= 2;
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier *= 0.72f;
                playerStats.eq.guns[playerStats.eq.equipped].fireRate /= 0.86f;
                break;
            case 8:
                playerStats.eq.guns[playerStats.eq.equipped].critChance += 0.15f;
                // +1 pierce & +10% pierce efficiency on crit
                break;
            case 9:
                playerStats.eq.guns[playerStats.eq.equipped].pierce += 1;
                playerStats.eq.guns[playerStats.eq.equipped].pierceEfficiency += 0.15f;
                break;
            case 10:
                // overload
                break;
            case 11:
                playerStats.eq.guns[playerStats.eq.equipped].plasmaBulletChance += 0.25f;
                break;
            case 12:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier *= 1.09f;
                playerStats.eq.guns[playerStats.eq.equipped].critDamage += 0.18f;
                break;
            case 13:
                playerStats.eq.guns[playerStats.eq.equipped].fireRate /= 1.18f;
                playerStats.eq.guns[playerStats.eq.equipped].critChance += 0.09f;
                break;
            case 14:
                // chance not to consume ammo
                break;
            case 15:
                // chance to fire 2 additional bullets in cone
                break;
            case 16:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier *= 1.06f;
                // increase damage further with base damage
                break;
            case 17:
                playerStats.eq.guns[playerStats.eq.equipped].range += 0.04f;
                playerStats.eq.guns[playerStats.eq.equipped].damageGain += 0.15f;
                break;
            case 18:
                playerStats.eq.guns[playerStats.eq.equipped].fireRate /= 1.08f;
                // + Ammo
                break;
            case 19:
                // Dasing fires wave of bullets
                break;
            case 20:
                // additional bullets fired every 6 shots
                break;
            case 21:
                playerStats.eq.guns[playerStats.eq.equipped].fireRate /= 1.06f;
                // increased fire rate with missing mag
                break;
            case 22:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier *= 1.06f;
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
                playerStats.eq.guns[playerStats.eq.equipped].fireRate /= 1.09f;
                // On Hit Rate Increase
                break;
            case 27:
                // Gun gains 5 parts after each room
                break;
            case 28:
                playerStats.eq.guns[playerStats.eq.equipped].curse += 0.28f;
                break;
            case 29:
                // Dread Orb - On Hit
                break;
        }
        /*tempi = 0;
        while (which >= playerStats.accessoriesPerType)
        {
            which -= playerStats.accessoriesPerType;
            tempi++;
        }

        switch (tempi)
        {
            // front --
            case 0:
                switch (which)
                {
                    case 0:
                        playerStats.eq.guns[playerStats.eq.equipped].damage *= 1.06f;
                        break;
                    case 1:
                        playerStats.eq.guns[playerStats.eq.equipped].DoT += 0.15f;
                        break;
                    case 2:
                        playerStats.eq.guns[playerStats.eq.equipped].spreadMultiplyer *= 2;
                        playerStats.eq.guns[playerStats.eq.equipped].damage *= 0.76f;
                        playerStats.eq.guns[playerStats.eq.equipped].fireRate *= 1.12f;
                        playerStats.eq.guns[playerStats.eq.equipped].accuracy *= 1.08f;
                        break;
                    case 3:
                        playerStats.eq.guns[playerStats.eq.equipped].penetration += 0.03f;
                        // armor shred
                        break;
                    case 4:
                        // Saw
                        break;
                }
                break;
            // top --
            case 1:
                switch (which)
                {
                    case 0:
                        playerStats.eq.guns[playerStats.eq.equipped].accuracy *= 0.82f;
                        break;
                    case 1:
                        // vulnerable applied
                        break;
                    case 2:
                        playerStats.eq.guns[playerStats.eq.equipped].pierce += 1;
                        playerStats.eq.guns[playerStats.eq.equipped].accuracy *= 0.97f;
                        break;
                    case 3:
                        // pierce efficiency
                        break;
                    case 4:
                        // Laser
                        break;
                }
                break;
            // bottom --
            case 2:
                switch (which)
                {
                    case 0:
                        tempi = playerStats.eq.guns[playerStats.eq.equipped].magazineSize / 6;
                        playerStats.eq.guns[playerStats.eq.equipped].magazineSize += tempi;
                        playerStats.DisplayAmmo();
                        break;
                    case 1:
                        playerStats.eq.guns[playerStats.eq.equipped].reloadTime *= 0.88f;
                        break;
                    case 2:
                        playerStats.eq.guns[playerStats.eq.equipped].penetration += 0.06f;
                        break;
                    case 3:
                        // chance to not consume ammo
                        break;
                    case 4:
                        // Auto-Reload
                        break;
                }
                break;
            // rear --
            case 3:
                switch (which)
                {
                    case 0:
                        for (int i = 0; i < 3; i++)
                        {
                            playerStats.eq.guns[playerStats.eq.equipped].MaxSlots++;
                        }
                        playerStats.eq.guns[playerStats.eq.equipped].fireRate *= 0.96f;
                        break;
                    case 1:
                        playerStats.eq.guns[playerStats.eq.equipped].fireRate *= 0.925f;
                        break;
                    case 2:
                        playerStats.eq.guns[playerStats.eq.equipped].critChance += 0.1f;
                        playerStats.eq.guns[playerStats.eq.equipped].critDamage += 0.09f;
                        break;
                    case 3:
                        playerStats.cooldownReduction *= 1.1f;
                        break;
                    case 4:
                        // On-shot effects rate
                        break;
                }
                break;
        }*/
    }

    void LoseEffect(int which)
    {
        switch (which)
        {
            case 0:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier /= 1.18f;
                break;
            case 1:
                playerStats.eq.guns[playerStats.eq.equipped].fireRate *= 1.24f;
                break;
            case 2:
                playerStats.eq.guns[playerStats.eq.equipped].accuracy *= 1.48f;
                playerStats.eq.guns[playerStats.eq.equipped].range -= 0.03f;
                break;
            case 3:
                playerStats.eq.guns[playerStats.eq.equipped].poisonBulletChance -= 0.25f;
                break;
                case 4:
                playerStats.eq.guns[playerStats.eq.equipped].critChance -= 0.12f;
                playerStats.eq.guns[playerStats.eq.equipped].critDamage -= 0.18f;
                break;
                case 5:
                playerStats.eq.guns[playerStats.eq.equipped].reloadTime /= 0.64f;
                break;
                case 6:
                playerStats.eq.guns[playerStats.eq.equipped].magazineMultiplier /= 2;
                playerStats.eq.guns[playerStats.eq.equipped].reloadTime /= 1.08f;
                playerStats.DisplayAmmo();
                break;
                case 7:
                playerStats.eq.guns[playerStats.eq.equipped].spreadMultiplyer /= 2;
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier /= 0.72f;
                playerStats.eq.guns[playerStats.eq.equipped].fireRate *= 0.86f;
                break;
                case 8:
                playerStats.eq.guns[playerStats.eq.equipped].critChance -= 0.15f;
                // +1 pierce & +10% pierce efficiency on crit
                break;
                case 9:
                playerStats.eq.guns[playerStats.eq.equipped].pierce -= 1;
                playerStats.eq.guns[playerStats.eq.equipped].pierceEfficiency -= 0.15f;
                break;
                case 10:
                // overload
                break;
                case 11:
                playerStats.eq.guns[playerStats.eq.equipped].plasmaBulletChance -= 0.25f;
                break;
                case 12:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier /= 1.09f;
                playerStats.eq.guns[playerStats.eq.equipped].critDamage -= 0.18f;
                break;
                case 13:
                playerStats.eq.guns[playerStats.eq.equipped].fireRate *= 1.18f;
                playerStats.eq.guns[playerStats.eq.equipped].critChance -= 0.09f;
                break;
                case 14:
                // chance not to consume ammo
                break;
                case 15:
                // chance to fire 2 additional bullets in cone
                break;
            case 16:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier /= 1.06f;
                // increase damage further with base damage
                break;
            case 17:
                playerStats.eq.guns[playerStats.eq.equipped].range -= 0.04f;
                playerStats.eq.guns[playerStats.eq.equipped].damageGain -= 0.15f;
                break;
            case 18:
                playerStats.eq.guns[playerStats.eq.equipped].fireRate *= 1.08f;
                // + Ammo
                break;
            case 19:
                // Dasing fires wave of bullets
                break;
            case 20:
                // additional bullets fired every 6 shots
                break;
            case 21:
                playerStats.eq.guns[playerStats.eq.equipped].fireRate *= 1.06f;
                // increased fire rate with missing mag
                break;
            case 22:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier /= 1.06f;
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
                playerStats.eq.guns[playerStats.eq.equipped].fireRate *= 1.09f;
                // On Hit Rate Increase
                break;
            case 27:
                // Gun gains 5 parts after each room
                break;
            case 28:
                playerStats.eq.guns[playerStats.eq.equipped].curse -= 0.28f;
                break;
            case 29:
                // Dread Orb - On Hit
                break;
        }
        /*tempi = 0;
        while (which >= playerStats.accessoriesPerType)
        {
            which -= playerStats.accessoriesPerType;
            tempi++;
        }

        switch (tempi)
        {
            // front --
            case 0:
                switch (which)
                {
                    case 0:
                        playerStats.eq.guns[playerStats.eq.equipped].damage /= 1.06f;
                        break;
                    case 1:
                        playerStats.eq.guns[playerStats.eq.equipped].DoT -= 0.15f;
                        break;
                    case 2:
                        playerStats.eq.guns[playerStats.eq.equipped].spreadMultiplyer /= 2;
                        playerStats.eq.guns[playerStats.eq.equipped].damage /= 0.76f;
                        playerStats.eq.guns[playerStats.eq.equipped].fireRate /= 1.12f;
                        playerStats.eq.guns[playerStats.eq.equipped].accuracy /= 1.08f;
                        break;
                    case 3:
                        playerStats.eq.guns[playerStats.eq.equipped].penetration -= 0.03f;
                        // armor shred
                        break;
                    case 4:
                        // Saw
                        break;
                }
                break;
            // top --
            case 1:
                switch (which)
                {
                    case 0:
                        playerStats.eq.guns[playerStats.eq.equipped].accuracy /= 0.82f;
                        break;
                    case 1:
                        // vulnerable applied
                        break;
                    case 2:
                        playerStats.eq.guns[playerStats.eq.equipped].pierce -= 1;
                        playerStats.eq.guns[playerStats.eq.equipped].accuracy /= 0.97f;
                        break;
                    case 3:
                        // pierce efficiency
                        break;
                    case 4:
                        // Laser
                        break;
                }
                break;
            // bottom --
            case 2:
                switch (which)
                {
                    case 0:
                        tempi = playerStats.eq.guns[playerStats.eq.equipped].magazineSize / 7;
                        playerStats.eq.guns[playerStats.eq.equipped].magazineSize -= tempi;
                        playerStats.DisplayAmmo();
                        break;
                    case 1:
                        playerStats.eq.guns[playerStats.eq.equipped].reloadTime /= 0.88f;
                        break;
                    case 2:
                        playerStats.eq.guns[playerStats.eq.equipped].penetration -= 0.06f;
                        break;
                    case 3:
                        // chance to not consume ammo
                        break;
                    case 4:
                        // Auto-Reload
                        break;
                }
                break;
            // rear --
            case 3:
                switch (which)
                {
                    case 0:
                        for (int i = 0; i < 3; i++)
                        {
                            playerStats.eq.guns[playerStats.eq.equipped].MaxSlots--;
                        }
                        playerStats.eq.guns[playerStats.eq.equipped].fireRate /= 0.96f;
                        break;
                    case 1:
                        playerStats.eq.guns[playerStats.eq.equipped].fireRate /= 0.925f;
                        break;
                    case 2:
                        playerStats.eq.guns[playerStats.eq.equipped].critChance -= 0.1f;
                        playerStats.eq.guns[playerStats.eq.equipped].critDamage -= 0.09f;
                        break;
                    case 3:
                        playerStats.cooldownReduction /= 1.1f;
                        break;
                    case 4:
                        // On-shot Effects rate
                        break;
                }
                break;
        }*/
    }

    void Quit()
    {
        playerStats.free = true;
        Hud.SetActive(false);
        active = false;

        for (int i = 0; i < tooltips.Length; i++)
        {
            tooltips[i].SetActive(false);
        }
    }
}
