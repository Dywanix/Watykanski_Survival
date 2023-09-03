using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTable : MonoBehaviour
{
    public GameObject Player, Glow, Hud;
    // accessory shit
    public Image[] AInEq, AEquipped;
    public Image ThrashCan;
    public Button[] EQButtons, OnButtons;
    public Sprite[] AccessorySprite;
    public Sprite BCan, RCan;
    public int[] AEQValues, ONValues;
    public GameObject[] tooltips;

    public PlayerController playerStats;
    public TMPro.TextMeshProUGUI EqippedSlots;

    int current, tempi;
    bool active, delition;

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        }
        if (Vector3.Distance(transform.position, Player.transform.position) <= 4.2f)
        {
            if (playerStats.day)
            {
                Glow.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E) && !active)
                {
                    playerStats.free = false;
                    Hud.SetActive(true);
                    UpdateInfo();
                    active = true;
                }
            }
            else Glow.SetActive(false);

            if (Input.GetKeyDown(KeyCode.Escape) && active)
            {
                Quit();
            }
        }
        else Glow.SetActive(false);
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
            playerStats.GainScrap(12);
            playerStats.GainTools(4);
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
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier *= 1.15f;
                break;
            case 1:
                playerStats.eq.guns[playerStats.eq.equipped].fireRate /= 1.2f;
                break;
            case 2:
                playerStats.eq.guns[playerStats.eq.equipped].accuracy /= 1.4f;
                playerStats.eq.guns[playerStats.eq.equipped].range += 0.03f;
                break;
            case 3:
                playerStats.eq.guns[playerStats.eq.equipped].penetration += 0.14f;
                break;
            case 4:
                playerStats.eq.guns[playerStats.eq.equipped].critChance += 0.1f;
                playerStats.eq.guns[playerStats.eq.equipped].critDamage += 0.15f;
                break;
            case 5:
                playerStats.eq.guns[playerStats.eq.equipped].reloadTime *= 0.7f;
                break;
            case 6:
                playerStats.eq.guns[playerStats.eq.equipped].magazineMultiplier *= 2;
                playerStats.eq.guns[playerStats.eq.equipped].reloadTime *= 1.12f;
                playerStats.DisplayAmmo();
                break;
            case 7:
                playerStats.eq.guns[playerStats.eq.equipped].spreadMultiplyer *= 2;
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier *= 0.7f;
                playerStats.eq.guns[playerStats.eq.equipped].fireRate /= 0.85f;
                break;
            case 8:
                playerStats.eq.guns[playerStats.eq.equipped].pierce += 1;
                playerStats.eq.guns[playerStats.eq.equipped].critChance += 0.03f;
                // +1 pierce on crit
                break;
            case 9:
                playerStats.eq.guns[playerStats.eq.equipped].pierce += 1;
                // pierce efficiency
                break;
            case 10:
                // overload
                break;
            case 11:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier *= 1.05f;
                playerStats.eq.guns[playerStats.eq.equipped].fireRate /= 1.06f;
                playerStats.eq.guns[playerStats.eq.equipped].accuracy /= 1.12f;
                playerStats.eq.guns[playerStats.eq.equipped].penetration += 0.04f;
                break;
            case 12:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier *= 1.075f;
                playerStats.eq.guns[playerStats.eq.equipped].critDamage += 0.15f;
                break;
            case 13:
                playerStats.eq.guns[playerStats.eq.equipped].fireRate /= 1.15f;
                playerStats.eq.guns[playerStats.eq.equipped].critChance += 0.075f;
                break;
            case 14:
                // chance not to consume ammo
                break;
            case 15:
                // chance to fire 2 additional bullets in cone
                break;
            case 16:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier *= 1.075f;
                // increase damage further with base damage
                break;
            case 17:
                playerStats.eq.guns[playerStats.eq.equipped].range += 0.02f;
                playerStats.eq.guns[playerStats.eq.equipped].damageGain += 0.15f;
                break;
            case 18:
                playerStats.eq.guns[playerStats.eq.equipped].penetration += 0.06f;
                // +DoT per Penetration
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
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier /= 1.15f;
                break;
            case 1:
                playerStats.eq.guns[playerStats.eq.equipped].fireRate *= 1.2f;
                break;
            case 2:
                playerStats.eq.guns[playerStats.eq.equipped].accuracy *= 1.4f;
                playerStats.eq.guns[playerStats.eq.equipped].range -= 0.03f;
                break;
            case 3:
                playerStats.eq.guns[playerStats.eq.equipped].penetration -= 0.14f;
                break;
                case 4:
                playerStats.eq.guns[playerStats.eq.equipped].critChance -= 0.1f;
                playerStats.eq.guns[playerStats.eq.equipped].critDamage -= 0.15f;
                break;
                case 5:
                playerStats.eq.guns[playerStats.eq.equipped].reloadTime /= 0.7f;
                break;
                case 6:
                playerStats.eq.guns[playerStats.eq.equipped].magazineMultiplier /= 2;
                playerStats.eq.guns[playerStats.eq.equipped].reloadTime /= 1.12f;
                playerStats.DisplayAmmo();
                break;
                case 7:
                playerStats.eq.guns[playerStats.eq.equipped].spreadMultiplyer /= 2;
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier /= 0.7f;
                playerStats.eq.guns[playerStats.eq.equipped].fireRate *= 0.85f;
                break;
                case 8:
                playerStats.eq.guns[playerStats.eq.equipped].pierce -= 1;
                playerStats.eq.guns[playerStats.eq.equipped].critChance -= 0.03f;
                // +1 pierce on crit
                break;
                case 9:
                playerStats.eq.guns[playerStats.eq.equipped].pierce -= 1;
                // pierce efficiency
                break;
                case 10:
                // overload
                break;
                case 11:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier /= 1.05f;
                playerStats.eq.guns[playerStats.eq.equipped].fireRate *= 1.06f;
                playerStats.eq.guns[playerStats.eq.equipped].accuracy *= 1.12f;
                playerStats.eq.guns[playerStats.eq.equipped].penetration -= 0.04f;
                break;
                case 12:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier /= 1.075f;
                playerStats.eq.guns[playerStats.eq.equipped].critDamage -= 0.15f;
                break;
                case 13:
                playerStats.eq.guns[playerStats.eq.equipped].fireRate *= 1.15f;
                playerStats.eq.guns[playerStats.eq.equipped].critChance -= 0.075f;
                break;
                case 14:
                // chance not to consume ammo
                break;
                case 15:
                // chance to fire 2 additional bullets in cone
                break;
            case 16:
                playerStats.eq.guns[playerStats.eq.equipped].damageMultiplier /= 1.075f;
                // increase damage further with base damage
                break;
            case 17:
                playerStats.eq.guns[playerStats.eq.equipped].range -= 0.02f;
                playerStats.eq.guns[playerStats.eq.equipped].damageGain -= 0.15f;
                break;
            case 18:
                playerStats.eq.guns[playerStats.eq.equipped].penetration -= 0.06f;
                // +DoT per Penetration
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
