using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAnimationManager : MonoBehaviour
{
    private CharacterManager characterManager;
    private ObjectManager objectManager;

    // For now just for testing purposes, later will be automated
    public VRObjectInfo[] characterOptions = {
        new("C:\\VKR\\Puppeteer VR\\ExternalAssets\\test.vrm"),
        new("C:\\VKR\\Puppeteer VR\\ExternalAssets\\test2_short.vrm"),
    };
    void Awake()
    {
        characterManager = FindObjectOfType<CharacterManager>();
        objectManager = FindObjectOfType<ObjectManager>();

        foreach (VRObjectInfo characterInfo in characterOptions)
        {
            characterManager.CreateCharacter(characterInfo);
        }

        GameObject cube = objectManager.CreateObject(new VRObjectInfo("C:\\VKR\\Puppeteer VR\\ExternalAssets\\cube-smol.glb"));
        cube.transform.Translate(0, 1, 0);
    }
}
