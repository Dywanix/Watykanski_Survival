using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public SpriteRenderer[] RandomSprite, RandomGlow;
    public RandomToBuy[] RandomToBuys;

    public Sprite[] Sprites;
    public int[] Roll;

    public void Start()
    {
        Roll[0] = Random.Range(0, Sprites.Length);

        do
        {
            Roll[1] = Random.Range(0, Sprites.Length);
        } while (Roll[1] == Roll[0]);

        do
        {
            Roll[2] = Random.Range(0, Sprites.Length);
        } while (Roll[2] == Roll[0] || Roll[2] == Roll[1]);

        for (int i = 0; i < 3; i++)
        {
            RandomSprite[i].sprite = Sprites[Roll[i]];
            RandomGlow[i].sprite = Sprites[Roll[i]];
            RandomToBuys[i].effect = Roll[i];
        }
    }
}
