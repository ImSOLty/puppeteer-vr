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
public class LightPropData : PropData
{
    public Color color;
    public Range range;
    public int intensity;
}

[Serializable]
public class CameraPropData : PropData
{
    public Color color;
    public int FOV;
    public string SpoutName;
}

[Serializable]
public class ObjectPropData : PropData
{
    public string propertiesUuid;
    public Color color;
    public int FOV;
    public string SpoutName;
}


[Serializable]
public class SceneProperties : ISerializationCallbackReceiver
{
    public string sceneUuid;
    public string name;
    public string locationUuid;
    private AssetProperties locationAssetProperties;
    public List<string> characterUuids = new();
    private List<AssetProperties> characterAssetsProperties = new();
    public List<LightPropData> lightPropDatas = new();
    public List<CameraPropData> cameraPropDatas = new();

    public void OnBeforeSerialize()
    {
        characterUuids.Clear();
        foreach (AssetProperties assetProperties in characterAssetsProperties)
        {
            characterUuids.Add(assetProperties.assetUuid);
        }
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
    public List<AssetProperties> GetCharacterAssetsProperties() { return characterAssetsProperties; }
    public AssetProperties GetLocationAssetProperties() { return locationAssetProperties; }
}
[Serializable]
class ScenesProperties
{
    public List<SceneProperties> scenes = new();
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
    public static List<SceneProperties> GetScenesProperties()
    {
        if (scenesProperties == null) { return null; }
        return scenesProperties.scenes;
    }
    private void UpdateScenesPropertiesFile()
    {
        Settings.Files.ScenesPropertiesData.Write(JsonUtility.ToJson(scenesProperties, prettyPrint: true));
    }
}
