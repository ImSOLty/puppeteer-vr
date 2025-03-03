using System.Collections;
using System.Collections.Generic;
using UniGLTF;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLocationUI : MonoBehaviour
{
    public void LoadLocationByPath(string filePath)
    {
        RuntimeGltfInstance loaded = FindObjectOfType<ImportManager>().LoadGLTFByPathName(filePath);
        loaded.ShowMeshes();
        DontDestroyOnLoad(loaded.gameObject);
        SceneManager.LoadScene("Action");
    }
}
