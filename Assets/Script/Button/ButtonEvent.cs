using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonEvent : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent rightClick;
    public RightMenu RightMenu;

    private void Start()
    {

        rightClick.AddListener(new UnityAction(ButtonRightClick));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            rightClick.Invoke();
    }

    private void ButtonRightClick()
    {
        RightMenu.Open(gameObject);
    }
}
