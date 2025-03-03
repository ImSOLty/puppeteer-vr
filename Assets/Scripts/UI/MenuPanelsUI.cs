using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelsUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel, calibrationPanel, locationsPanel;
    [SerializeField] private CalibrationUI calibrationUI;
    public void OpenCalibration()
    {
        mainMenuPanel.SetActive(false);
        calibrationPanel.SetActive(true);
        calibrationUI.PanelActivate();
    }
    public void OpenLocations()
    {
        mainMenuPanel.SetActive(false);
        locationsPanel.SetActive(true);
    }
    public void BackToMainMenu()
    {
        calibrationPanel.SetActive(false);
        locationsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
