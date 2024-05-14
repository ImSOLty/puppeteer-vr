using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO For now it will just change the color of the elements' sprite a bit
public class UIHighlighter : MonoBehaviour
{
    private bool _highlighted = false;
    private Color _defaultColor;
    [SerializeField] [Range(0f, 1f)] private float colorModifier = 0.25f;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
    }

    public void Enable()
    {
        _highlighted = true;
        ChangeColor(1 - colorModifier);
    }

    public void Disable()
    {
        _highlighted = false;
        ChangeColor(1 + colorModifier);
    }

    private void ChangeColor(float change)
    {
        Color newColor = new Color(
            Math.Clamp(_defaultColor.r * change, 0f, 255f),
            Math.Clamp(_defaultColor.r * change, 0f, 255f),
            Math.Clamp(_defaultColor.r * change, 0f, 255f)
        );
        _image.color = newColor;
    }


    public void SetHighlight(bool on)
    {
        if (on)
            Enable();
        else
            Disable();
    }
}