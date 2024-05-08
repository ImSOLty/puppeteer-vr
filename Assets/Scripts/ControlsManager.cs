using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ControlsManager : MonoBehaviour
{
    private ActionBasedController _leftController, _rightController;
    [SerializeField] private InputActionProperty leftPrimaryButtonClickAction;
    [SerializeField] private InputActionProperty rightPrimaryButtonClickAction;
    private UIManager _uiManager;

    private void Awake()
    {
        _uiManager = FindObjectOfType<UIManager>();
    }

    private void Start()
    {
        ActionBasedController[] controllers = FindObjectsOfType<ActionBasedController>();
        if (controllers[0].gameObject.name.ToLower().Contains("Left"))
        {
            _leftController = controllers[0];
            _rightController = controllers[1];
        }
        else
        {
            _leftController = controllers[1];
            _rightController = controllers[0];
        }
        SetEvents();
    }

    void SetEvents()
    {
        leftPrimaryButtonClickAction.action.performed += _uiManager.TimelineCanvasOpenClose;
    }

}