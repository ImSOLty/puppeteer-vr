using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class RecordManagementManager : MonoBehaviour
{
    [SerializeField] private Transform newPlayerPosition;
    [SerializeField] private GameObject recordManagementUICanvas;

    public void SwitchToRecordManagement()
    {
        // Deactivate avatar
        CharacterManager characterManager = FindObjectOfType<CharacterManager>();
        characterManager.DetachCharacter();

        // Deactivate UI, managers and action handlers
        FindObjectOfType<AnimationManager>().CurrentActionType = ActionType.PLAYING;
        FindObjectOfType<Teleport>().enabled = false;
        FindObjectOfType<ObjectsUI>().gameObject.SetActive(false);
        FindObjectOfType<RecordingUI>().gameObject.SetActive(false);

        // Activate RecordManagementUI and managers
        recordManagementUICanvas.SetActive(true);
        FindObjectOfType<RecordControlsUI>().enabled = true;

        // Move player to a position
        Player player = FindObjectOfType<Player>();
        player.transform.SetPositionAndRotation(newPlayerPosition.position, Quaternion.identity);
    }
}
