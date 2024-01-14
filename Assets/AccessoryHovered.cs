using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AccessoryHovered : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Backpack bp;

    public int order;
    public bool equipped, rerolled;

    public void OnPointerEnter(PointerEventData eventData)
    {
        bp.TooltipOpen(order, equipped, rerolled);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bp.TooltipClose();
    }
}
