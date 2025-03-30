using System;
using System.IO;
using Klak.Spout;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LightInstanceUI : MonoBehaviour
{
    [SerializeField] private LightInstance lightInstance;
    [SerializeField] private Image indicator;
    [SerializeField] private Slider red, green, blue, intensity, range;
    [SerializeField] private GameObject propertiesWindow, sphere, openMeButton;

    void Start()
    {
        if (Settings.Animation.AnimationMode != Mode.PROPS_MANAGEMENT)
        {
            foreach (GameObject gameObject in new[] { propertiesWindow, sphere, openMeButton })
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void UpdateLight()
    {
        Color color = new(red.value, green.value, blue.value);
        indicator.color = color;
        lightInstance.UpdateLight(color, intensity.value, range.value);
    }

    public void CloseProperties()
    {
        sphere.SetActive(true);
        openMeButton.SetActive(true);
        propertiesWindow.SetActive(false);
    }
    public void OpenProperties()
    {
        sphere.SetActive(false);
        openMeButton.SetActive(false);
        propertiesWindow.SetActive(true);
    }
}
