using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppSceneCreationManager : MonoBehaviour
{
    public SceneProperties sceneProperties = null;
    public void InitialSetupWithLocationAndCharacters(AssetProperties location, List<AssetProperties> characters)
    {
        sceneProperties = new();
        sceneProperties.SetupCharacters(characters);
        sceneProperties.SetupLocation(location);
    }

    public void StartSceneCreation()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(Settings.Scenes.AppSceneManagementSceneName);
    }
}
