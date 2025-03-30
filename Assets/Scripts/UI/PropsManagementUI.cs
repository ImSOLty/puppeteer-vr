using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public enum PropTool
{
    CAMERA, LIGHT, EDIT, PROP
}

public enum JoystickDirection { LEFT, RIGHT, UP, DOWN, NONE }

public class PropsManagementUI : MonoBehaviour
{
    [SerializeField] private PropsManagement propsManagement;
    [SerializeField] private PropTool currentPropTool;
    [SerializeField] private RawImage propLight, propCamera, propLeft, propCenter, propRight, propEditing;
    [SerializeField] private Text propLeftText, propCenterText, propRightText;
    public SteamVR_Action_Vector2 wheelAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("JoystickPosition");
    public Vector2 wheelAxis;
    private List<AssetProperties> propsOptions;
    private int currentObjectPropIndex = 0;
    private RectTransform propLightRectTransform, propCameraRectTransform, propCenterRectTransform, propEditingRectTransform;
    private bool noneWasBetweenSwitches = true;
    [SerializeField] private Texture2D defaultTexture;

    void Awake()
    {
        propLightRectTransform = propLight.transform.parent.GetComponent<RectTransform>();
        propCameraRectTransform = propCamera.transform.parent.GetComponent<RectTransform>();
        propCenterRectTransform = propCenter.transform.parent.GetComponent<RectTransform>();
        propEditingRectTransform = propEditing.transform.parent.GetComponent<RectTransform>();
    }

    void Start()
    {
        propsOptions = AssetsManager.GetAssetsPropertiesByAssetType(AssetType.PROP);
        RedrawProps();
    }

    void Update()
    {
        if (propsManagement.inCreationProcess)
        {
            return; // Don't do anything while user manages created prop;
        }
        wheelAxis = wheelAction.GetAxis(SteamVR_Input_Sources.RightHand);
        JoystickDirection direction = PropsManagement.GetDirection(wheelAxis);
        if (noneWasBetweenSwitches && direction != JoystickDirection.NONE)
        {
            if (currentPropTool == PropTool.CAMERA && direction == JoystickDirection.LEFT)
            {
                currentPropTool = PropTool.LIGHT;
            }
            else if (currentPropTool == PropTool.LIGHT && direction == JoystickDirection.RIGHT)
            {
                currentPropTool = PropTool.CAMERA;
            }
            else if ((currentPropTool == PropTool.CAMERA || currentPropTool == PropTool.LIGHT) && direction == JoystickDirection.DOWN ||
                currentPropTool == PropTool.EDIT && direction == JoystickDirection.UP)
            {
                if (PropsExist())
                    currentPropTool = PropTool.PROP;
                else
                    currentPropTool = currentPropTool == PropTool.CAMERA ? PropTool.EDIT : PropTool.CAMERA;
            }
            else if (currentPropTool == PropTool.PROP)
            {
                if (direction == JoystickDirection.UP)
                {
                    currentPropTool = PropTool.CAMERA;
                }
                if (direction == JoystickDirection.DOWN)
                {
                    currentPropTool = PropTool.EDIT;
                }
                if (PropsExist() && (direction == JoystickDirection.RIGHT || direction == JoystickDirection.LEFT))
                {
                    currentObjectPropIndex += direction == JoystickDirection.RIGHT ? 1 : -1;
                    currentObjectPropIndex = (currentObjectPropIndex + propsOptions.Count) % propsOptions.Count;
                }
            }
            RedrawProps();
            SetPropsManagement();
        }
        noneWasBetweenSwitches = direction == JoystickDirection.NONE;
    }

    private void RedrawProps()
    {
        foreach ((PropTool, RectTransform) option in new[] {
             (PropTool.CAMERA, propCameraRectTransform),
             (PropTool.PROP, propCenterRectTransform),
             (PropTool.EDIT, propEditingRectTransform),
             (PropTool.LIGHT, propLightRectTransform)
        })
        {
            option.Item2.localScale = Vector3.one * ((currentPropTool == option.Item1) ? 1.4f : 1); // Rescale to demonstrate selection
        }

        if (propsOptions.Count == 0)
        {
            return;
        }
        int leftIndex = (currentObjectPropIndex - 1 + propsOptions.Count) % propsOptions.Count;
        int rightIndex = (currentObjectPropIndex + 1) % propsOptions.Count;

        foreach ((RawImage, Text, int) prop in new[] {
            (propLeft, propLeftText, leftIndex),
            (propCenter, propCenterText, currentObjectPropIndex),
            (propRight, propRightText, rightIndex)
        })
        {
            AssetProperties propOption = propsOptions[prop.Item3]; // Get by index
            if (propOption.PreviewExists())
            {
                prop.Item1.texture = propOption.GetPreviewTexture();
                prop.Item2.text = "";
            }
            else
            {
                prop.Item1.texture = defaultTexture;
                prop.Item2.text = propOption.name;
            }
        }
    }

    private void SetPropsManagement()
    {
        propsManagement.SetPropAssetProperties(PropsExist() ? propsOptions[currentObjectPropIndex] : null);
        propsManagement.SetPropTool(currentPropTool);
    }

    private bool PropsExist()
    {
        return propsOptions.Count > 0;
    }
}
