using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = FindObjectOfType<Canvas>();
    }

    void Show()
    {
        _canvas.enabled = true;
    }

    void Close()
    {
        _canvas.enabled = false;
    }
}