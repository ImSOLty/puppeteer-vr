using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioSelector : MonoBehaviour
{
    [SerializeField] private Color selectedOptionColor, nonSelectedOptionColor;
    private List<Button> _options;
    [HideInInspector] public Button selectedOption;

    void Start()
    {
        selectedOption = null;
        _options = new List<Button>();
        foreach (Button b in GetComponentsInChildren<Button>())
        {
            b.targetGraphic.color = nonSelectedOptionColor;
            _options.Add(b);
            b.onClick.AddListener(() => UpdateButtons(b));
        }
    }

    void UpdateButtons(Button selected)
    {
        selectedOption = selected;
        _options.ForEach(button =>
            button.targetGraphic.color = button == selectedOption ? selectedOptionColor : nonSelectedOptionColor);
    }
}