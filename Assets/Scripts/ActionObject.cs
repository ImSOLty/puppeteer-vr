using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    private bool isActive = false;
    public bool isCharacter = false;
    private Transform[] childrenTransforms;
    void Start()
    {
        childrenTransforms = GetComponentsInChildren<Transform>();
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
        RigidbodyActive(active);
    }

    public void RigidbodyActive(bool active = true)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = active;
        }
    }
}
