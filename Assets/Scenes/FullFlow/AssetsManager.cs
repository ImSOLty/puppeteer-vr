using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum AssetType
{
    UNKNOWN,
    LOCATION,
    CHARACTER,
    PROP,
}

[Serializable]
public class AssetProperties
{
    public string name;
    public string fileReference;
    public bool exists;
}

[Serializable]
public class AssetsConfiguration
{
    public Dictionary<string, AssetProperties> locationAssetsProperties = new();
    public Dictionary<string, AssetProperties> characterAssetsProperties = new();
    public Dictionary<string, AssetProperties> propsAssetsProperties = new();
}

public class AssetsManager : MonoBehaviour
{
    public string assetsConfigPath = "AssetsConfiguration.json";
    private string _assetsConfigPathFullPath;

    private AssetsConfiguration assetsConfiguration;

    void Awake()
    {
        _assetsConfigPathFullPath = Path.Combine(Application.streamingAssetsPath, assetsConfigPath);
        ParseAssetsConfiguration();
    }

    private void ParseAssetsConfiguration()
    {
        if (!File.Exists(_assetsConfigPathFullPath))
        {
            assetsConfiguration = new();
            File.WriteAllText(_assetsConfigPathFullPath, JsonUtility.ToJson(assetsConfiguration));
        }
        assetsConfiguration = JsonUtility.FromJson<AssetsConfiguration>(File.ReadAllText(_assetsConfigPathFullPath));
    }

    public AssetProperties GetAssetPropertiesByAssetTypeAndUUID(AssetType assetType, string uuid)
    {
        Dictionary<string, AssetProperties> assetProperties = GetAssetsPropertiesByAssetType(assetType);
        if (assetProperties.ContainsKey(uuid))
        {
            return assetProperties[uuid];
        }
        return null;
    }

    public Dictionary<string, AssetProperties> GetAssetsPropertiesByAssetType(AssetType assetType)
    {
        switch (assetType)
        {
            case AssetType.LOCATION:
                return assetsConfiguration.locationAssetsProperties;
            case AssetType.CHARACTER:
                return assetsConfiguration.characterAssetsProperties;
            case AssetType.PROP:
                return assetsConfiguration.propsAssetsProperties;
        }
        return null;
    }

}
