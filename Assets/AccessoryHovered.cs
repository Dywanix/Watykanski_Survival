using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AccessoryHovered : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Backpack bp;

    public int order;
    public bool equipped;

    public void OnPointerEnter(PointerEventData eventData)
    {
        bp.TooltipOpen(order, equipped);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bp.TooltipClose();
    }
}
