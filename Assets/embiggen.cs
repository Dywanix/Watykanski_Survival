using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class embiggen : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform Card;
    public Vector3 scaleChange;

    public void EmSmollen()
    {
        Card.localScale -= scaleChange;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Card.localScale += scaleChange;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EmSmollen();
    }
}
