using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

enum ElementType
{
    SLIDER, BUTTON, SCROLL_RECT, DROPDOWN, TOGGLE, CUSTOM_ELEMENT, UNKNOWN, INPUT_FIELD
}

public class UIReactiveManager : MonoBehaviour
{
    public SteamVR_Action_Boolean systemAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("System");
    [SerializeField] private float scrollingSpeed = 0.1f;
    [SerializeField] private LayerMask UILayer;
    [SerializeField] private float userUIDistanceFromUser;
    [SerializeField] private float userUIDistanceFromFloor;
    private Transform userHead;
    [SerializeField] private GameObject keyboardPrefab;
    private Transform userUITransform;
    private VKB_Keyboard keyboard;

    void Start()
    {
        userHead = FindObjectOfType<Player>().GetComponentInChildren<Camera>().transform;
        systemAction.onStateDown += delegate { UpdateUserUITransform(); };

        // Setting up User UI (near)
        userUITransform = new GameObject("UserUI").transform;
        keyboard = Instantiate(keyboardPrefab, userUITransform).GetComponent<VKB_Keyboard>();
        HideUserKeyboard();
    }

    void UpdateUserUITransform()
    {
        Vector3 way = userHead.forward;
        way.y = 0;
        Vector3 place = way * userUIDistanceFromUser;
        place.y = userUIDistanceFromFloor;
        userUITransform.position = place;
        userUITransform.LookAt(userHead);
    }

    void ShowUserKeyboard() { if (keyboard != null) keyboard.gameObject.SetActive(true); }
    void HideUserKeyboard() { if (keyboard != null) keyboard.gameObject.SetActive(false); }

    public void PointerClick(PointerEventArgs e)
    {
        // Close keyboard if exists on any click besides keyboard buttons
        if (e.target.GetComponent<VKB_Key>() == null) { HideUserKeyboard(); }

        switch (DefineUIElement(e.target))
        {
            case ElementType.BUTTON:
                var button = e.target.GetComponent<Button>();
                if (button.onClick != null && button.IsActive() && button.IsInteractable())
                {
                    button.onClick.Invoke();
                }
                break;
            case ElementType.DROPDOWN:
                var dropdown = e.target.GetComponent<Dropdown>();
                dropdown.Show();
                break;
            case ElementType.TOGGLE:
                var toggle = e.target.GetComponent<Toggle>();
                toggle.Select();
                break;
            case ElementType.INPUT_FIELD:
                var inputField = e.target.GetComponent<InputField>();

                inputField.GetType().GetField("m_AllowInput", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(inputField, true);
                inputField.GetType().InvokeMember("SetCaretVisible", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance, null, inputField, null);

                ShowUserKeyboard();
                keyboard.SetupKeyboard(inputField);
                UpdateUserUITransform();
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
        else if (targetTransform.GetComponent<InputField>() != null)
        {
            return ElementType.INPUT_FIELD;
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
