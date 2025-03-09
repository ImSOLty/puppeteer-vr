using UnityEngine;

public class ObjectsUI : MonoBehaviour
{
    ObjectManager objectManager;
    CharacterManager characterManager;

    public void Awake()
    {
        objectManager = FindObjectOfType<ObjectManager>();
        characterManager = FindObjectOfType<CharacterManager>();
    }


    public void SetCharacter(string pathName)
    {
        characterManager.SetCharacterAsMain(new VRObjectInfo(pathName));
    }

}
