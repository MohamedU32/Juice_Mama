using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick_Custom : Joystick
{
    private bool isDragging = false;

    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);     // Hide joystick initially
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.gameObject.SetActive(false);
        isDragging = false;
        base.OnPointerUp(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            Debug.Log("dragging");
            background.gameObject.SetActive(true);      // Show joystick on first drag
            background.position = eventData.position;
            isDragging = true;
        }

        base.OnDrag(eventData);
    }

}