using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorkbenchTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Workbench table;

    public int order;

    public void OnPointerEnter(PointerEventData eventData)
    {
        table.TooltipOpen(order);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        table.TooltipClose();
    }
}
