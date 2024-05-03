using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRotation : MonoBehaviour
{
    public float speed;
    public Vector3 fromPos, toPos;
    public int positionsCount;
    private List<Vector3> positions;
    private int targetPosition = 0;
    private Vector3 rotationVector = Vector3.one;

    private void Start()
    {
        positions = new List<Vector3>();
        for (int i = 0; i < positionsCount; i++)
        {
            positions.Add(new Vector3(
                Random.Range(fromPos.x, toPos.x),
                Random.Range(fromPos.y, toPos.y),
                Random.Range(fromPos.z, toPos.z))
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = Vector3.Lerp(transform.position, positions[targetPosition], Time.deltaTime);
        if (Vector3.Distance(gameObject.transform.position, positions[targetPosition]) < 0.2)
        {
            targetPosition += 1;
            targetPosition %= positions.Count;
            rotationVector = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        gameObject.transform.Rotate(rotationVector * (speed * Time.deltaTime));
    }
}