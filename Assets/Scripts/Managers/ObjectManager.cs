using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectManager : MonoBehaviour
{
    private ImportManager importManager;

    void Awake()
    {
        importManager = FindObjectOfType<ImportManager>();
    }

    public void CreateObject()
    {

    }
}
