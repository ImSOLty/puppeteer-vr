using System.Collections;
using System.Collections.Generic;
using UniGLTF;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Valve.VR;
public class VRCharacterInfo
{
    public string pathName;

    public VRCharacterInfo(string pathName)
    {
        this.pathName = pathName;
    }
}

public class CharacterManager : MonoBehaviour
{
    private TrackerManager trackerManager;
    [SerializeField] private GameObject VRIKRig;
    GameObject currentCharacter = null;

    void Awake()
    {
        trackerManager = FindObjectOfType<TrackerManager>();
    }
    public void CreateCharacterAndSetAsMain(VRCharacterInfo character)
    {
        RemoveCurrentCharacter();
        currentCharacter = CreateCharacter(character);
    }
    private void RemoveCurrentCharacter()
    {
        if (currentCharacter == null)
        {
            return;
        }
        GameObject.Destroy(currentCharacter);
    }

    private GameObject CreateCharacter(VRCharacterInfo character)
    {
        return LoadCharacterByPathName(character.pathName);
    }

    private GameObject LoadCharacterByPathName(string characterPath)
    {
        RuntimeGltfInstance vrmCharacter = FindObjectOfType<ImportManager>().LoadVRMByPathName(characterPath);
        vrmCharacter.ShowMeshes();

        GameObject characterGameObject = vrmCharacter.gameObject;
        GameObject characterRig = Instantiate(VRIKRig, characterGameObject.transform);
        Rig rig = characterRig.AddComponent<Rig>();

        RigHelperSetup helperSetup = characterRig.GetComponent<RigHelperSetup>();
        helperSetup.provideSources(vrm: characterGameObject.transform, trackerManager);
        helperSetup.Setup();

        RigBuilder rigBuilder = characterGameObject.AddComponent<RigBuilder>();
        rigBuilder.layers.Clear();
        rigBuilder.layers.Add(new RigLayer(rig, active: true));
        rigBuilder.Build();
        return vrmCharacter.gameObject;
    }
}
