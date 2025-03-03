using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectManager : MonoBehaviour
{
    CharacterManager characterManager;

    void Awake()
    {
        characterManager = FindObjectOfType<CharacterManager>();
    }

    public void SelectCharacter(VRCharacterInfo character)
    {
        characterManager.CreateCharacterAndSetAsMain(character);
    }
}
