using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Accessory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CraftingTable table;

    public string pack;
    public int order;

    public void OnPointerEnter(PointerEventData eventData)
    {
        table.TooltipOpen(pack, order);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        table.TooltipClose(pack, order);
    }
}
