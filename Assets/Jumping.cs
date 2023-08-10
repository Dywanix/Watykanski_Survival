using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumping : MonoBehaviour
{
    public Transform form;
    public float jumpHeight, jumpDuration;
    float fall, jumpValue;
    public bool rolling;

    void Start()
    {
        jumpHeight *= Random.Range(0.94f, 1.06f);
        jumpValue = jumpHeight;
        fall = jumpHeight * 2 / jumpDuration;
    }

    void Update()
    {
        if (!rolling)
        {
            form.position = new Vector3(form.position.x, form.position.y + jumpHeight * Time.deltaTime, 0);
            jumpHeight -= fall * Time.deltaTime;
            if (jumpHeight < -jumpValue)
                Bounce();
        }
    }

    void Bounce()
    {
        if (jumpValue > 1.8f)
        {
            jumpValue *= 0.65f;
            jumpValue -= 0.3f;
            jumpHeight = jumpValue;
            jumpDuration *= 0.88f;
            fall = jumpHeight * 2 / jumpDuration;
        }
        else rolling = true;
    }
}
