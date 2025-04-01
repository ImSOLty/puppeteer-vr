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
public class LightPropData : PropData
{
    public Color32 color;
    public float range;
    public float intensity;

    public LightPropData(Vector3 position, Vector3 rotation, Color32 color, float range, float intensity)
    {
        propUuid = Guid.NewGuid().ToString();
        this.position = position;
        this.rotation = rotation;
        this.color = color;
        this.range = range;
        this.intensity = intensity;
    }
}



[Serializable]
public class SceneProperties : ISerializationCallbackReceiver
{
    public string sceneUuid = null;
    public string name;
    public string locationUuid;
    public string updatedAt;
    private AssetProperties locationAssetProperties = null;
    public List<string> characterUuids = new();
    private List<AssetProperties> characterAssetsProperties = new();
    public List<CameraPropData> cameraPropDatas = new();
    public List<ObjectPropData> objectPropDatas = new();
    public List<LightPropData> lightPropDatas = new();

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
        updatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        characterAssetsProperties = characters;
    }
    public void SetupLocation(AssetProperties location)
    {
        updatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        locationAssetProperties = location;
    }
    public void UpdateScene(SceneProperties other)
    {
        sceneUuid = other.sceneUuid;
        name = other.name;
        locationUuid = other.locationUuid;
        characterUuids = other.characterUuids;
        cameraPropDatas = other.cameraPropDatas;
        objectPropDatas = other.objectPropDatas;
        lightPropDatas = other.lightPropDatas;
        locationAssetProperties = AssetsManager.GetAssetPropertiesByAssetTypeAndUUID(AssetType.LOCATION, locationUuid);
        characterAssetsProperties.Clear();
        foreach (string uuid in characterUuids)
        {
            characterAssetsProperties.Add(AssetsManager.GetAssetPropertiesByAssetTypeAndUUID(AssetType.CHARACTER, uuid));
        }
        updatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    public AssetProperties GetLocationAssetProperties() { return locationAssetProperties; }
    public List<AssetProperties> GetCharacterAssetsProperties() { return characterAssetsProperties; }
    public List<CameraPropData> GetCameraPropDatas() { return cameraPropDatas; }
    public List<ObjectPropData> GetObjectPropDatas() { return objectPropDatas; }
    public List<LightPropData> GetLightPropDatas() { return lightPropDatas; }
}
[Serializable]
public class ScenesProperties
{
    public List<SceneProperties> scenes = new();

    public SceneProperties GetSceneByUuid(string uuid) { return scenes.Find((scene) => scene.sceneUuid == uuid); }
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
    public void CreateNewOrUpdateScene(SceneProperties sceneProperties)
    {
        SceneProperties scene = scenesProperties.GetSceneByUuid(sceneProperties.sceneUuid);
        if (scene != null)
        {
            scene.UpdateScene(sceneProperties);
        }
        else
        {
            scenesProperties.CreateNewScene(sceneProperties);
        }
        UpdateScenesPropertiesFile();
    }
    public static void DeleteScene(SceneProperties sceneProperties)
    {
        scenesProperties.DeleteScene(sceneProperties);
        UpdateScenesPropertiesFile();
    }
    public static List<SceneProperties> GetScenesPropertiesList()
    {
        if (scenesProperties == null) { return null; }
        return scenesProperties.scenes;
    }
    public static ScenesProperties GetScenesProperties()
    {
        return scenesProperties;
    }
    private static void UpdateScenesPropertiesFile()
    {
        Settings.Files.ScenesPropertiesData.Write(JsonUtility.ToJson(scenesProperties, prettyPrint: true));
    }
}
