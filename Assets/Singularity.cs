using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singularity : MonoBehaviour
{
    public Bullet ThisBullet, BulletShard;
    public GameObject BulletTick;
    public Rigidbody2D Dir;
    public float frequency, delay;

    [Header("Singularity Stats")]
    public float grow;
    public float additionalArea;
    public bool empowered;

    [Header("Singularity Areas")]
    public GameObject[] AreaObject;
    public float[] AreaSizeProc;
    private int count, areas;

    void Start()
    {
        if (empowered)
        {
            grow = 0.048f * (ThisBullet.playerAreaBonus * 0.3f + 0.7f);
            additionalArea = 0.33f * (ThisBullet.playerAreaBonus * 0.18f + 0.82f);
        }
        else
        {
            grow = 0.032f * (ThisBullet.playerAreaBonus * 0.28f + 0.72f);
            additionalArea = 0.44f * (ThisBullet.playerAreaBonus * 0.2f + 0.8f);
        }
        areas = 1;
        DisplayAreas();
    }

    void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0)
            Tick();
    }

    void Tick()
    {
        count = 0;
        for (float i = ThisBullet.areaSize; i >= 0.26f; i -= additionalArea)
        {
            if (count < AreaObject.Length + 1)
            {
                Area(i);
                count++;
            }
        }
        delay += frequency;
    }

    void Area(float area)
    {
        GameObject bullet = Instantiate(BulletTick, transform.position, transform.rotation);
        BulletShard = bullet.GetComponent(typeof(Bullet)) as Bullet;
        bullet.transform.localScale = new Vector3(area, area, 1f);

        BulletShard.damage = ThisBullet.damage;
        if (empowered)
            BulletShard.damage *= 1f + 0.1f * count;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Enemy")
        {
            ThisBullet.areaSize += grow;
            transform.localScale = new Vector3(ThisBullet.areaSize, ThisBullet.areaSize, 1f);
            DisplayAreas();
        }
    }

    void DisplayAreas()
    {
        while (ThisBullet.areaSize - additionalArea * areas >= 0.26f && areas < AreaObject.Length + 1)
        {
            AreaObject[areas - 1].SetActive(true);
            areas++;
        }
        for (int i = 0; i < areas - 1; i++)
        {
            AreaSizeProc[i] = (ThisBullet.areaSize - additionalArea * (i + 1)) / ThisBullet.areaSize;
            AreaObject[i].transform.localScale = new Vector3(AreaSizeProc[i], AreaSizeProc[i], 1f);
        }
    }
}
