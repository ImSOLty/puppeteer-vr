using System.Collections;
using System.Collections.Generic;
using UniGLTF;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Valve.VR;

public class LoadCharacterAndSet : MonoBehaviour
{
    private TrackerManager trackerManager;
    [SerializeField] private GameObject VRIKRig;
    [SerializeField] private string characterPath;

    private void Awake()
    {
        trackerManager = FindObjectOfType<TrackerManager>();
        LoadCharacterAndSetAsMain();
    }

    private void LoadCharacterAndSetAsMain()
    {
        RuntimeGltfInstance vrmCharacter = FindObjectOfType<ImportManager>().LoadVRMByPathName(characterPath);
        vrmCharacter.ShowMeshes();

        GameObject characterGameObject = vrmCharacter.gameObject;
        GameObject characterRig = Instantiate(VRIKRig, characterGameObject.transform);
        Rig rig = characterRig.AddComponent<Rig>();

        // IKTargetFollowVRRig IKtargetFollow = characterGameObject.AddComponent<IKTargetFollowVRRig>();
        trackerManager.DefineTrackers();

        RigHelperSetup helperSetup = characterRig.GetComponent<RigHelperSetup>();
        helperSetup.provideSources(vrm: characterGameObject.transform, trackerManager);
        helperSetup.Setup();

        RigBuilder rigBuilder = characterGameObject.AddComponent<RigBuilder>();
        rigBuilder.layers.Clear();
        rigBuilder.layers.Add(new RigLayer(rig, active: true));
        rigBuilder.Build();
    }
}
