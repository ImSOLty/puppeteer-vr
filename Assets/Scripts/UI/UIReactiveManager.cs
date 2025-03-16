using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;

enum ElementType
{
    SLIDER, BUTTON, SCROLL_RECT, DROPDOWN, TOGGLE, CUSTOM_ELEMENT, UNKNOWN
}

public class UIReactiveManager : MonoBehaviour
{
    [SerializeField] private float scrollingSpeed = 0.1f;
    [SerializeField] private LayerMask UILayer;
    public void PointerClick(PointerEventArgs e)
    {
        switch (DefineUIElement(e.target))
        {
            case ElementType.BUTTON:
                // Debug.Log("Clicked on button!");
                var button = e.target.GetComponent<Button>();
                if (button.onClick != null && button.IsActive() && button.IsInteractable())
                {
                    button.onClick.Invoke();
                }
                break;
            case ElementType.DROPDOWN:
                // Debug.Log("Clicked on button!");
                var dropdown = e.target.GetComponent<Dropdown>();
                dropdown.Show();
                break;
            case ElementType.TOGGLE:
                // Debug.Log("Clicked on button!");
                var toggle = e.target.GetComponent<Toggle>();
                toggle.Select();
                break;
            case ElementType.SLIDER:
                break;
            case ElementType.CUSTOM_ELEMENT:
                var reactiveElement = e.target.GetComponent<UICustomReactiveElement>();
                reactiveElement.OnPointerClick(e);
                break;
            default:
                break;
        }
    }
    public void PointerIn(PointerEventArgs e)
    {
        switch (DefineUIElement(e.target))
        {
            case ElementType.BUTTON:
                // Debug.Log("Entered button collider!");
                break;
            case ElementType.SLIDER:
                break;
            case ElementType.CUSTOM_ELEMENT:
                var reactiveElement = e.target.GetComponent<UICustomReactiveElement>();
                reactiveElement.OnPointerIn(e);
                break;
            default:
                break;
        }
    }
    public void PointerOut(PointerEventArgs e)
    {
        switch (DefineUIElement(e.target))
        {
            case ElementType.BUTTON:
                // Debug.Log("Out of button!");
                break;
            case ElementType.SLIDER:
                break;

            case ElementType.CUSTOM_ELEMENT:
                var reactiveElement = e.target.GetComponent<UICustomReactiveElement>();
                reactiveElement.OnPointerOut(e);
                break;
            default:
                break;
        }
    }
    public void PointerHold(PointerEventArgs e)
    {
        switch (DefineUIElement(e.target))
        {
            case ElementType.BUTTON:
                break;
            case ElementType.SLIDER:
                var slider = e.target.GetComponent<Slider>();
                float percentX = (e.hit.point.x - e.hit.collider.bounds.min.x) / e.hit.collider.bounds.size.x;
                slider.value = slider.minValue + (slider.maxValue - slider.minValue) * percentX;
                break;
            case ElementType.CUSTOM_ELEMENT:
                var reactiveElement = e.target.GetComponent<UICustomReactiveElement>();
                reactiveElement.OnPointerHold(e);
                break;
            default:
                break;
        }
    }

    public void PointerRelease(PointerEventArgs e)
    {
        switch (DefineUIElement(e.target))
        {
            case ElementType.BUTTON:
                break;
            case ElementType.SLIDER:
                Debug.Log("Released!");
                break;
            case ElementType.CUSTOM_ELEMENT:
                var reactiveElement = e.target.GetComponent<UICustomReactiveElement>();
                reactiveElement.OnPointerRelease(e);
                break;
            default:
                break;
        }
    }

    public void JoystickMove(JoystickEventArgs e)
    {
        switch (DefineUIElement(e.target))
        {
            case ElementType.SCROLL_RECT:
                ScrollRect scroll = e.target.GetComponent<ScrollRect>();
                Scrollbar scrollbar = scroll.verticalScrollbar;
                scrollbar.value = Mathf.Clamp01(scrollbar.value + e.axis.y * scrollingSpeed);
                break;
            default:
                break;
        }
    }
    private ElementType DefineUIElement(Transform targetTransform)
    {
        if (targetTransform.GetComponent<Button>() != null)
        {
            return ElementType.BUTTON;
        }
        else if (targetTransform.GetComponent<Slider>() != null)
        {
            return ElementType.SLIDER;
        }
        else if (targetTransform.GetComponent<ScrollRect>() != null)
        {
            return ElementType.SCROLL_RECT;
        }
        else if (targetTransform.GetComponent<Dropdown>() != null)
        {
            return ElementType.DROPDOWN;
        }
        else if (targetTransform.GetComponent<Toggle>() != null)
        {
            return ElementType.TOGGLE;
        }
        else if (targetTransform.GetComponent<UICustomReactiveElement>() != null)
        {
            return ElementType.CUSTOM_ELEMENT;
        }
        return ElementType.UNKNOWN;
    }
}
