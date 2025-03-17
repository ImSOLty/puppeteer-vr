using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Assertions;
using Valve.VR.InteractionSystem;

public class ActionObjectData
{
    public Vector3 position;
    public Quaternion rotation;

    public ActionObjectData(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
    }
}

public class ActionObject : MonoBehaviour
{
    private ObjectPropData propData = null;
    private AssetProperties assetProperties = null;
    private Rigidbody rb;
    private Throwable throwable;
    private bool isActive = false;
    public bool isCharacter = false;
    private Transform[] childrenTransforms;
    void Start()
    {
        childrenTransforms = GetComponentsInChildren<Transform>();
        rb = GetComponent<Rigidbody>();
        throwable = GetComponent<Throwable>();
    }
    public ActionObjectData[] GetActionData()
    {
        List<ActionObjectData> result = new() { new ActionObjectData(transform) };
        foreach (Transform t in childrenTransforms)
        {
            result.Add(new ActionObjectData(t));
        }
        return result.ToArray();
    }

    public void SetByActionData(ActionObjectData[] data)
    {
        Assert.AreEqual(data.Length, childrenTransforms.Length + 1);
        transform.SetPositionAndRotation(data[0].position, data[0].rotation);
        for (int i = 0; i < childrenTransforms.Length; i++)
        {
            childrenTransforms[i].SetPositionAndRotation(data[i + 1].position, data[i + 1].rotation);
        }
    }

    public bool IsActive()
    {
        return isActive;
    }
    public void SetActive(bool active = true)
    {
        isActive = active;
        SetRigidbodyActive(active);
    }

    public void SetRigidbodyActive(bool active = true)
    {
        if (rb != null)
        {
            rb.useGravity = active;
        }
    }
    public void SetInteractable(bool active = true)
    {
        if (throwable != null)
        {
            throwable.enabled = active;
        }
    }

    public void SetAssetProperties(AssetProperties assetProperties) { this.assetProperties = assetProperties; }

    public void SetPropData(ObjectPropData propData)
    {
        this.propData = propData;
        transform.position = propData.position;
        transform.eulerAngles = propData.rotation;
    }

    public ObjectPropData AssemblePropData()
    {
        ObjectPropData result = new ObjectPropData(
            position: transform.position,
            rotation: transform.rotation.eulerAngles,
            assetPropertiesUuid: assetProperties.assetUuid
        );
        if (propData != null) { result.propUuid = propData.propUuid; }
        return result;
    }
}
