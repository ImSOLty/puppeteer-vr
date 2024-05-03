using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSelection : MonoBehaviour
{
    private CameraInstance _instance;

    private void Start()
    {
        _instance = transform.parent.GetComponent<CameraInstance>();
    }

    private void OnMouseUpAsButton()
    {
        _instance.UpdateClosure();
    }
}