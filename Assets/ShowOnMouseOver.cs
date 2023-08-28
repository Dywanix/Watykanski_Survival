using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject ThingToShow;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ThingToShow.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ThingToShow.SetActive(false);
    }
}
