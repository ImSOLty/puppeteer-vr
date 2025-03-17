using System;
using System.Collections.Generic;
using UnityEngine;

public class PropData
{
    public string propUuid;
    public Vector3 position;
    public Vector3 rotation;
}

[Serializable]
public class CameraPropData : PropData
{
    public float FOV;
    public string spoutName;

    public CameraPropData(Vector3 position, Vector3 rotation, float FOV, string spoutName)
    {
        propUuid = Guid.NewGuid().ToString();
        this.position = position;
        this.rotation = rotation;
        this.FOV = FOV;
        this.spoutName = spoutName;
    }
}

[Serializable]
public class ObjectPropData : PropData
{
    public string assetPropertiesUuid;

    public ObjectPropData(Vector3 position, Vector3 rotation, string assetPropertiesUuid)
    {
        propUuid = Guid.NewGuid().ToString();
        this.position = position;
        this.rotation = rotation;
        this.assetPropertiesUuid = assetPropertiesUuid;
    }
}


[Serializable]
public class SceneProperties : ISerializationCallbackReceiver
{
    public string sceneUuid;
    public string name;
    public string locationUuid;
    private AssetProperties locationAssetProperties = null;
    public List<string> characterUuids = new();
    private List<AssetProperties> characterAssetsProperties = new();
    public List<CameraPropData> cameraPropDatas = new();
    public List<ObjectPropData> objectPropDatas = new();

    public void OnBeforeSerialize()
    {
        characterUuids.Clear();
        foreach (AssetProperties assetProperties in characterAssetsProperties)
        {
            characterUuids.Add(assetProperties.assetUuid);
        }
        if (locationAssetProperties != null)
            locationUuid = locationAssetProperties.assetUuid;
    }

    public void OnAfterDeserialize()
    {
        characterAssetsProperties.Clear();
        foreach (string uuid in characterUuids)
        {
            characterAssetsProperties.Add(AssetsManager.GetAssetPropertiesByAssetTypeAndUUID(AssetType.CHARACTER, uuid));
        }
        locationAssetProperties = AssetsManager.GetAssetPropertiesByAssetTypeAndUUID(AssetType.LOCATION, locationUuid);
    }

    public void SetupCharacters(List<AssetProperties> characters)
    {
        characterAssetsProperties = characters;
    }
    public void SetupLocation(AssetProperties location)
    {
        locationAssetProperties = location;
    }
    public AssetProperties GetLocationAssetProperties() { return locationAssetProperties; }
    public List<AssetProperties> GetCharacterAssetsProperties() { return characterAssetsProperties; }
    public List<CameraPropData> GetCameraPropDatas() { return cameraPropDatas; }
    public List<ObjectPropData> GetObjectPropDatas() { return objectPropDatas; }
}
[Serializable]
class ScenesProperties
{
    public List<SceneProperties> scenes = new();

    public void CreateNewScene(SceneProperties newScene) { scenes.Add(newScene); }
    public void DeleteScene(SceneProperties scene) { scenes.Remove(scene); }
}

public class AppScenesManager : MonoBehaviour
{
    private static ScenesProperties scenesProperties = null;

    void Awake()
    {
        ParseScenesProperties();
    }

    private void ParseScenesProperties()
    {
        if (!Settings.Files.ScenesPropertiesData.Exists())
        {
            scenesProperties = new();
            UpdateScenesPropertiesFile();
        }
        scenesProperties = JsonUtility.FromJson<ScenesProperties>(Settings.Files.ScenesPropertiesData.Read());
    }
    public void CreateNewScene(SceneProperties sceneProperties)
    {
        scenesProperties.CreateNewScene(sceneProperties);
        UpdateScenesPropertiesFile();
    }
    public static void DeleteScene(SceneProperties sceneProperties)
    {
        scenesProperties.DeleteScene(sceneProperties);
        UpdateScenesPropertiesFile();
    }
    public static List<SceneProperties> GetScenesProperties()
    {
        if (scenesProperties == null) { return null; }
        return scenesProperties.scenes;
    }
    private static void UpdateScenesPropertiesFile()
    {
        Settings.Files.ScenesPropertiesData.Write(JsonUtility.ToJson(scenesProperties, prettyPrint: true));
    }
}
