using System;
using System.Collections.Generic;
using UnityEngine;

public class FindFood : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private bool showDebugVisuals = true;
    [SerializeField] private float scanFrequency = 1;
    float nextScanTime;

    private void Start()
    {
        DetectedFood = new List<GameObject>();
        nextScanTime = 0;
    }

    public List<GameObject> DetectedFood
    {
        get;
        set;
    }

    public void UpdateDetector()
    {
        if (nextScanTime > Time.time)
        {
            return;
        }
        Utils.Tools.AdvanceExpry(ref nextScanTime, scanFrequency);
        
        // Perform sphere check
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionMask);

        DetectedFood.Clear();
        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                var plant = collider.GetComponent<FarmPlant>();
                var isSomethingToEat = plant is not null && plant.IsEdible() ? true : false;
                if (isSomethingToEat && !DetectedFood.Contains(collider.gameObject))
                {
                    DetectedFood.Add(collider.gameObject);
                }
            }
        }
    }

    // Debug visualization
    private void OnDrawGizmos()
    {
        if (!showDebugVisuals || this.enabled == false) return;

        Gizmos.color = DetectedFood.Count >0 ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

    }
}
