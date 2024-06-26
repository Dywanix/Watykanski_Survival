using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemHovered : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerController playerStats;

    public int order;
    public bool effect;

    public void OnPointerEnter(PointerEventData eventData)
    {
        playerStats.ItemTooltipOpen(order, effect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        playerStats.ItemTooltipClose();
    }
}
