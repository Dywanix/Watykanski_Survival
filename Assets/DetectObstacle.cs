using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectObstacle : MonoBehaviour
{
    public Enemy enemy;
    public Wall wall;
    public Transform CurrentEnd;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.transform.tag == "Obstacle")
        {
            wall = other.GetComponent<Wall>();
            if (Vector3.Distance(wall.Ends[0].position, enemy.Player.transform.position) <  Vector3.Distance(wall.Ends[1].position, enemy.Player.transform.position))
                enemy.FoundObstacle(wall.Ends[0]);
            else enemy.FoundObstacle(wall.Ends[1]);
        }
    }


    /*public int CheckSides()
    {
        checks = 0;

        if (Left.IsTouching(Obstacle));
        {
            checks += 1;
        }

        if (Right.IsTouching(Obstacle));
        {
            checks += 2;
        }

        return checks;
    }*/
}
