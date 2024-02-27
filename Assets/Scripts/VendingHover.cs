using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendingHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public VendingMachine vendingMachine;

    public int slot;

    public void OnPointerEnter(PointerEventData eventData)
    {
        vendingMachine.SlotTooltipOpen(slot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        vendingMachine.SlotTooltipClose();
    }
}
