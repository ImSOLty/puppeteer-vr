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
            uiManager.PointerClick(e.target.gameObject);
        }
    }
    public override void OnPointerIn(PointerEventArgs e)
    {
        base.OnPointerIn(e);
        if (((1 << e.target.gameObject.layer) & UILayer) != 0)
        {
            uiManager.PointerIn(e.target.gameObject);
        }
    }
    public override void OnPointerOut(PointerEventArgs e)
    {
        base.OnPointerOut(e);
        if (((1 << e.target.gameObject.layer) & UILayer) != 0)
        {
            uiManager.PointerOut(e.target.gameObject);
        }
    }
}
