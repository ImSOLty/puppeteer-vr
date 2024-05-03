using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Material[] materials;
    [SerializeField] private int number;
    [SerializeField] private float timeBetween;
    [SerializeField] private float force;
    private bool generate = false;
    [SerializeField] private Transform Room;

    private void Start()
    {
        StartCoroutine(Generate());
    }

    private void FixedUpdate()
    {
        if (!generate) return;
        GameObject prefab = Instantiate(ballPrefab, transform.position, Quaternion.identity, Room);
        prefab.transform.localScale *= Random.Range(0.5f, 2f);
        prefab.GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Length)];
        prefab.GetComponent<Rigidbody>()
            .AddForce(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 0f), Random.Range(-1f, 1f)) * force);
        generate = false;
    }

    IEnumerator Generate()
    {
        for (int i = 0; i < number; i++)
        {
            generate = true;
            yield return new WaitForSeconds(timeBetween);
        }
    }
}