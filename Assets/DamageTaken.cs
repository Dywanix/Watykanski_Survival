using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaken : MonoBehaviour
{
    public TMPro.TextMeshProUGUI damageText;

    public void SetText(float damage, string hue)
    {
        damageText.fontSize = 42 + damage * 0.22f;
        switch (hue)
        {
            case "orange":
                damageText.color = new Color(1, 0.5f, 0, 1);
                break;
            case "red":
                damageText.color = new Color(1, 0, 0, 1);
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
