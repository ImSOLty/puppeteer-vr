using UniGLTF;
using Unity.VisualScripting;
using UnityEngine;

public class SceneSetupManager : MonoBehaviour
{
    [SerializeField] private GameObject cameraPrefab;
    private CharacterManager characterManager;
    private ObjectManager objectManager;
    private ImportManager importManager;
    private AppSceneCreationManager sceneCreationManager;
    private SceneProperties sceneProperties = null;
    private bool selfDriven;

    private GameObject locationObject;

    void Awake()
    {
        characterManager = FindObjectOfType<CharacterManager>();
        objectManager = FindObjectOfType<ObjectManager>();
        importManager = FindObjectOfType<ImportManager>();
        sceneCreationManager = FindObjectOfType<AppSceneCreationManager>();

        sceneProperties = Settings.Animation.ScenePropertiesData;
        selfDriven = Settings.Animation.AnimationMode != Mode.PROPS_MANAGEMENT && Settings.Animation.AnimationMode != Mode.PROPS_MANAGEMENT_EDIT;
        if (!selfDriven)
        {
            sceneProperties = sceneCreationManager.sceneProperties;
        }

        SceneSetup();
        PropsSetup();
        CharacterSetup();
        // SetupForMode();
    }

    void SceneSetup()
    {
        RuntimeGltfInstance location = importManager.LoadGLTFByPathName(sceneProperties.GetLocationAssetProperties().fileReference);
        location.ShowMeshes();
        locationObject = location.gameObject;
        foreach (MeshFilter meshFilter in locationObject.GetComponentsInChildren<MeshFilter>())
        {
            MeshCollider collider = meshFilter.AddComponent<MeshCollider>();
            collider.sharedMesh = meshFilter.sharedMesh;
            // collider.convex = true;
        }
    }

    void PropsSetup()
    {
        // Setting up cameras
        foreach (CameraPropData propData in sceneProperties.GetCameraPropDatas())
        {
            // Create and set cameras
            GameObject cameraObject = Instantiate(cameraPrefab);
            CameraInstance cameraInstance = cameraObject.GetComponent<CameraInstance>();
            cameraInstance.SetPropData(propData);
        }

        // Setting up objects
        foreach (ObjectPropData propData in sceneProperties.GetObjectPropDatas())
        {
            // Create and set objects
            GameObject propObject = objectManager.CreateObject(AssetsManager.GetAssetPropertiesByAssetTypeAndUUID(
                AssetType.PROP,
                propData.assetPropertiesUuid
            ));
            ActionObject actionObject = propObject.GetComponent<ActionObject>();
            actionObject.SetPropData(propData);
        }
    }

    void CharacterSetup()
    {
        foreach (AssetProperties characterInfo in sceneProperties.GetCharacterAssetsProperties())
        {
            characterManager.CreateCharacter(characterInfo);
        }
    }
}
