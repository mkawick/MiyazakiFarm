using System;
using UnityEngine;

public class CropTester : MonoBehaviour
{
    [Range(0, 1)] public float plantScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnValidate()
    {
        /*var spawner = GetComponent<FoodSpawner>();
        foreach (var plant in spawner.spawnableObjectPrefab)
        {
            plant.SetActive(true);
            plant.transform.localScale = Vector3.one * plantScale;
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
