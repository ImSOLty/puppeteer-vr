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
    [SerializeField] [Range(-1f, 1f)] private float colorModifier = -0.25f;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _defaultColor = _image.color;
    }

    public void Enable()
    {
        ChangeColor(1 + colorModifier);
        _highlighted = true;
    }

    public void Disable()
    {
        ChangeColor(1);
        _highlighted = false;
    }

    private void ChangeColor(float change)
    {
        if (!_highlighted)
            _defaultColor = _image.color;
        Color newColor = new Color(
            Math.Clamp(_defaultColor.r * change, 0f, 255f),
            Math.Clamp(_defaultColor.g * change, 0f, 255f),
            Math.Clamp(_defaultColor.b * change, 0f, 255f)
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