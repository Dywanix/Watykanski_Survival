using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini_Boss1 : MonoBehaviour
{
    public Enemy enemy;
    public GameObject Projectal;
    public Transform[] Barrels;
    public SpriteRenderer GunImages;
    public Sprite[] gunSprites;

    public int switches;
    int currentGun, waves;
    //public GameObject Rocket;
    //public Transform[] ShoulderBarrels;

    //int currentShoulder;

    void Start()
    {
        //Invoke("ShoulderFire", 4f);
        Invoke("ChangeMode", 15f);
    }

    void ChangeMode()
    {
        if (currentGun == 0)
            ChangeTo(1);
        else ChangeTo(0);
        switches++;
    }

    void ChangeTo(int which)
    {
        enemy.GainStun(1f);
        currentGun = which;
        GunImages.sprite = gunSprites[which];

        switch (which)
        {
            case 0:
                enemy.attackSpeed = 0.267f;
                enemy.bulletCount = 1;
                enemy.accuracy = 15f;
                break;
            case 1:
                enemy.GainStun(0.4f);
                enemy.attackSpeed = 2.34f;
                enemy.bulletCount = 6;
                enemy.accuracy = 6f;
                break;
        }

        waves = 3 + switches / 3;
        for (int i = 0; i < waves; i++)
        {
            Invoke("AoE", 0.4f + (0.18f * i));
        }

        Invoke("ChangeMode", 9f + 1f * switches);
    }

    void AoE()
    {
        for (int i = 0; i < Barrels.Length; i++)
        {
            GameObject bullet = Instantiate(Projectal, Barrels[i].position, Barrels[i].rotation);
            Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
            bullet_body.AddForce(Barrels[i].up * 10.3f, ForceMode2D.Impulse);
        }
    }

    /*void ShoulderFire()
    {
        currentShoulder = Random.Range(0, 2);

        FireFromShoulder();

        if (currentShoulder == 0)
            currentShoulder = 1;
        else currentShoulder = 0;

        Invoke("FireFromShoulder", 0.75f);

        Invoke("ShoulderFire", 9.15f);
    }*/

    /*void FireFromShoulder()
    {
        GameObject bullet = Instantiate(Rocket, ShoulderBarrels[currentShoulder].position, ShoulderBarrels[currentShoulder].rotation);
        Rigidbody2D bullet_body = bullet.GetComponent<Rigidbody2D>();
        bullet_body.AddForce(ShoulderBarrels[currentShoulder].up * 8f * Random.Range(0.92f, 1.08f), ForceMode2D.Impulse);
    }*/
}
