using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsUI : MonoBehaviour
{
    ObjectManager objectManager;

    public void Awake()
    {
        objectManager = FindObjectOfType<ObjectManager>();
    }


    public void SetCharacter(string pathName)
    {
        objectManager.SelectCharacter(new VRCharacterInfo(pathName));
    }

}
