using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftOver : MonoBehaviour
{
    public Enemy enemy;

    public GameObject[] Objects;
    public int[] Count;
    public float[] ForceMin, ForceMax;

    public void Trigger()
    {
        for (int i = 0; i < Objects.Length; i++)
        {
            for (int j = 0; j < Count[i]; j++)
            {
                enemy.Sight.rotation = Quaternion.Euler(enemy.Sight.rotation.x, enemy.Sight.rotation.y, enemy.Dir.rotation + Random.Range(0, 360f));
                GameObject thing = Instantiate(Objects[i], enemy.Dir.position, enemy.Sight.rotation);
                Rigidbody2D thing_body = thing.GetComponent<Rigidbody2D>();
                thing_body.AddForce(enemy.Sight.up * Random.Range(ForceMin[i], ForceMax[i]), ForceMode2D.Impulse);
            }
        }
    }
}
