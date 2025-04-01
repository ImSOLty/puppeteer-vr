using UniGLTF;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PropsManagement : MonoBehaviour
{
    private SteamVR_Behaviour_Pose handPose;
    [SerializeField] private LaserInteractor laserInteractor;
    [SerializeField] private LayerMask propsLayerMask;
    public SteamVR_Action_Vector2 joystickAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("JoystickPosition");
    public SteamVR_Action_Boolean grabAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
    public SteamVR_Action_Boolean yPressAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("YPress");
    public SteamVR_Action_Boolean xPressAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("XPress");
    [SerializeField] AppSceneCreationManager appSceneCreationManager;
    [SerializeField] ObjectManager objectManager;
    [SerializeField] private GameObject cameraPropPrefab, lightPropPrefab;
    [SerializeField] private float propTranslationSpeed = 2;
    private GameObject currentProp = null;
    private ParentConstraint currentPropConstraint = null;
    private bool propMoved = false;
    private PropTool propTool;
    private AssetProperties propAssetProperties;
    [HideInInspector] public bool inCreationProcess;

    void Awake()
    {
        laserInteractor = FindObjectOfType<LaserInteractor>();
        handPose = laserInteractor.GetComponent<SteamVR_Behaviour_Pose>();

        appSceneCreationManager = FindObjectOfType<AppSceneCreationManager>();

        //Setup Constraint
        ParentConstraint constraint = GetComponent<ParentConstraint>();
        constraint.AddSource(new ConstraintSource()
        {
            weight = 1.0f,
            sourceTransform = handPose.transform,
        });
        constraint.locked = true;
    }

    public void Update()
    {
        if (grabAction.GetStateDown(handPose.inputSource))
        {
            inCreationProcess = true;
            if (currentProp == null) { AcquireCurrentProp(); }
            if (currentProp == null) { inCreationProcess = false; return; }

            //Disable objectProps components if any
            ActionObject actionObject = currentProp.GetComponent<ActionObject>();
            if (actionObject) { actionObject.SetRigidbodyActive(false); actionObject.SetInteractable(false); }
        }

        JoystickDirection direction = GetDirection(joystickAction.GetAxis(handPose.inputSource));
        if (inCreationProcess && (direction == JoystickDirection.UP || direction == JoystickDirection.DOWN))
        {
            Vector3 translationWay = direction == JoystickDirection.UP ? Vector3.forward : Vector3.back;
            Vector3 newTranslationOffset = currentPropConstraint.translationOffsets[0] + translationWay * propTranslationSpeed * Time.deltaTime;
            currentPropConstraint.translationOffsets = new[] { newTranslationOffset };
            propMoved = true;
        }

        if (grabAction.GetStateUp(handPose.inputSource))
        {
            inCreationProcess = false;
            if (currentProp != null)
            {
                //Enable objectProps components if any
                ActionObject actionObject = currentProp.GetComponent<ActionObject>();
                if (actionObject) { actionObject.SetRigidbodyActive(true); actionObject.SetInteractable(true); }

                // Release prop
                currentPropConstraint.constraintActive = false;
                if (!propMoved) { Destroy(currentProp); }
                currentProp = null;
            }
        }

        if (!inCreationProcess && xPressAction.GetStateDown(handPose.inputSource))
        {
            appSceneCreationManager.SetupPropDatas();
            appSceneCreationManager.Save();
        }
        if (!inCreationProcess && yPressAction.GetStateDown(handPose.inputSource))
        {
            Settings.Hints.currentHintAbout = HintAbout.MAIN_MENU;
            SceneManager.LoadScene(Settings.Scenes.MainMenuSceneName);
        }
    }

    public void AcquireCurrentProp()
    {
        if (propTool == PropTool.CAMERA)
        {
            currentProp = Instantiate(cameraPropPrefab);
            propMoved = false;
        }
        else if (propTool == PropTool.LIGHT)
        {
            currentProp = Instantiate(lightPropPrefab);
            propMoved = false;
        }
        else if (propTool == PropTool.PROP)
        {
            if (propAssetProperties == null) { return; }
            currentProp = objectManager.CreateObject(propAssetProperties);
            propMoved = false;
        }
        else if (propTool == PropTool.EDIT)
        {
            Transform acquired = laserInteractor.GetObjectByLaserAndMask(propsLayerMask);
            if (acquired == null) { return; }
            currentProp = acquired.gameObject;
            //temporary check on camera and lights
            if (currentProp.layer == LayerMask.NameToLayer("CameraScreen"))
            {
                currentProp.transform.SetPositionAndRotation(cameraPropPrefab.transform.position, cameraPropPrefab.transform.rotation);
            }
            if (currentProp.layer == LayerMask.NameToLayer("LightProp"))
            {
                currentProp.transform.SetPositionAndRotation(lightPropPrefab.transform.position, lightPropPrefab.transform.rotation);
            }
        }

        currentPropConstraint = currentProp.GetOrAddComponent<ParentConstraint>();
        if (currentPropConstraint.sourceCount == 0)
        {
            currentPropConstraint.AddSource(new ConstraintSource()
            {
                weight = 1.0f,
                sourceTransform = handPose.transform,
            });
            currentPropConstraint.locked = true;
            currentPropConstraint.rotationOffsets = new[] { currentProp.transform.rotation.eulerAngles };
            currentPropConstraint.translationOffsets = new[] { Vector3.zero };
        }
        currentPropConstraint.constraintActive = true;
    }

    public void SetPropAssetProperties(AssetProperties assetProperties) { propAssetProperties = assetProperties; }
    public void SetPropTool(PropTool propTool) { this.propTool = propTool; }

    public static JoystickDirection GetDirection(Vector2 stickInput)
    {
        // Determine direction based on x and y values
        if (stickInput == Vector2.zero)
        {
            return JoystickDirection.NONE;
        }
        if (Mathf.Abs(stickInput.x) > Mathf.Abs(stickInput.y))
        {
            return stickInput.x > 0 ? JoystickDirection.RIGHT : JoystickDirection.LEFT;
        }
        else
        {
            return stickInput.y > 0 ? JoystickDirection.UP : JoystickDirection.DOWN;
        }
    }
}
