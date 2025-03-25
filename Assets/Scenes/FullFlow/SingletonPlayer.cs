using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SingletonPlayer : MonoBehaviour
{
    private Player _instance;
    [SerializeField] private Vector3 defaultPosistion;
    [SerializeField] private GameObject playerPrefab;

    void Awake()
    {
        Application.targetFrameRate = 120;
        _instance = FindObjectOfType<Player>();
        if (!_instance)
        {
            playerPrefab = Instantiate(playerPrefab);
            _instance = playerPrefab.GetComponent<Player>();
        }
        _instance.gameObject.transform.SetPositionAndRotation(defaultPosistion, Quaternion.identity);
    }
}
