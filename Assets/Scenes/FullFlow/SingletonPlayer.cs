using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class SingletonPlayer : MonoBehaviour
{
    private Player _instance;
    [SerializeField] private GameObject playerPrefab;

    void Awake()
    {
        _instance = FindObjectOfType<Player>();
        if (!_instance)
        {
            playerPrefab = Instantiate(playerPrefab);
            _instance = playerPrefab.GetComponent<Player>();
        }
    }
}
