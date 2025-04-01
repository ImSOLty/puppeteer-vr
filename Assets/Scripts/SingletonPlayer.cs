using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SingletonPlayer : MonoBehaviour
{
    private Player _instance;
    [SerializeField] private Vector3 defaultPosistion;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private CharacterManager characterManager;
    private AssetProperties defaultCharacter = new(
        name: "DEFAULT",
        fileReference: Path.Combine(Settings.Files.AssetsFolderPath, "girl.vrm"),
        previewPath: Path.Combine(Settings.Files.AssetsFolderPath, "girl-preview.vrm")
    );

    void Awake()
    {
        Application.targetFrameRate = 120;
        _instance = FindObjectOfType<Player>();
        if (!_instance)
        {
            playerPrefab = Instantiate(playerPrefab);
            _instance = playerPrefab.GetComponent<Player>();
        }
        characterManager.SetCharacterAsMain(defaultCharacter); // Default character
        _instance.gameObject.transform.SetPositionAndRotation(defaultPosistion, Quaternion.identity);
    }
}
