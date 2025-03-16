using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelsUI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel, calibrationPanel, assetsPanel;
    [SerializeField] private CalibrationUI calibrationUI;
    public void OpenCalibration()
    {
        foreach (GameObject panel in new GameObject[] { assetsPanel, mainMenuPanel }) { panel.SetActive(false); }
        calibrationPanel.SetActive(true);
        calibrationUI.PanelActivate();
    }
    public void OpenAssets()
    {
        foreach (GameObject panel in new GameObject[] { calibrationPanel, mainMenuPanel }) { panel.SetActive(false); }
        assetsPanel.SetActive(true);
    }
    public void BackToMainMenu()
    {
        foreach (GameObject panel in new GameObject[] { calibrationPanel, assetsPanel }) { panel.SetActive(false); }
        mainMenuPanel.SetActive(true);
    }
}
