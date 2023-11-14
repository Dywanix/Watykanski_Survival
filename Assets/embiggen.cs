using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class embiggen : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform Card;
    public Vector3 scaleChange;

    void Start()
    {
        Card.localScale = new Vector3(1f, 1f, 1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Card.localScale += scaleChange;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Card.localScale -= scaleChange;
    }
}
