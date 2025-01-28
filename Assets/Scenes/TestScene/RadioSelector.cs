using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioSelector : MonoBehaviour
{
    [SerializeField] private Color selectedOptionColor, nonSelectedOptionColor;
    private List<(Button, Image)> _options;
    [HideInInspector] public Button selectedOption;

    void Start()
    {
        selectedOption = null;
        _options = new List<(Button, Image)>();
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            Image img = b.GetComponent<Image>();
            img.color = nonSelectedOptionColor;
            _options.Add((b, img));
            b.onClick.AddListener(() => UpdateButtons(b));
        }
    }

    void UpdateButtons(Button selected)
    {
        selectedOption = selected;
        _options.ForEach(button =>
            button.Item2.color = button.Item1 == selectedOption ? selectedOptionColor : nonSelectedOptionColor);
    }
}