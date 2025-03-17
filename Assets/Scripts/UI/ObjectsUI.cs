using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Valve.VR;
using VRM;
using Unity.VisualScripting;
using UnityEngine.Animations;

public class ObjectsUI : MonoBehaviour
{
    public SteamVR_Behaviour_Pose handPose;
    public SteamVR_Action_Vector2 wheelAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("JoystickPosition");
    public Vector2 wheelAxis;
    private List<Tuple<string, RawImage>> options = new();

    private CharacterManager characterManager;
    private int chosenOption = -1;
    [SerializeField] private GameObject panelWithOptions;

    [SerializeField] private GameObject optionImagePrefab;

    [SerializeField] private Color HighlightColor, DefaultColor;
    [SerializeField] private Texture2D emptyTexture;


    void Awake()
    {
        characterManager = FindObjectOfType<CharacterManager>();

        handPose = FindObjectOfType<LaserInteractor>().GetComponent<SteamVR_Behaviour_Pose>();
        //Setup Constraint
        ParentConstraint constraint = GetComponent<ParentConstraint>();
        constraint.AddSource(new ConstraintSource()
        {
            weight = 1.0f,
            sourceTransform = handPose.transform,
        });
        constraint.locked = true;
    }


    void Start()
    {
        foreach (Transform child in panelWithOptions.transform) { Destroy(child.gameObject); } // Clear UI

        GameObject emptyObject = Instantiate(optionImagePrefab, panelWithOptions.transform);
        RawImage image = emptyObject.GetComponent<RawImage>();
        image.texture = emptyTexture;
        options.Add(new Tuple<string, RawImage>(null, image));

        foreach (KeyValuePair<string, ActionCharacter> entry in characterManager.GetActionCharacters())
        {
            GameObject imageObject = Instantiate(optionImagePrefab, panelWithOptions.transform);

            image = imageObject.GetComponent<RawImage>();
            image.texture = entry.Value.GetComponent<VRMMeta>().Meta.Thumbnail;

            options.Add(new Tuple<string, RawImage>(entry.Key, image));
        }

        //Place images

        float angleStep = 360f / options.Count; // Calculate the angle step between each object

        for (int i = 0; i < options.Count; i++)
        {
            float angle = 360 - i * angleStep; // Calculate the angle for the current object
            Tuple<string, RawImage> option = options[i];
            float radianAngle = angle * Mathf.Deg2Rad;

            // Calculate the position of the object
            float radius = 0.5f;
            Vector2 minMaxAnchor = new Vector2(
                0.5f + radius * Mathf.Cos(radianAngle),
                0.5f + radius * Mathf.Sin(radianAngle)
            );
            RectTransform rectTransform = option.Item2.GetComponent<RectTransform>();
            rectTransform.anchorMin = minMaxAnchor;
            rectTransform.anchorMax = minMaxAnchor;
        }

    }
    void Update()
    {
        wheelAxis = wheelAction.GetAxis(handPose.inputSource);
        if (wheelAxis != Vector2.zero)
        {
            if (!panelWithOptions.activeSelf)
                panelWithOptions.SetActive(true);


            float angle = Mathf.Atan2(wheelAxis.x, wheelAxis.y) * Mathf.Rad2Deg;
            if (angle < 0)
            {
                angle += 2 * Mathf.PI * Mathf.Rad2Deg;
            }
            int newChosenOption = Mathf.FloorToInt(angle / (360 / options.Count)); //chosen angle div (360/number)

            //Highlighting
            if (newChosenOption != chosenOption)
            {
                if (chosenOption != -1)
                {
                    options[chosenOption].Item2.color = Settings.Colors.defaultColor;
                }
                options[newChosenOption].Item2.color = Settings.Colors.selectionGrayscale;
            }
            chosenOption = newChosenOption;

            if (options[chosenOption].Item1 == null)
            {
                characterManager.DetachCharacter();
            }
            else
            {
                characterManager.SetCharacterAsMain(options[chosenOption].Item1); // pathname
            }
        }
        else
        {
            if (panelWithOptions.activeSelf)
                panelWithOptions.SetActive(false);
        }
    }
}
