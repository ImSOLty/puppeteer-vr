using System.Collections;
using System.Collections.Generic;
using UniGLTF;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LoadCharacterAndSet : MonoBehaviour
{
    [SerializeField] private GameObject VRIKRig;
    [SerializeField] private string characterPath;

    private void Start()
    {
        LoadCharacterAndSetAsMain();
    }

    private void LoadCharacterAndSetAsMain()
    {
        RuntimeGltfInstance vrmCharacter = FindObjectOfType<ImportManager>().LoadVRMByPathName(characterPath);
        vrmCharacter.ShowMeshes();

        GameObject characterGameObject = vrmCharacter.gameObject;
        GameObject characterRig = Instantiate(VRIKRig, characterGameObject.transform);
        Rig rig = characterRig.AddComponent<Rig>();

        RigHelperSetup helperSetup = characterRig.GetComponent<RigHelperSetup>();
        helperSetup.provideSources(vrm: characterGameObject.transform);
        helperSetup.Setup();

        IKTargetFollowVRRig IKtargetFollow = characterGameObject.AddComponent<IKTargetFollowVRRig>();

        RigBuilder rigBuilder = characterGameObject.AddComponent<RigBuilder>();
        rigBuilder.layers.Clear();
        rigBuilder.layers.Add(new RigLayer(rig, active: true));
        rigBuilder.Build();
    }
}
