using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppSceneCreationManager : MonoBehaviour
{
    public AssetsManager assetsManager;
    SceneProperties sceneProperties = null;
    public void InitialSetupWithLocationAndCharacters(AssetProperties location, List<AssetProperties> characters)
    {
        sceneProperties = new();
        sceneProperties.SetupCharacters(characters);
        sceneProperties.SetupLocation(location);
    }
    public void StartSceneCreation()
    {
        DontDestroyOnLoad(gameObject);
    }
}
