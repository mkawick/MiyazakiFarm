using UnityEngine;

public class WatchOutForFearTriggers : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField, Range(5, 100)] private float detectionRadius = 10f;
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private bool showDebugVisuals = true;
    [SerializeField] private float safeSpaceSelectionDistance = 12f;
    [SerializeField] private float scanFrequency = 1;

    private float nextScanTime;
    public WatchOutForFearTriggers()
    {
        if (detectionRadius < 5)
            detectionRadius = 5;
        if(safeSpaceSelectionDistance < detectionRadius)
            safeSpaceSelectionDistance = detectionRadius + 2;
        nextScanTime = 0;
    }
    public GameObject DetectedFearTrigger
    {
        get;
        set;
    }

    public GameObject UpdateDetector()
    {
        if (nextScanTime > Time.time)
        {
            return DetectedFearTrigger;
        }
        Utils.Tools.AdvanceExpry(ref nextScanTime, scanFrequency);
        // Perform sphere check
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionMask);

        if (colliders.Length > 0)
        {
            DetectedFearTrigger = colliders[0].gameObject;
        }
        else
        {
            DetectedFearTrigger = null;
        }
        return DetectedFearTrigger;
    }

    public Vector3 SelectSafePlace()
    {
        if (DetectedFearTrigger is null)
        {
            return transform.position;
        }
        var dirToTrigger =  (DetectedFearTrigger.transform.position - transform.position).normalized;
        var safeSpot = dirToTrigger *  safeSpaceSelectionDistance;
        var randomDir = Random.insideUnitCircle * (safeSpaceSelectionDistance - detectionRadius);
        return safeSpot + new Vector3(randomDir.x, 0, randomDir.y);
    }

    // Debug visualization
    private void OnDrawGizmos()
    {
        if (!showDebugVisuals || this.enabled == false) return;

        Gizmos.color = DetectedFearTrigger ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

    }
}
