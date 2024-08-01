using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTaken : MonoBehaviour
{
    public TMPro.TextMeshProUGUI damageText;

    public void SetText(float damage, string hue)
    {
        damageText.fontSize = 36 + damage * 0.125f;
        switch (hue)
        {
            case "base":
                damageText.color = new Color(1, 1f, 1f, 1); //white
                break;
            case "baseCrit":
                damageText.color = new Color(1, 0.85f, 0.4f, 1); //yellow?
                damageText.fontSize += 5f;
                break;
            case "green":
                damageText.color = new Color(0.388f, 0.709f, 0.063f, 1); //green
                break;
            case "cyan":
                damageText.color = new Color(0.361f, 0.847f, 0.925f, 1); //cyan
                break;
            case "burn":
                damageText.color = new Color(0.9f, 0.57f, 0.22f, 1); //orange
                break;
            case "burnCrit":
                damageText.color = new Color(0.7f, 0f, 0f, 1); //dark orange / red
                break;
        }
        damageText.text = damage.ToString("0");
    }
}
