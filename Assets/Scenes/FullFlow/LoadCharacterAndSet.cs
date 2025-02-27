using System.Collections;
using System.Collections.Generic;
using UniGLTF;
using UnityEngine;

public class LoadCharacterAndSet : MonoBehaviour
{
    [SerializeField] private string characterPath;

    private void LoadCharacterAndSetAsMain()
    {
        RuntimeGltfInstance vrmCharacter = FindObjectOfType<ImportManager>().LoadVRMByPathName(characterPath);

    }
}
