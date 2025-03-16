using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSceneManagementStarter : MonoBehaviour
{
    void Awake()
    {
        FindObjectOfType<AppSceneCreationManager>().InitialSetupWithLocationAndCharacters(
            location: AssetsManager.GetAssetPropertiesByAssetTypeAndUUID(AssetType.LOCATION, uuid: "0457dcc4-1a41-4870-8617-a5aa5c432817"),
            characters: new List<AssetProperties>() {
                AssetsManager.GetAssetPropertiesByAssetTypeAndUUID(AssetType.CHARACTER, uuid: "206108ce-506d-4a3f-971e-bf61e4d4d0a5")
            }
        );
    }
}
