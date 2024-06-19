using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaken : MonoBehaviour
{
    public TMPro.TextMeshProUGUI damageText;

    public void SetText(float damage, string hue)
    {
        damageText.fontSize = 40 + damage * 0.18f;
        switch (hue)
        {
            case "orange":
                damageText.color = new Color(1, 1f, 0.1f, 1); //yellow tbf
                break;
            case "red":
                damageText.color = new Color(1, 0.4f, 0, 1); //orange tbf
                damageText.fontSize += 8f;
                break;
            case "green":
                damageText.color = new Color(0.388f, 0.709f, 0.063f, 1);
                break;
            case "cyan":
                damageText.color = new Color(0.361f, 0.847f, 0.925f, 1);
                break;
        }
        damageText.text = damage.ToString("0");
    }
}
