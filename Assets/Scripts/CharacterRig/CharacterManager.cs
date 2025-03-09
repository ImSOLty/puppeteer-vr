using System.Collections.Generic;
using UniGLTF;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class VRCharacterInfo
{
    public readonly string pathName;

    public VRCharacterInfo(string pathName)
    {
        this.pathName = pathName;
    }
}

public class CharacterManager : MonoBehaviour
{
    private TrackerManager trackerManager;
    private ImportManager importManager;
    [SerializeField] private GameObject VRIKRigPrefab;
    Dictionary<string, ActionCharacter> readyCharacters = new();
    private ActionCharacter currentCharacter = null;

    void Awake()
    {
        trackerManager = FindObjectOfType<TrackerManager>();
        importManager = FindObjectOfType<ImportManager>();
    }
    public ActionCharacter SetCharacterAsMain(VRCharacterInfo character)
    {
        if (currentCharacter != null)
        {
            currentCharacter.SetUsage(false);
        }

        if (!readyCharacters.ContainsKey(character.pathName))
        {
            CreateCharacter(character);
        }

        currentCharacter = readyCharacters[character.pathName];
        currentCharacter.SetUsage(true);
        return currentCharacter;
    }

    public void CreateCharacter(VRCharacterInfo character)
    {
        ActionCharacter actionCharacter = LoadCharacterByPathName(character.pathName);
        actionCharacter.SetUsage(false);
        readyCharacters.Add(character.pathName, actionCharacter);
    }

    private ActionCharacter LoadCharacterByPathName(string characterPath)
    {
        RuntimeGltfInstance vrmCharacter = importManager.LoadVRMByPathName(characterPath);
        vrmCharacter.ShowMeshes();

        GameObject characterGameObject = vrmCharacter.gameObject;
        GameObject characterRig = Instantiate(VRIKRigPrefab, characterGameObject.transform);
        Rig rig = characterRig.AddComponent<Rig>();

        RigHelperSetup helperSetup = characterRig.GetComponent<RigHelperSetup>();
        helperSetup.provideSources(vrm: characterGameObject.transform, trackerManager);
        helperSetup.Setup();

        RigBuilder rigBuilder = characterGameObject.AddComponent<RigBuilder>();
        rigBuilder.layers.Clear();
        rigBuilder.layers.Add(new RigLayer(rig, active: true));
        rigBuilder.Build();

        return characterGameObject.AddComponent<ActionCharacter>();
    }

    public ActionCharacter GetCurrentCharacter()
    {
        return currentCharacter;
    }
}
