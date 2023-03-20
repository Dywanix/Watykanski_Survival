using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaken : MonoBehaviour
{
    public TMPro.TextMeshProUGUI damageText;

    public void SetText(float damage, bool crited)
    {
        if (crited)
        {
            damageText.fontSize = 48;
            damageText.color = new Color(1, 0, 0, 1);
            damageText.text = damage.ToString("0") + "!";
        }
        else damageText.text = damage.ToString("0");
    }
}
