using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppSceneCreationManager : MonoBehaviour
{
    public SceneProperties sceneProperties = null;
    public void InitialSetupWithExistingSceneProperties(SceneProperties sceneProperties)
    {
        this.sceneProperties = sceneProperties;
    }
    public void InitialSetupWithNameLocationAndCharacters(
        string name,
        AssetProperties location,
        List<AssetProperties> characters)
    {
        sceneProperties = new();
        sceneProperties.name = name;
        sceneProperties.SetupCharacters(characters);
        sceneProperties.SetupLocation(location);
    }
    public void SetupPropDatas()
    {
        sceneProperties.cameraPropDatas.Clear();
        sceneProperties.objectPropDatas.Clear();
        sceneProperties.lightPropDatas.Clear();
        foreach (CameraInstance cameraInstance in FindObjectsOfType<CameraInstance>())// For each camera
        {
            sceneProperties.cameraPropDatas.Add(cameraInstance.AssemblePropData());
        }
        foreach (LightInstance lightInstance in FindObjectsOfType<LightInstance>())// For each light
        {
            sceneProperties.lightPropDatas.Add(lightInstance.AssemblePropData());
        }
        foreach (ActionObject actionObject in FindObjectsOfType<ActionObject>())// For each object prop
        {
            if (actionObject.isCharacter) continue;

            sceneProperties.objectPropDatas.Add(actionObject.AssemblePropData());
        }
        sceneProperties.sceneUuid ??= Guid.NewGuid().ToString();
    }

    public void Save()
    {
        AppScenesManager appScenesManager = FindObjectOfType<AppScenesManager>();
        appScenesManager.CreateNewOrUpdateScene(sceneProperties);
        Settings.Hints.currentHintAbout = HintAbout.MAIN_MENU;
        SceneManager.LoadScene(Settings.Scenes.MainMenuSceneName);
    }


    public void StartSceneCreation()
    {
        DontDestroyOnLoad(gameObject);
        Settings.Hints.currentHintAbout = HintAbout.SCENE_EDITING;
        SceneManager.LoadScene(Settings.Scenes.AppSceneManagementSceneName);
    }
}
