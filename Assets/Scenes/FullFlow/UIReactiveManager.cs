using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;

enum ElementType
{
    SLIDER, BUTTON, UNKNOWN
}

public class UIReactiveManager : MonoBehaviour
{
    [SerializeField] private LayerMask UILayer;
    public void PointerClick(PointerEventArgs e)
    {
        switch (DefineUIElement(e.target))
        {
            case ElementType.BUTTON:
                Debug.Log("Clicked on button!");
                var button = e.target.GetComponent<Button>();
                if (button.onClick != null && button.IsActive() && button.IsInteractable())
                {
                    button.onClick.Invoke();
                }
                break;

            case ElementType.SLIDER:
                var slider = e.target.GetComponent<Slider>();
                float percentX = (e.hit.point.x - e.hit.collider.bounds.min.x) / e.hit.collider.bounds.size.x;
                slider.value = slider.minValue + (slider.maxValue - slider.minValue) * percentX;
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
                Debug.Log("Entered button collider!");
                break;

            case ElementType.SLIDER:
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
                Debug.Log("Out of button!");
                break;
            case ElementType.SLIDER:
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
        return ElementType.UNKNOWN;
    }
}
