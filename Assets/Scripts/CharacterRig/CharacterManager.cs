using System.Collections.Generic;
using UniGLTF;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private TrackerManager trackerManager;
    [SerializeField] private ImportManager importManager;
    [SerializeField] private GameObject CharacterHelpers;
    Dictionary<string, ActionCharacter> readyCharacters = new();
    [SerializeField] private float referenceModelScale; // only height
    private ActionCharacter currentCharacter = null;

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
        if (!trackerManager.trackersDefined)
        {
            trackerManager.DefineTrackers();
        }
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

        //define the highest and lowest point in character meshes, to define its scale
        float highest = 0f, lowest = 0f;
        foreach (Mesh mesh in vrmCharacter.Meshes)
        {
            highest = Mathf.Max(highest, mesh.bounds.max.y);
            lowest = Mathf.Max(lowest, mesh.bounds.min.y);
        }
        Debug.Log(highest - lowest);
        float scaleAccordingToReference = (highest - lowest) / referenceModelScale;
        Debug.Log(scaleAccordingToReference);

        GameObject characterGameObject = vrmCharacter.gameObject;
        GameObject characterHelpers = Instantiate(CharacterHelpers, characterGameObject.transform);
        Rig rig = characterHelpers.AddComponent<Rig>();

        RigHelperSetup helperSetup = characterHelpers.GetComponent<RigHelperSetup>();
        helperSetup.provideSources(vrm: characterGameObject.transform, trackerManager);
        helperSetup.Setup();

        RigBuilder rigBuilder = characterGameObject.AddComponent<RigBuilder>();
        rigBuilder.layers.Clear();
        rigBuilder.layers.Add(new RigLayer(rig, active: true));
        rigBuilder.Build();

        ActionCharacter actionCharacter = characterGameObject.AddComponent<ActionCharacter>();
        actionCharacter.neededScaleForThisModel = scaleAccordingToReference;

        return actionCharacter;
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
