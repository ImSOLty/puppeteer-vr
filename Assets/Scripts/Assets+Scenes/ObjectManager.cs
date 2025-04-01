using UnityEngine;
using UniGLTF;
using Unity.VisualScripting;


public class ObjectManager : MonoBehaviour
{
    [SerializeField] private ImportManager importManager;
    [SerializeField] private GameObject baseActionObjectPrefab;

    public GameObject CreateObject(AssetProperties objInfo)
    {
        ActionObject actionObject = LoadObjectByPathName(objInfo.fileReference);
        actionObject.SetAssetProperties(objInfo);
        return actionObject.gameObject;
    }

    private ActionObject LoadObjectByPathName(string objPath)
    {
        GameObject objectBase = Instantiate(baseActionObjectPrefab).transform.GetChild(0).gameObject;

        RuntimeGltfInstance gltfObject = importManager.LoadGLTFByPathName(objPath);
        foreach (MeshFilter meshFilter in gltfObject.gameObject.GetComponentsInChildren<MeshFilter>())
        {
            MeshCollider collider = meshFilter.AddComponent<MeshCollider>();
            collider.convex = true;
            collider.sharedMesh = meshFilter.mesh;
            collider.gameObject.layer = objectBase.layer;
        }

        gltfObject.ShowMeshes();
        gltfObject.gameObject.transform.SetParent(objectBase.transform);

        return objectBase.GetComponent<ActionObject>();
    }
}
