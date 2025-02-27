using System.Collections;
using System.Collections.Generic;
using UniGLTF;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLocation : MonoBehaviour
{
    [SerializeField] private string defaultPath;

    public void Start()
    {
        LoadLocationByPath(defaultPath);
    }
    public void LoadLocationByPath(string filePath)
    {
        RuntimeGltfInstance loaded = FindObjectOfType<ImportManager>().LoadGLTFByPathName(filePath);
        loaded.ShowMeshes();
        DontDestroyOnLoad(loaded.gameObject);
        SceneManager.LoadScene("Action");
    }
}
