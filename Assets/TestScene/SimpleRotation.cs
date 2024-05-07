using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRotation : MonoBehaviour
{
    [SerializeField] private float speed, rotationSpeed;
    [SerializeField] private int positionsCount;
    [SerializeField] private float moveSize;
    [SerializeField] private Material[] materials;
    private List<Vector3> positions;
    private int _targetPosition = 0;
    private Vector3 _rotationVector = Vector3.one;
    private bool _allowedToMove = false;

    private void Start()
    {
        SetRandomColor();
        Vector3 initialPos = transform.position;
        positions = new List<Vector3>();
        for (int i = 0; i < positionsCount; i++)
        {
            positions.Add(initialPos + new Vector3(
                Random.Range(-moveSize, moveSize),
                Random.Range(-moveSize, moveSize),
                Random.Range(-moveSize, moveSize))
            );
        }
    }

    void SetRandomColor()
    {
        GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        if (!_allowedToMove)
        {
            return;
        }

        gameObject.transform.position =
            Vector3.Lerp(transform.position, positions[_targetPosition], Time.deltaTime * speed);
        if (Vector3.Distance(gameObject.transform.position, positions[_targetPosition]) < 0.3f)
        {
            _targetPosition += 1;
            _targetPosition %= positions.Count;
            _rotationVector = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        gameObject.transform.Rotate(_rotationVector * (rotationSpeed * Time.deltaTime));
    }

    public void Allow(bool allowed)
    {
        _allowedToMove = allowed;
    }
}