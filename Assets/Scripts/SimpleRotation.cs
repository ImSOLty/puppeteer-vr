using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(new Vector3(1, 1, 1) * speed * Time.deltaTime);
    }
}