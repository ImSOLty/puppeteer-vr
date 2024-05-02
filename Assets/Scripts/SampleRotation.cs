using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleRotation : MonoBehaviour
{
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(new Vector3(1, 1, 1) * speed * Time.deltaTime);
    }
}