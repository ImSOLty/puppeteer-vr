using System.Collections;
using System.Collections.Generic;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

public class FileBrowserItemElement : UICustomReactiveElement
{
    [SerializeField] private FileBrowserItem browserItem;
    private EventSystem eventSystem;

    void Awake()
    {
        eventSystem = FindObjectOfType<InputModule>().GetComponent<EventSystem>();
    }
    public override void OnPointerClick(PointerEventArgs eventData)
    {
        PointerEventData pointerEventData = ConstructPointerEventData();
        pointerEventData.button = PointerEventData.InputButton.Left;

        browserItem.OnPointerClick(pointerEventData);
    }
    public override void OnPointerIn(PointerEventArgs eventData)
    {
        browserItem.OnPointerEnter(ConstructPointerEventData());
    }
    public override void OnPointerOut(PointerEventArgs eventData)
    {
        browserItem.OnPointerExit(ConstructPointerEventData());
    }

    private PointerEventData ConstructPointerEventData()
    {
        return new PointerEventData(eventSystem);
    }
}
