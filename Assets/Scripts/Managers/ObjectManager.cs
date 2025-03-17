using UnityEngine;
using UniGLTF;
using Unity.VisualScripting;


public class ObjectManager : MonoBehaviour
{
    private ImportManager importManager;
    [SerializeField] private GameObject baseActionObjectPrefab;
    void Awake()
    {
        importManager = FindObjectOfType<ImportManager>();
    }

    public GameObject CreateObject(AssetProperties objInfo)
    {
        ActionObject actionObject = LoadObjectByPathName(objInfo.fileReference);
        return actionObject.gameObject;
    }

    private ActionObject LoadObjectByPathName(string objPath)
    {
        GameObject objectBase = Instantiate(baseActionObjectPrefab);

        RuntimeGltfInstance gltfObject = importManager.LoadGLTFByPathName(objPath);
        foreach (MeshFilter meshFilter in gltfObject.gameObject.GetComponentsInChildren<MeshFilter>())
        {
            MeshCollider collider = meshFilter.AddComponent<MeshCollider>();
            collider.convex = true;
            collider.sharedMesh = meshFilter.mesh;
            collider.gameObject.layer = objectBase.layer;
        }

        gltfObject.ShowMeshes();
        gltfObject.gameObject.transform.parent = objectBase.transform;

        return objectBase.GetComponent<ActionObject>();
    }
}
