using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum ElementType
{
    BUTTON, UNKNOWN
}

public class UIReactiveManager : MonoBehaviour
{
    public void PointerClick(GameObject gameObject)
    {
        switch (DefineUIElement(gameObject))
        {
            case ElementType.BUTTON:
                Debug.Log("Clicked on button!");
                var button = gameObject.GetComponent<Button>();
                if (button.onClick != null && button.IsActive() && button.IsInteractable())
                {
                    button.onClick.Invoke();
                }
                break;
            default:
                break;
        }
    }
    public void PointerIn(GameObject gameObject)
    {
        switch (DefineUIElement(gameObject))
        {
            case ElementType.BUTTON:
                Debug.Log("Entered button collider!");
                break;
            default:
                break;
        }
    }
    public void PointerOut(GameObject gameObject)
    {
        switch (DefineUIElement(gameObject))
        {
            case ElementType.BUTTON:
                Debug.Log("Out of button!");
                break;
            default:
                break;
        }
    }
    private ElementType DefineUIElement(GameObject gameObject)
    {
        if (gameObject.GetComponent<Button>() != null)
        {
            return ElementType.BUTTON;
        }
        return ElementType.UNKNOWN;
    }
}
