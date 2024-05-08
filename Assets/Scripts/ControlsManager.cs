using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControlsManager : MonoBehaviour
{
    private ActionBasedController _leftController, _rightController;
    [SerializeField] private GameObject testObject;

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
    }

    void Update()
    {
        if (_leftController.activateAction.action.WasPressedThisFrame())
        {
            testObject.SetActive(!testObject.activeSelf);
        }
    }
}