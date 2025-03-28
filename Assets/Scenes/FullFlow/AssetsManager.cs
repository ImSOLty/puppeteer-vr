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
    public string createdAt;
    public string fileReference;
    public string previewPath;
    private Texture2D previewTexture;
    private bool previewExists;
    private bool exists;
    public AssetProperties(string name, string fileReference, string previewPath)
    {
        this.name = name;
        this.fileReference = fileReference;
        this.previewPath = previewPath;
        this.createdAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.exists = File.Exists(fileReference);
        this.previewExists = File.Exists(previewPath);
        this.assetUuid = Guid.NewGuid().ToString();

        SetupPreviewTexture();
    }

    public void OnBeforeSerialize()
    {
        // Save the data to the serializedData variable.
    }

    public void OnAfterDeserialize()
    {
        exists = File.Exists(fileReference);
        previewExists = File.Exists(previewPath);

        SetupPreviewTexture();
    }
    public bool Exists()
    {
        return exists;
    }
    public bool PreviewExists()
    {
        return previewExists;
    }
    public Texture2D GetPreviewTexture()
    {
        return previewTexture;
    }
    private void SetupPreviewTexture()
    {
        if (!previewExists) { previewTexture = null; return; }

        byte[] bytes = File.ReadAllBytes(previewPath);
        Texture2D tex = new Texture2D(2, 2); // Size does not matter
        tex.LoadImage(bytes);
        previewTexture = tex;
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
    private static AssetsConfiguration assetsConfiguration = null;

    void Awake()
    {
        ParseAssetsConfiguration();
    }

    private void ParseAssetsConfiguration()
    {
        if (!Settings.Files.AssetsConfiguration.Exists())
        {
            assetsConfiguration = new();
            UpdateAssetsConfigurationFile();
        }
        assetsConfiguration = JsonUtility.FromJson<AssetsConfiguration>(Settings.Files.AssetsConfiguration.Read());
    }

    public static AssetProperties GetAssetPropertiesByAssetTypeAndUUID(AssetType assetType, string uuid)
    {
        if (assetsConfiguration == null) { return null; }
        return assetsConfiguration.GetAssetConfigurationByType(assetType).GetByKey(uuid);
    }

    public static List<AssetProperties> GetAssetsPropertiesByAssetType(AssetType assetType)
    {
        if (assetsConfiguration == null) { return null; }
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
        Settings.Files.AssetsConfiguration.Write(JsonUtility.ToJson(assetsConfiguration, prettyPrint: true));
    }

}
