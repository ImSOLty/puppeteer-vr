using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Valve.VR;
using VRM;

public class ObjectsUI : MonoBehaviour
{
    public SteamVR_Action_Vector2 wheelAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("ChooseWheel");
    public Vector2 wheelAxis;
    public List<Tuple<string, Texture2D>> options = new();

    public CharacterManager characterManager;
    int chosenOption = -1;


    void Awake()
    {
        characterManager = FindObjectOfType<CharacterManager>();
    }
    void Start()
    {
        foreach (KeyValuePair<string, ActionCharacter> entry in characterManager.GetActionCharacters())
        {
            options.Add(new Tuple<string, Texture2D>(entry.Key, entry.Value.GetComponent<VRMMeta>().Meta.Thumbnail));
        }
    }
    void Update()
    {
        wheelAxis = wheelAction.GetAxis(SteamVR_Input_Sources.Any);
        if (wheelAxis != Vector2.zero)
        {
            float angle = (Mathf.Atan2(wheelAxis.x, wheelAxis.y) + Mathf.PI) * Mathf.Rad2Deg;
            chosenOption = Mathf.FloorToInt(angle / (Mathf.PI * Mathf.Rad2Deg * 2 / options.Count)); //chosen angle div (360/number)
            characterManager.SetCharacterAsMain(options[chosenOption].Item1); // pathname
        }
    }
}
