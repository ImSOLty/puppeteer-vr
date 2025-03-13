using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;

public class LaserInteractor : SteamVR_LaserPointer
{
    [SerializeField] private LayerMask UILayer;
    private UIReactiveManager uiManager;

    void Awake()
    {
        uiManager = FindObjectOfType<UIReactiveManager>();
    }

    public override void OnPointerClick(PointerEventArgs e)
    {
        base.OnPointerClick(e);
        if (((1 << e.target.gameObject.layer) & UILayer) != 0)
        {
            uiManager.PointerClick(e);
        }
    }
    public override void OnPointerIn(PointerEventArgs e)
    {
        base.OnPointerIn(e);
        if (((1 << e.target.gameObject.layer) & UILayer) != 0)
        {
            uiManager.PointerIn(e);
        }
    }
    public override void OnPointerOut(PointerEventArgs e)
    {
        base.OnPointerOut(e);
        if (((1 << e.target.gameObject.layer) & UILayer) != 0)
        {
            uiManager.PointerOut(e);
        }
    }
    public override void OnPointerHold(PointerEventArgs e)
    {
        base.OnPointerHold(e);
        if (((1 << e.target.gameObject.layer) & UILayer) != 0)
        {
            uiManager.PointerHold(e);
        }
    }
    public override void OnPointerRelease(PointerEventArgs e)
    {
        base.OnPointerRelease(e);
        if (e.target == null)
        {
            return;
        }
        if (((1 << e.target.gameObject.layer) & UILayer) != 0)
        {
            uiManager.PointerRelease(e);
        }
    }
}
