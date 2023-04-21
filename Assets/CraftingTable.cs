using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTable : MonoBehaviour
{
    public GameObject Player, Glow, Hud;
    // accessory shit
    public Image[] AInEq, AFront, ATop, ABottom, ARear;
    public Button[] EQButtons, FButtons, TButtons, BButtons, RButtons;
    public Sprite[] AccessorySprite;
    public int[] AEQValues, FValues, TValues, BValues, RValues, Weights;
    public GameObject[] tooltips;

    public PlayerController playerStats;
    public TMPro.TextMeshProUGUI Front, Top, Bottom, Rear;

    int current, tempi;
    bool active;

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

    void UpdateInfo()
    {
        // Eq
        for (int i = 0; i < AInEq.Length; i++)
        {
            AInEq[i].enabled = false;
        }
        for (int i = 0; i < AFront.Length; i++)
        {
            AFront[i].enabled = false;
            ATop[i].enabled = false;
            ABottom[i].enabled = false;
            ARear[i].enabled = false;
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

                    if (i < playerStats.accessoriesPerType)
                    {
                        if (Weights[i] <= playerStats.eq.guns[playerStats.eq.equipped].MaxSlots[0] - playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[0])
                            EQButtons[current].interactable = true;
                        else EQButtons[current].interactable = false;
                    }
                    else if (i < playerStats.accessoriesPerType * 2)
                    {
                        if (Weights[i] <= playerStats.eq.guns[playerStats.eq.equipped].MaxSlots[1] - playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[1])
                            EQButtons[current].interactable = true;
                        else EQButtons[current].interactable = false;
                    }
                    else if (i < playerStats.accessoriesPerType * 3)
                    {
                        if (Weights[i] <= playerStats.eq.guns[playerStats.eq.equipped].MaxSlots[2] - playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[2])
                            EQButtons[current].interactable = true;
                        else EQButtons[current].interactable = false;
                    }
                    else
                    {
                        if (Weights[i] <= playerStats.eq.guns[playerStats.eq.equipped].MaxSlots[3] - playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[3])
                            EQButtons[current].interactable = true;
                        else EQButtons[current].interactable = false;
                    }

                    current++;
                }
            }
        }
        // --Gun--
        // Front
        current = 0;
        for (int i = 0; i < playerStats.accessoriesPerType; i++)
        {
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[i] > 0)
            {
                for (int j = 0; j < playerStats.eq.guns[playerStats.eq.equipped].Accessories[i]; j++)
                {
                    AFront[current].sprite = AccessorySprite[i];
                    AFront[current].enabled = true;
                    FValues[current] = i;
                    current++;
                }
            }
        }
        Front.text = playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[0].ToString("0") + "/" + playerStats.eq.guns[playerStats.eq.equipped].MaxSlots[0].ToString("0");

        // Top
        current = 0;
        for (int i = playerStats.accessoriesPerType; i < playerStats.accessoriesPerType * 2; i++)
        {
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[i] > 0)
            {
                for (int j = 0; j < playerStats.eq.guns[playerStats.eq.equipped].Accessories[i]; j++)
                {
                    ATop[current].sprite = AccessorySprite[i];
                    ATop[current].enabled = true;
                    TValues[current] = i;
                    current++;
                }
            }
        }
        Top.text = playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[1].ToString("0") + "/" + playerStats.eq.guns[playerStats.eq.equipped].MaxSlots[1].ToString("0");

        // Bottom
        current = 0;
        for (int i = playerStats.accessoriesPerType * 2; i < playerStats.accessoriesPerType * 3; i++)
        {
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[i] > 0)
            {
                for (int j = 0; j < playerStats.eq.guns[playerStats.eq.equipped].Accessories[i]; j++)
                {
                    ABottom[current].sprite = AccessorySprite[i];
                    ABottom[current].enabled = true;
                    BValues[current] = i;
                    current++;
                }
            }
        }
        Bottom.text = playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[2].ToString("0") + "/" + playerStats.eq.guns[playerStats.eq.equipped].MaxSlots[2].ToString("0");

        // Rear
        current = 0;
        for (int i = playerStats.accessoriesPerType * 3; i < playerStats.accessoriesPerType * 4; i++)
        {
            if (playerStats.eq.guns[playerStats.eq.equipped].Accessories[i] > 0)
            {
                for (int j = 0; j < playerStats.eq.guns[playerStats.eq.equipped].Accessories[i]; j++)
                {
                    ARear[current].sprite = AccessorySprite[i];
                    ARear[current].enabled = true;
                    RValues[current] = i;

                    if (i == playerStats.accessoriesPerType * 3)
                    {
                        if (playerStats.eq.guns[playerStats.eq.equipped].MaxSlots[0] - playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[0] > 0 && playerStats.eq.guns[playerStats.eq.equipped].MaxSlots[1] - playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[1] > 0 && playerStats.eq.guns[playerStats.eq.equipped].MaxSlots[2] - playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[2] > 0)
                            EQButtons[RValues[current]].interactable = true;
                        else EQButtons[RValues[current]].interactable = false;
                    }

                    current++;
                }
            }
        }
        Rear.text = playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[3].ToString("0") + "/" + playerStats.eq.guns[playerStats.eq.equipped].MaxSlots[3].ToString("0");
    }

    public void TooltipOpen(string pack, int order)
    {
        switch (pack)
        {
            case "eq":
                tempi = AEQValues[order];
                break;
            case "front":
                tempi = FValues[order];
                break;
            case "top":
                tempi = TValues[order];
                break;
            case "bottom":
                tempi = BValues[order];
                break;
            case "rear":
                tempi = RValues[order];
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
            case "front":
                tempi = FValues[order];
                break;
            case "top":
                tempi = TValues[order];
                break;
            case "bottom":
                tempi = BValues[order];
                break;
            case "rear":
                tempi = RValues[order];
                break;
        }
        tooltips[tempi].SetActive(false);
    }

    public void Equip(int which)
    {
        playerStats.eq.Accessories[AEQValues[which]]--;
        playerStats.eq.guns[playerStats.eq.equipped].Accessories[AEQValues[which]]++;

        if (AEQValues[which] < playerStats.accessoriesPerType)
        {
            playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[0] += Weights[AEQValues[which]];
        }
        else if (AEQValues[which] < playerStats.accessoriesPerType * 2)
        {
            playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[1] += Weights[AEQValues[which]];
        }
        else if (AEQValues[which] < playerStats.accessoriesPerType * 3)
        {
            playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[2] += Weights[AEQValues[which]];
        }
        else
        {
            playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[3] += Weights[AEQValues[which]];
        }

        GainEffect(AEQValues[which]);
        tooltips[AEQValues[which]].SetActive(false);

        UpdateInfo();
    }

    public void UnEquipF(int which)
    {
        playerStats.eq.Accessories[FValues[which]]++;
        playerStats.eq.guns[playerStats.eq.equipped].Accessories[FValues[which]]--;

        playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[0] -= Weights[FValues[which]];

        LoseEffect(FValues[which]);
        tooltips[FValues[which]].SetActive(false);

        UpdateInfo();
    }

    public void UnEquipT(int which)
    {
        playerStats.eq.Accessories[TValues[which]]++;
        playerStats.eq.guns[playerStats.eq.equipped].Accessories[TValues[which]]--;

        playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[1] -= Weights[TValues[which]];

        LoseEffect(TValues[which]);
        tooltips[TValues[which]].SetActive(false);

        UpdateInfo();
    }

    public void UnEquipB(int which)
    {
        playerStats.eq.Accessories[BValues[which]]++;
        playerStats.eq.guns[playerStats.eq.equipped].Accessories[BValues[which]]--;

        playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[2] -= Weights[BValues[which]];

        LoseEffect(BValues[which]);
        tooltips[BValues[which]].SetActive(false);

        UpdateInfo();
    }

    public void UnEquipR(int which)
    {
        playerStats.eq.Accessories[RValues[which]]++;
        playerStats.eq.guns[playerStats.eq.equipped].Accessories[RValues[which]]--;

        playerStats.eq.guns[playerStats.eq.equipped].TakenSlots[3] -= Weights[RValues[which]];

        LoseEffect(RValues[which]);
        tooltips[RValues[which]].SetActive(false);

        UpdateInfo();
    }

    void GainEffect(int which)
    {
        tempi = 0;
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
                        playerStats.eq.guns[playerStats.eq.equipped].bulletSpread *= 2;
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
                            playerStats.eq.guns[playerStats.eq.equipped].MaxSlots[i]++;
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
                        // Cooldowns
                        break;
                    case 4:
                        // On-shot effects rate
                        break;
                }
                break;
        }
    }

    void LoseEffect(int which)
    {
        tempi = 0;
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
                        playerStats.eq.guns[playerStats.eq.equipped].bulletSpread /= 2;
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
                            playerStats.eq.guns[playerStats.eq.equipped].MaxSlots[i]--;
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
                        // cooldowns
                        break;
                    case 4:
                        // On-shot Effects rate
                        break;
                }
                break;
        }
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
