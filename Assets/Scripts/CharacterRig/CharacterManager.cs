using System.Collections.Generic;
using UniGLTF;
using UnityEngine;
using UnityEngine.Animations.Rigging;

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
    public ActionCharacter SetCharacterAsMain(string pathName)
    {
        return SetCharacterAsMain(new VRObjectInfo(pathName));
    }
    public ActionCharacter SetCharacterAsMain(VRObjectInfo character)
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

    public GameObject CreateCharacter(VRObjectInfo character)
    {
        ActionCharacter actionCharacter = LoadCharacterByPathName(character.pathName);
        actionCharacter.SetUsage(false);
        readyCharacters.Add(character.pathName, actionCharacter);
        return actionCharacter.gameObject;
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
    public ActionCharacter GetActionCharacterByInfo(VRObjectInfo character)
    {
        if (readyCharacters.ContainsKey(character.pathName))
        {
            return readyCharacters[character.pathName];
        }
        return null;
    }
    public Dictionary<string, ActionCharacter> GetActionCharacters()
    {
        return readyCharacters;
    }
}
