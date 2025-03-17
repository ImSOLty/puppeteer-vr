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

    public void DetachCharacter()
    {
        if (currentCharacter != null)
        {
            currentCharacter.SetUsage(false);
        }
        currentCharacter = null;
    }
    public ActionCharacter SetCharacterAsMain(string uuid)
    {
        return SetCharacterAsMain(AssetsManager.GetAssetPropertiesByAssetTypeAndUUID(AssetType.CHARACTER, uuid));
    }
    public ActionCharacter SetCharacterAsMain(AssetProperties character)
    {
        if (currentCharacter != null)
        {
            currentCharacter.SetUsage(false);
        }

        if (!readyCharacters.ContainsKey(character.assetUuid))
        {
            CreateCharacter(character);
        }

        currentCharacter = readyCharacters[character.assetUuid];
        currentCharacter.SetUsage(true);
        return currentCharacter;
    }

    public GameObject CreateCharacter(AssetProperties character)
    {
        ActionCharacter actionCharacter = LoadCharacterByPathName(character.fileReference);
        actionCharacter.SetAssetProperties(character);
        actionCharacter.SetUsage(false);
        readyCharacters.Add(character.assetUuid, actionCharacter);
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
    public ActionCharacter GetActionCharacterByInfo(AssetProperties character)
    {
        if (readyCharacters.ContainsKey(character.assetUuid))
        {
            return readyCharacters[character.assetUuid];
        }
        return null;
    }
    public Dictionary<string, ActionCharacter> GetActionCharacters()
    {
        return readyCharacters;
    }
}
