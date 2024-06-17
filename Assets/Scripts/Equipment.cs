using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    public PlayerController playerStats;
    public Backpack bp;
    public Camera playerCamera;
    public bool gambler;
    public GunsLibrary Library;
    public ItemsLibrary ILibrary;
    public Transform Barrel;
    public Gun[] guns;
    public int[] Accessories;
    public Image itemImage;
    public SpriteRenderer equippedGun;
    private Bullet firedBullet;

    public bool[] slotFilled;
    public int equipped, item;
    public float onHitIncrease;
    float temp;
    int tempi, roll;

    [Header("On Hit")]
    public GameObject Peacemaker, Boomerange, Wave, Laser, Orb;
    public float[] freeBulletCharges, peacemakerCharges, boomerangCharges, waveCharges, laserCharges, orbCharges;
    public MultipleBullets waveBullet;

    [Header("Active Items")]
    public float itemActivationRate;
    public GameObject BladeProjectal, KnifeProjectal, ImmolateArea, StormCloud;
    public GameObject ImmolateSmallArea, ImmolateMediumArea;
    public float bladesThrowMaxCooldown, bladesThrowCooldown, bladesBaseDamage, bladesPierceEff, knifeThrowMaxCooldown, knifeThrowCooldown, knivesBaseDamage, immolateMaxCooldown, immolateCooldown,
        immolateBaseDamage, immolateHPRatio, rainCooldown, rainFrequency, howitzerMaxCooldown, howitzerCooldown, cloudMaxCooldown, cloudCooldown, cloudBaseDamage, cloudDuration, cloudSpeed;
    public int bladesCount, bladesPierce, knivesCount, grenadeCount;

    // -- items
    //public GameObject[] Drones;
    //public GameObject Caltrop, Knife, Cleaver;
    //public float itemsActivationRate = 1f;

    [Header("Items")]
    public int[] Items;
    public int[] ItemList, EffectList;
    public int[] Effects;
    public int itemsCollected, effectsCollected;
    public GameObject DeflectProjectal, DischargeObject;
    public TMPro.TextMeshProUGUI Tooltip;

    //public GameObject Saw, Laser;
    //public float sawCharges, laserCharges;

    void Start()
    {
        playerStats.gunImage.sprite = guns[equipped].gunSprite;
        equippedGun.sprite = guns[equipped].holdingSprite;
        playerStats.DisplayAmmo();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent(typeof(Camera)) as Camera;
        if (gambler)
        {
            /*tempi = 3;
            while (tempi > 0)
            {
                roll = Random.Range(0, ILibrary.ItemSprite.Length);
                if (Items[roll] < 5)
                {
                    PickUpItem(roll);
                    tempi--;
                }
            }*/
            PickUpItem(Random.Range(0, ILibrary.ItemSprite.Length));
            PickUpEffect(Random.Range(0, ILibrary.Effects.Length));
        }
        //Invoke("AutoReload", 3f);
        //Invoke("ThrowCaltrops", 8f);
        //Invoke("KnifeThrow", 2.85f);
        //Invoke("ThrowSaw", 3.5f);
        //Invoke("ThrowCleaver", 4f);

        /*for (int i = 0; i < 24; i++)
        {
            PickUpItem(i);
        }*/
    }

    void Update()
    {
        if (!playerStats.day)
        {
            if (Effects[0] > 0)
                bladesThrowCooldown -= Time.deltaTime * itemActivationRate * playerStats.SpeedMultiplyer(0.24f) * (1f + playerStats.cooldownReduction * 0.2f);
            if (bladesThrowCooldown < 0f)
                ScissorsThrow();

            if (Effects[1] > 0)
                knifeThrowCooldown -= Time.deltaTime * itemActivationRate * playerStats.SpeedMultiplyer(0.73f) * (1f + playerStats.cooldownReduction * 0.42f);
            if (knifeThrowCooldown < 0f)
                KnifeThrow();

            if (Effects[2] > 0)
                immolateCooldown -= Time.deltaTime * itemActivationRate;
            if (immolateCooldown < 0f)
                Immolate();

            if (Effects[3] > 0)
                rainCooldown -= Time.deltaTime * itemActivationRate * playerStats.SpeedMultiplyer(1.2f);
            if (rainCooldown < 0f)
                Rain();

            if (Effects[4] > 0)
                howitzerCooldown -= Time.deltaTime * itemActivationRate * (1f + playerStats.cooldownReduction * 0.36f);
            if (howitzerCooldown < 0f)
            {
                for (int i = 0; i < grenadeCount; i++)
                {
                    playerStats.GrenadeDrop(10f);
                }
                howitzerCooldown += howitzerMaxCooldown;
            }

            if (Effects[5] > 0)
                cloudCooldown -= Time.deltaTime * itemActivationRate * (1f + playerStats.cooldownReduction * 0.21f);
            if (cloudCooldown < 0f)
                Storm();
        }
     }

    public void PickUpItem(int which)
    {
        Items[which]++;
        if (Items[which] == 1)
        {
            ItemList[itemsCollected] = which;
            itemsCollected++;
        }

        ShowTooltip(which);
        switch (which)
        {
            case 0:
                playerStats.adrenalineCharges += playerStats.totalSlained / 2;
                break;
            case 1:
                guns[equipped].specialBulletChance[0] += 0.05f;
                break;
            case 2:
                playerStats.dashBaseCooldown *= 0.97f;
                break;
            case 3:
                playerStats.GainSC(4);
                playerStats.rechargeDelay /= 1.1f;
                break;
            case 4:
                playerStats.dashBaseCooldown *= 0.85f;
                playerStats.dashMaxCharges++;
                break;
            case 5:
                playerStats.GainCR(0.1f);
                break;
            case 6:
                playerStats.GainHP(10);
                break;
            case 7:
                playerStats.GainDMG((playerStats.maxHealth - 50) * 0.0008f);
                playerStats.GainHP(10);
                break;
            case 8:
                guns[equipped].MaxSlots++;
                break;
            case 9:
                guns[equipped].accuracy /= 1.12f;
                guns[equipped].range *= 1.12f;
                playerCamera.orthographicSize++;
                break;
            case 10:
                playerStats.GainDMG(0.06f);
                playerStats.bloodMoney += playerStats.totalSlained / 2;
                break;
            case 11:
                playerStats.GainSC(4);
                break;
            case 13:
                guns[equipped].magazineMultiplierTenth += 2;
                break;
            case 14:
                playerStats.GainSC(4);
                if (Items[14] == 1)
                    playerStats.emergencyShields = true;
                break;
            case 15:
                playerStats.grenadeDamageMultiplyer += 0.18f;
                playerStats.grenadeBaseCooldown *= 0.9f;
                break;
            case 16:
                playerStats.additionalCritChance += 0.07f;
                playerStats.luck += 2;
                playerStats.map.luck += 2;
                break;
            case 18:
                guns[equipped].specialBulletChance[1] += 0.06f;
                break;
            case 19:
                guns[equipped].specialBulletChance[2] += 0.05f;
                break;
            case 21:
                playerStats.GainDMG(0.06f);
                playerStats.forceIncrease += 0.15f;
                break;
            case 22:
                playerStats.additionalCritDamage += 0.15f;
                break;
            case 23:
                playerStats.GainGold(10f);
                break;
            case 25:
                playerStats.GainDMG(0.06f);
                playerStats.GainFR(0.08f);
                break;
            case 26:
                playerStats.GainSC(4);
                break;
            case 27:
                playerStats.grenadeBaseCooldown *= 0.88f;
                playerStats.grenadeMaxCharges++;
                break;
            case 28:
                playerStats.grenadeBaseCooldown *= 0.91f;
                break;
            case 29:
                playerStats.grenadeMaxCharges++;
                break;
            case 31:
                playerStats.GainDMG(0.06f);
                break;
            case 32:
                playerStats.cooldownReduction += (playerStats.fireRateBonus - 1f) / 4.5f;
                playerStats.GainFR(0.08f);
                break;
            case 33:
                playerStats.GainSC(4 + playerStats.level * 0.6f);
                break;
            case 34:
                guns[equipped].specialBulletChance[3] += 0.05f;
                break;
            case 35:
                playerStats.bloodBagCharges += playerStats.totalSlained / 2;
                break;
            case 36:
                itemActivationRate += 0.21f;
                break;
            case 39:
                guns[equipped].magazineMultiplierTenth += 1;
                playerStats.GainFR(0.09f);
                break;
            case 40:
                guns[equipped].peacemaker += 0.6f;
                break;
            case 41:
                guns[equipped].boomerang += 0.6f;
                break;
            case 42:
                guns[equipped].laser += 0.4f;
                break;
            case 43:
                guns[equipped].peacemaker += 0.3f;
                guns[equipped].boomerang += 0.3f;
                guns[equipped].laser += 0.3f;
                break;
        }
    }

    public void PickUpEffect(int which)
    {
        Effects[which]++;
        if (Effects[which] == 1)
        {
            EffectList[effectsCollected] = which;
            effectsCollected++;
        }

        switch (which, Effects[which])
        {
            case (0, 1):
                bladesThrowMaxCooldown = 4.5f;
                bladesThrowCooldown = 1f + bladesThrowMaxCooldown * 0.5f;
                bladesCount = 6;
                bladesBaseDamage = 20f;
                bladesPierce = 2;
                bladesPierceEff = 0.8f;
                break;
            case (0, 2):
                bladesCount += 4;
                bladesBaseDamage += 2f;
                break;
            case (0, 3):
                bladesBaseDamage += 8f;
                break;
            case (0, 4):
                bladesThrowMaxCooldown *= 0.75f;
                break;
            case (0, 5):
                bladesPierce += 1;
                bladesPierceEff += 0.1f;
                break;
            case (1, 1):
                knifeThrowMaxCooldown = 3.6f;
                knifeThrowCooldown = 1f + knifeThrowMaxCooldown * 0.5f;
                knivesCount = 2;
                knivesBaseDamage = 27f;
                break;
            case (1, 2):
                knivesCount += 1;
                knifeThrowMaxCooldown *= 0.9f;
                break;
            case (1, 3):
                knivesBaseDamage += 7f;
                knifeThrowMaxCooldown *= 0.9f;
                break;
            case (1, 4):
                knivesCount += 2;
                break;
            case (1, 5):
                knifeThrowMaxCooldown *= 0.7f;
                break;
            case (2, 1):
                immolateMaxCooldown = 0.86f;
                immolateCooldown = 1f + immolateMaxCooldown * 0.5f;
                immolateBaseDamage = 6f;
                ImmolateArea = ImmolateSmallArea;
                immolateHPRatio = 0.04f;
                break;
            case (2, 2):
                immolateBaseDamage += 3f;
                break;
            case (2, 3):
                immolateMaxCooldown *= 0.75f;
                break;
            case (2, 4):
                ImmolateArea = ImmolateMediumArea;
                break;
            case (2, 5):
                immolateHPRatio += 0.05f;
                break;
            case (3, 1):
                rainFrequency = 2.28f;
                break;
            case (3, 2):
                rainFrequency *= 0.86f;
                break;
            case (3, 3):
                guns[equipped].damage *= 1.04f;
                rainFrequency *= 0.92f;
                break;
            case (3, 4):
                rainFrequency *= 0.85f;
                break;
            case (3, 5):
                guns[equipped].damage *= 1.05f;
                rainFrequency *= 0.93f;
                break;
            case (4, 1):
                howitzerMaxCooldown = 5.7f;
                howitzerCooldown = 1f + howitzerMaxCooldown * 0.5f;
                grenadeCount = 1;
                break;
            case (4, 2):
                howitzerMaxCooldown *= 0.8f;
                break;
            case (4, 3):
                playerStats.grenadeDamageMultiplyer += 0.07f;
                howitzerMaxCooldown *= 0.93f;
                break;
            case (4, 4):
                grenadeCount++;
                break;
            case (4, 5):
                howitzerMaxCooldown *= 0.78f;
                break;
            case (5, 1):
                cloudMaxCooldown = 18f;
                cloudCooldown = 1f + cloudMaxCooldown * 0.5f;
                cloudDuration = 14f;
                cloudSpeed = 6f;
                cloudBaseDamage += 14f;
                break;
            case (5, 2):
                cloudDuration += 4f;
                cloudSpeed += 1f;
                break;
            case (5, 3):
                cloudBaseDamage += 6f;
                break;
            case (5, 4):
                cloudMaxCooldown *= 0.75f;
                break;
            case (5, 5):
                cloudDuration += 6f;
                cloudBaseDamage += 2f;
                break;
        }
    }

    void ShowTooltip(int which)
    {
        //Tooltip.text = ILibrary.ItemTooltip[ItemList[which]];

        Invoke("HideTooltip", 1f);
    }

    void HideTooltip()
    {
        Tooltip.text = "";
    }

    public void OnHit(float efficiency)
    {
        //Flash();
        onHitIncrease = 1f + 0.3f * guns[equipped].Accessories[26] + 0.48f * guns[equipped].Accessories[26 + bp.ALibrary.count];
        onHitIncrease *= 1f + 0.028f * playerStats.adrenalineStacks;

        freeBulletCharges[equipped] += efficiency * guns[equipped].freeBullet * onHitIncrease;
        if (freeBulletCharges[equipped] >= 5f)
        {
            playerStats.FireDirection(0f, 0f);
            freeBulletCharges[equipped] -= 5f;
        }

        peacemakerCharges[equipped] += efficiency * (1f + 0.24f * guns[equipped].fireRate) * guns[equipped].peacemaker * guns[equipped].BulletsFired() * onHitIncrease;
        if (peacemakerCharges[equipped] >= 11f)
        {
            FirePeacemaker();
            peacemakerCharges[equipped] -= 11f;
        }

        boomerangCharges[equipped] += efficiency * (1f + 0.12f * guns[equipped].fireRate) * guns[equipped].boomerang * onHitIncrease;
        if (boomerangCharges[equipped] >= 10f)
        {
            FireBoomerang();
            boomerangCharges[equipped] -= 10f;
        }

        waveCharges[equipped] += efficiency * guns[equipped].wave * guns[equipped].BulletsFired() * onHitIncrease;
        if (waveCharges[equipped] >= 13f)
        {
            FireWave();
            waveCharges[equipped] -= 13f;
        }

        laserCharges[equipped] += efficiency * (1f + 0.05f * guns[equipped].fireRate) * guns[equipped].laser * onHitIncrease;
        if (laserCharges[equipped] >= 4f)
        {
            FireLaser();
            laserCharges[equipped] -= 4f;
        }

        /*orbCharges[equipped] += efficiency * (1f + 0.07f * guns[equipped].fireRate) * guns[equipped].Accessories[29] * onHitIncrease;
        if (orbCharges[equipped] >= 5f)
        {
            FireOrb();
            orbCharges[equipped] -= 5f;
        }*/
    }

    void FirePeacemaker()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-0.7f * guns[equipped].accuracy, 0.7f * guns[equipped].accuracy));
        GameObject bullet = Instantiate(Peacemaker, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 22f * Random.Range(0.95f, 1.05f), ForceMode2D.Impulse);

        playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        playerStats.SetBullet(1f);
        playerStats.firedBullet.damage *= guns[equipped].onHitModifier;
        playerStats.firedBullet.falloff += 0.4f + 0.4f * playerStats.firedBullet.falloff;
        playerStats.firedBullet.duration = 0.4f + 0.4f * playerStats.firedBullet.duration;

        playerStats.firedBullet.damage *= 1.4f + 0.04f * Items[40] + (0.14f + 0.04f * Items[40]) * playerStats.firedBullet.pierce;
        playerStats.firedBullet.pierceEfficiency += 0.16f * playerStats.firedBullet.pierce;
        playerStats.firedBullet.pierce = 5 + Items[40];
    }

    void FireBoomerang()
    {
        for (int i = 0; i < guns[equipped].BulletsFired(); i++)
        {
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-0.6f * guns[equipped].accuracy - guns[equipped].BulletsFired() * 1f, 0.6f * guns[equipped].accuracy + guns[equipped].BulletsFired() * 1f));
            GameObject bullet = Instantiate(Boomerange, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * 18f * Random.Range(0.96f, 1.04f), ForceMode2D.Impulse);

            playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            playerStats.SetBullet(1f);
            playerStats.firedBullet.damage *= guns[equipped].onHitModifier;
            playerStats.firedBullet.falloff = 40f;
            playerStats.firedBullet.duration = 50f;

            playerStats.firedBullet.damage *= 1.1f + 0.04f * Items[41] + (0.06f + 0.02f * Items[41]) * playerStats.firedBullet.pierce + (0.35f + 0.11f * Items[41]) * playerStats.firedBullet.pierceEfficiency;
            playerStats.firedBullet.pierce += 7 + Items[41];
            playerStats.firedBullet.pierceEfficiency = 0.6f + 0.03f * Items[41] + (0.6f + 0.03f * Items[41]) * playerStats.firedBullet.pierceEfficiency;
        }
    }

    void FireWave()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(guns[equipped].accuracy, guns[equipped].accuracy));
        GameObject bullet = Instantiate(Wave, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        playerStats.SetBullet(1f);
        waveBullet = bullet.GetComponent(typeof(MultipleBullets)) as MultipleBullets;
        if (guns[equipped].targetArea && Vector3.Distance(playerStats.transform.position, new Vector2(playerStats.mousePos[0], playerStats.mousePos[1])) <= guns[equipped].range * 24f)
            waveBullet.bulletForce = 20f * Random.Range(1.07f, 1.08f) * Vector3.Distance(playerStats.transform.position, new Vector2(playerStats.mousePos[0], playerStats.mousePos[1])) / (guns[equipped].range * 23f);
        else waveBullet.bulletForce = playerStats.firedBullet.force;
        waveBullet.BulletShard = guns[equipped].bulletPrefab[Random.Range(0, guns[equipped].bulletPrefab.Length)];
    }

    void FireLaser()
    {
        for (int i = 0; i < guns[equipped].BulletsFired(); i++)
        {
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-0.6f * guns[equipped].accuracy - guns[equipped].BulletsFired() * 1f, 0.6f * guns[equipped].accuracy + guns[equipped].BulletsFired() * 1f));
            GameObject bullet = Instantiate(Laser, playerStats.Barrel.position, playerStats.Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(playerStats.Barrel.up * 0f * Random.Range(0.96f, 1.04f), ForceMode2D.Impulse);

            playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            playerStats.SetBullet(1f);
            playerStats.firedBullet.damage *= guns[equipped].onHitModifier;
            playerStats.firedBullet.falloff = 0.12f;
            playerStats.firedBullet.duration = 0.14f;

            playerStats.firedBullet.damage *= 0.36f + 0.033f * Items[42] + (0.025f + 0.0125f * Items[42]) * playerStats.firedBullet.pierce + (0.06f + 0.03f * Items[42]) * playerStats.firedBullet.pierceEfficiency;
            playerStats.firedBullet.pierce = 10;
            playerStats.firedBullet.pierceEfficiency = 1f;
            playerStats.firedBullet.shatter += 0.6f + 0.045f * Items[42] + (0.3f + 0.65f * Items[42]) * playerStats.firedBullet.shatter;
        }
    }

    void FireOrb()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-0.7f * guns[equipped].accuracy, 0.7f * guns[equipped].accuracy));
        GameObject bullet = Instantiate(Orb, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * 18f * Random.Range(0.95f, 1.05f), ForceMode2D.Impulse);

        playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        playerStats.SetBullet(1f);
        playerStats.firedBullet.damage *= guns[equipped].onHitModifier;
        playerStats.firedBullet.duration = 1.41f;

        playerStats.firedBullet.damage *= 0.16f;
        playerStats.firedBullet.DoT *= 3.8f; playerStats.firedBullet.DoT += 1.6f;
        playerStats.firedBullet.curse *= 8.8f; playerStats.firedBullet.curse += 5.4f;
    }

    public void Deflect(float damage)
    {
        for (int i = 0; i < 3; i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, Random.Range(0f, 80f) + 120f * i);
            GameObject bullet = Instantiate(DeflectProjectal, Barrel.position, Barrel.rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrel.up * Random.Range(16f, 18.2f), ForceMode2D.Impulse);

            firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            firedBullet.damage = damage * (playerStats.DamageDealtMultiplyer(1.27f) - 0.16f);
        }
    }

    public void Discharge(float shieldLost)
    {
        temp = 5f + Items[11] + shieldLost;
        temp *= (1f + 0.05f * playerStats.level) * Items[11];

        GameObject bullet = Instantiate(DischargeObject, Barrel.position, Barrel.rotation);

        firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        firedBullet.damage = temp * (playerStats.DamageDealtMultiplyer(1.05f));
    }

    public void ActivateItems()
    {
        /*if (Items[38] > 0)
            Invoke("KnifeThrow", 2.1f);*/
    }

    public void ScissorsThrow()
    {
        for (int i = 0; i < bladesCount; i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, 0f + (360f / bladesCount) * i);
            GameObject blade = Instantiate(BladeProjectal, Barrel.position, Barrel.rotation);
            Rigidbody2D blade_body = blade.GetComponent<Rigidbody2D>();
            blade_body.AddForce(Barrel.up * Random.Range(17.5f, 18.9f), ForceMode2D.Impulse);

            firedBullet = blade.GetComponent(typeof(Bullet)) as Bullet;
            firedBullet.damage = bladesBaseDamage * (1f + playerStats.level * 0.04f) * playerStats.DamageDealtMultiplyer(1.04f);
            firedBullet.pierce = bladesPierce;
            firedBullet.pierceEfficiency = bladesPierceEff;
        }
        bladesThrowCooldown += bladesThrowMaxCooldown;
    }

    void KnifeThrow()
    {
        for (int i = 0; i < knivesCount; i++)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, playerStats.Gun.rotation - 6f * knivesCount + 6f + 12f * i);
            GameObject knife = Instantiate(KnifeProjectal, Barrel.position, Barrel.rotation);
            Rigidbody2D knife_body = knife.GetComponent<Rigidbody2D>();
            knife_body.AddForce(Barrel.up * Random.Range(17.3f, 18.7f), ForceMode2D.Impulse);

            firedBullet = knife.GetComponent(typeof(Bullet)) as Bullet;
            firedBullet.damage = knivesBaseDamage * (1f + playerStats.level * 0.04f) * playerStats.DamageDealtMultiplyer(1.07f);
        }
        knifeThrowCooldown += knifeThrowMaxCooldown;
    }

    void Immolate()
    {
        GameObject fire = Instantiate(ImmolateArea, Barrel.position, Barrel.rotation);

        firedBullet = fire.GetComponent(typeof(Bullet)) as Bullet;
        firedBullet.damage = (immolateBaseDamage + playerStats.maxHealth * immolateHPRatio) * (1f + playerStats.level * 0.04f) * playerStats.DamageDealtMultiplyer(1.1f);

        immolateCooldown += immolateMaxCooldown;
    }

    void Rain()
    {
        if (guns[equipped].burst > 0)
        {
            for (int i = 0; i < guns[equipped].burst; i++)
            {
                playerStats.FireDirection(Random.Range(0f, 360f), 0f);
            }
        }
        playerStats.FireDirection(Random.Range(0f, 360f), 0f);

        rainCooldown += guns[equipped].fireRate * rainFrequency;
    }

    void Storm()
    {
        GameObject fire = Instantiate(StormCloud, Barrel.position, Barrel.rotation);

        firedBullet = fire.GetComponent(typeof(Bullet)) as Bullet;
        firedBullet.damage = cloudBaseDamage * (1f + playerStats.level * 0.04f) * playerStats.DamageDealtMultiplyer(1f);
        firedBullet.duration = cloudDuration;

        cloudCooldown += cloudMaxCooldown;
    }

    /*void ThrowCaltrops()
    {
        if (Items[0] > 0 && !playerStats.day)
        {
            for (int i = 0; i < 5 * Items[0]; i++)
            {
                Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, playerStats.Gun.rotation + (Random.Range(-4f, 4f) + i * 72f) / Items[0]);
                GameObject caltrop = Instantiate(Caltrop, Barrel.position, Barrel.rotation);
                Rigidbody2D caltrop_body = caltrop.GetComponent<Rigidbody2D>();
                caltrop_body.AddForce(Barrel.up * Random.Range(2.45f, 2.79f), ForceMode2D.Impulse);

                firedBullet = caltrop.GetComponent(typeof(Bullet)) as Bullet;
                firedBullet.damage *= playerStats.DamageDealtMultiplyer(1f);
            }
        }

        Invoke("ThrowCaltrops", 8f / itemsActivationRate);
    }

    void KnifeThrow()
    {
        if (Items[1] > 0 && !playerStats.day)
        {
            for (int i = 0; i < 2 * Items[1]; i++)
            {
                Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, playerStats.Gun.rotation - 6f - 3f * Items[1] + (3f + 6f / Items[1]) * i);
                GameObject knife = Instantiate(Knife, Barrel.position, Barrel.rotation);
                Rigidbody2D knife_body = knife.GetComponent<Rigidbody2D>();
                knife_body.AddForce(Barrel.up * Random.Range(17.5f, 18.9f), ForceMode2D.Impulse);

                firedBullet = knife.GetComponent(typeof(Bullet)) as Bullet;
                firedBullet.damage *= playerStats.DamageDealtMultiplyer(1f);
            }
        }

        temp = (2.85f / (1f + playerStats.SpeedMultiplyer(0.5f))) / itemsActivationRate;
        Invoke("KnifeThrow", temp);
    }

    void ThrowSaw()
    {
        if (Items[2] > 0 && !playerStats.day)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(0f, 360f));
            GameObject saw = Instantiate(Saw, Barrel.position, Barrel.rotation);
            Rigidbody2D saw_body = saw.GetComponent<Rigidbody2D>();
            saw_body.AddForce(Barrel.up * Random.Range(17.3f, 18.65f), ForceMode2D.Impulse);

            firedBullet = saw.GetComponent(typeof(Bullet)) as Bullet;
            firedBullet.damage *= playerStats.DamageDealtMultiplyer(1.12f);
        }

        temp = 3.5f / itemsActivationRate;

        temp /= 0.1f + 0.9f * Items[2];

        Invoke("ThrowSaw", temp);
    }

    void ThrowCleaver()
    {
        if (Items[10] > 0 && !playerStats.day)
        {
            Barrel.rotation = Quaternion.Euler(Barrel.rotation.x, Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-(12 + 2 * Items[10]), (12 + 2 * Items[10])));
            GameObject cleaver = Instantiate(Cleaver, Barrel.position, Barrel.rotation);
            Rigidbody2D cleaver_body = cleaver.GetComponent<Rigidbody2D>();
            cleaver_body.AddForce(Barrel.up * Random.Range(16.25f, 17.55f), ForceMode2D.Impulse);

            firedBullet = cleaver.GetComponent(typeof(Bullet)) as Bullet;
            firedBullet.damage = 40 + 0.05f * playerStats.maxHealth + 9 * Items[10];
            firedBullet.damage *= playerStats.DamageDealtMultiplyer(1f);
        }

        temp = (4f / (1f + playerStats.SpeedMultiplyer(0.6f))) / itemsActivationRate;

        temp /= 0.25f + 0.75f * Items[10];

        Invoke("ThrowCleaver", temp);
    }

    void AutoReload()
    {
        if (guns[equipped].bulletsLeft < guns[equipped].magazineSize)
        {
            guns[equipped].bulletsLeft += guns[equipped].Accessories[4 + playerStats.accessoriesPerType * 2];
            playerStats.DisplayAmmo();

            if (guns[equipped].individualReload)
            {
                Invoke("AutoReload", (0.75f + 10f * guns[equipped].reloadTime));
            }
            else Invoke("AutoReload", (0.75f + 10f * guns[equipped].reloadTime / (2 + guns[equipped].magazineSize)));
        }
        else Invoke("AutoReload", 2f);
    }

    public void SpecialCharges()
    {
        sawCharges += guns[equipped].Accessories[4] * guns[equipped].BulletsFired() * (1f + 0.15f * guns[equipped].Accessories[4 + playerStats.accessoriesPerType * 3]);
        if (sawCharges >= 12f)
        {
            FireSaw();
            sawCharges -= 12f;
        }

        laserCharges += guns[equipped].Accessories[4 + playerStats.accessoriesPerType] * (1f + 0.15f * guns[equipped].Accessories[4 + playerStats.accessoriesPerType * 3]);
        if (laserCharges >= 7f)
        {
            FireLaser();
            laserCharges -= 7f;
        }
    }

    public void FireSaw()
    {
        playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-(3f + 1.1f * guns[equipped].accuracy), (3f + 1.1f * guns[equipped].accuracy)));
        GameObject bullet = Instantiate(Saw, playerStats.Barrel.position, playerStats.Barrel.rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(playerStats.Barrel.up * guns[equipped].force * 1.19f * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);

        playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
        playerStats.SetBullet(1f);

        playerStats.firedBullet.damage *= 1.2f;
        playerStats.firedBullet.penetration += 0.12f;
        playerStats.firedBullet.DoT += 0.25f + 0.06f * playerStats.firedBullet.DoT;
        playerStats.firedBullet.pierce += 3;
        playerStats.firedBullet.pierceEfficiency *= 0.6f; playerStats.firedBullet.pierceEfficiency += 0.6f;
    }

    public void FireLaser()
    {
        for (int i = 0; i < guns[equipped].BulletsFired(); i++)
        {
            playerStats.Barrel.rotation = Quaternion.Euler(playerStats.Barrel.rotation.x, playerStats.Barrel.rotation.y, playerStats.Gun.rotation + Random.Range(-(1f + 0.7f * guns[equipped].accuracy), (1f + .7f * guns[equipped].accuracy)));
            GameObject bullet = Instantiate(Laser, playerStats.Barrel.position, playerStats.Barrel.rotation);

            playerStats.firedBullet = bullet.GetComponent(typeof(Bullet)) as Bullet;
            playerStats.SetBullet(1f);

            playerStats.firedBullet.damage *= 0.44f;
            playerStats.firedBullet.penetration *= 0.8f; playerStats.firedBullet.penetration += 0.05f;
            playerStats.firedBullet.vulnerableApplied += 0.05f + 0.15f * guns[equipped].fireRate;
            playerStats.firedBullet.pierce = 50;
            playerStats.firedBullet.pierceEfficiency = 1f;
        }
    }*/
}
