using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAnimationManager : MonoBehaviour
{
    private CharacterManager characterManager;
    void Awake()
    {
        characterManager = FindObjectOfType<CharacterManager>();
    }
    void Start()
    {
        // For now just for testing purposes, later will be automated
        characterManager.CreateCharacter(new VRCharacterInfo("C:\\VKR\\Puppeteer VR\\ExternalAssets\\test.vrm"));
        characterManager.CreateCharacter(new VRCharacterInfo("C:\\VKR\\Puppeteer VR\\ExternalAssets\\test2_short.vrm"));
    }
}
