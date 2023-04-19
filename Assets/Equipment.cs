using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    public Gun[] guns;
    public GameObject Caltrop, Turret;
    public GameObject[] gunSprite;
    public int[] Items, MaxItems, Accessories;
    public Image itemImage;
    public SpriteRenderer equippedGun;
    public Sprite[] itemSprites;

    public int equipped, item;
}
