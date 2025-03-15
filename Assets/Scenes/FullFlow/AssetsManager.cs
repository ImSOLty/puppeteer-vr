using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public enum AssetType
{
    UNKNOWN,
    LOCATION,
    CHARACTER,
    PROP,
}

[Serializable]
public class AssetProperties : ISerializationCallbackReceiver
{
    public string assetUuid;
    public string name;
    public string fileReference;
    private bool exists;
    public AssetProperties(string name, string fileReference)
    {
        this.name = name;
        this.fileReference = fileReference;
        this.exists = File.Exists(fileReference);
        this.assetUuid = Guid.NewGuid().ToString();
    }

    public void OnBeforeSerialize()
    {
        // Save the data to the serializedData variable.
    }

    public void OnAfterDeserialize()
    {
        exists = File.Exists(fileReference);
    }
    public bool Exists()
    {
        return exists;
    }
}

[Serializable]
public class AssetConfiguration : ISerializationCallbackReceiver
{

    public List<AssetProperties> _assetsPropertiesList = new();
    private Dictionary<string, AssetProperties> assetsPropertiesDict = new();

    public AssetProperties GetByKey(string key)
    {
        if (assetsPropertiesDict.ContainsKey(key))
        {
            return assetsPropertiesDict[key];
        }
        return null;
    }
    public void OnBeforeSerialize()
    {
        _assetsPropertiesList = GetAssetsProperties();
    }

    public void OnAfterDeserialize()
    {
        foreach (AssetProperties assetProperties in _assetsPropertiesList)
        {
            assetsPropertiesDict.Add(assetProperties.assetUuid, assetProperties);
        }
    }

    public void AddAssetsProperties(AssetProperties assetProperties)
    {
        assetsPropertiesDict.Add(assetProperties.assetUuid, assetProperties);
    }
    public void DeleteAssetsProperties(string uuid)
    {
        assetsPropertiesDict.Remove(uuid);
    }

    public List<AssetProperties> GetAssetsProperties()
    {
        return assetsPropertiesDict.Values.ToList();
    }
}

[Serializable]
public class AssetsConfiguration
{
    public AssetConfiguration locationAssetConfiguration = new(),
        characterAssetConfiguration = new(),
        propsAssetConfiguration = new();

    public void AddToAssetsConfiguration(AssetType assetType, AssetProperties assetProperties)
    {
        GetAssetConfigurationByType(assetType).AddAssetsProperties(assetProperties);
    }
    public void DeleteFromAssetsConfiguration(AssetType assetType, string uuid)
    {
        GetAssetConfigurationByType(assetType).DeleteAssetsProperties(uuid);
    }

    public AssetConfiguration GetAssetConfigurationByType(AssetType assetType)
    {
        switch (assetType)
        {
            case AssetType.LOCATION:
                return locationAssetConfiguration;
            case AssetType.CHARACTER:
                return characterAssetConfiguration;
            case AssetType.PROP:
                return propsAssetConfiguration;
        }
        return null;
    }
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
        if (!File.Exists(_assetsConfigPathFullPath) || true)//TEMPORARY always true!!!!!
        {
            assetsConfiguration = new();
            UpdateAssetsConfigurationFile();
            //TEMPORARY!!!!!
            CreateNewAsset(AssetType.CHARACTER, new AssetProperties(name: "test", fileReference: "C:\\VKR\\Puppeteer VR\\ExternalAssets\\test.vrm"));
            CreateNewAsset(AssetType.CHARACTER, new AssetProperties(name: "test2", fileReference: "C:\\VKR\\Puppeteer VR\\ExternalAssets\\test2.vrm"));
            CreateNewAsset(AssetType.CHARACTER, new AssetProperties(name: "test2_short", fileReference: "C:\\VKR\\Puppeteer VR\\ExternalAssets\\test2_short.vrm"));
            CreateNewAsset(AssetType.CHARACTER, new AssetProperties(name: "unexisting", fileReference: "C:\\VKR\\Puppeteer VR\\ExternalAssets\\unexisting.vrm"));
            CreateNewAsset(AssetType.CHARACTER, new AssetProperties(name: "unexisting2", fileReference: "C:\\VKR\\Puppeteer VR\\ExternalAssets\\unexisting2.vrm"));
            CreateNewAsset(AssetType.CHARACTER, new AssetProperties(name: "unexisting3", fileReference: "C:\\VKR\\Puppeteer VR\\ExternalAssets\\unexisting3.vrm"));
            CreateNewAsset(AssetType.CHARACTER, new AssetProperties(name: "unexisting4", fileReference: "C:\\VKR\\Puppeteer VR\\ExternalAssets\\unexisting4.vrm"));
            CreateNewAsset(AssetType.CHARACTER, new AssetProperties(name: "unexisting5", fileReference: "C:\\VKR\\Puppeteer VR\\ExternalAssets\\unexisting5.vrm"));
            CreateNewAsset(AssetType.LOCATION, new AssetProperties(name: "Interior", fileReference: "C:\\VKR\\Puppeteer VR\\ExternalAssets\\Scenes\\Interior.glb"));
            CreateNewAsset(AssetType.PROP, new AssetProperties(name: "cube_smol", fileReference: "C:\\VKR\\Puppeteer VR\\ExternalAssets\\cube-smol.glb"));
        }
        assetsConfiguration = JsonUtility.FromJson<AssetsConfiguration>(File.ReadAllText(_assetsConfigPathFullPath));
    }

    public AssetProperties GetAssetPropertiesByAssetTypeAndUUID(AssetType assetType, string uuid)
    {
        return assetsConfiguration.GetAssetConfigurationByType(assetType).GetByKey(uuid);
    }

    public List<AssetProperties> GetAssetsPropertiesByAssetType(AssetType assetType)
    {
        return assetsConfiguration.GetAssetConfigurationByType(assetType).GetAssetsProperties();
    }

    public void CreateNewAsset(AssetType assetType, AssetProperties assetProperties)
    {
        assetsConfiguration.AddToAssetsConfiguration(assetType, assetProperties);
        UpdateAssetsConfigurationFile();
    }
    public void DeleteAnAssetByUUID(AssetType assetType, string uuid)
    {
        assetsConfiguration.DeleteFromAssetsConfiguration(assetType, uuid);
        UpdateAssetsConfigurationFile();
    }
    public void DeleteAnAsset(AssetType assetType, AssetProperties assetProperties) { DeleteAnAssetByUUID(assetType, assetProperties.assetUuid); }

    private void UpdateAssetsConfigurationFile()
    {
        File.WriteAllText(_assetsConfigPathFullPath, JsonUtility.ToJson(assetsConfiguration, prettyPrint: true));
    }

}
