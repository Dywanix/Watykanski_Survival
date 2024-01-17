using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject Player, Glow;
    public PlayerController playerStats;
    public Transform Sight;
    public Rigidbody2D Dir;

    public GameObject HealthPotion;
    public GameObject[] Items1, Items2;
    //public int KeysRequired;
    public int[] range1, range2;
    public bool potionGuaranteed;
    int amount;

    void Update()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            playerStats = Player.GetComponent(typeof(PlayerController)) as PlayerController;
        }

        if (Vector3.Distance(transform.position, Player.transform.position) <= 3.5f) //&& playerStats.keys >= KeysRequired)
        {
            Glow.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                //playerStats.SpendKeys(KeysRequired);
                OpenChest();
            }
        }
        else Glow.SetActive(false);
    }

    void OpenChest()
    {
        if (potionGuaranteed)
        {
            Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
            GameObject potion = Instantiate(HealthPotion, Dir.position, transform.rotation);
            Rigidbody2D potion_body = potion.GetComponent<Rigidbody2D>();
            potion_body.AddForce(Sight.up * Random.Range(1.2f, 3.5f), ForceMode2D.Impulse);
        }

        amount = Random.Range(range1[0], range1[1] + 1 + playerStats.luck);
        for (int i = 0; i < amount; i++)
        {
            Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
            GameObject item = Instantiate(Items1[Random.Range(0, Items1.Length)], Dir.position, transform.rotation);
            Rigidbody2D item_body = item.GetComponent<Rigidbody2D>();
            item_body.AddForce(Sight.up * Random.Range(1.3f, 5.0f), ForceMode2D.Impulse);
        }

        amount = Random.Range(range2[0], range2[1] + 1);
        for (int i = 0; i < amount; i++)
        {
            Sight.rotation = Quaternion.Euler(Sight.rotation.x, Sight.rotation.y, Dir.rotation + Random.Range(0f, 360f));
            GameObject item = Instantiate(Items2[Random.Range(0, Items2.Length)], Dir.position, transform.rotation);
            Rigidbody2D item_body = item.GetComponent<Rigidbody2D>();
            item_body.AddForce(Sight.up * Random.Range(1.3f, 5.0f), ForceMode2D.Impulse);
        }

        Destroy(gameObject);
    }
}
