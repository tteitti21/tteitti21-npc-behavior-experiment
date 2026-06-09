using UnityEngine;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
    [Header("FOV Settings")]
    public float viewRadius = 10f;       // max distance NPC can see
    [Range(0, 360)]
    private float viewAngle = 130f;        // cone angle

    [Header("Proximity Override")]
    private float proximityRadius = 2f;   // detection radius that ignores angle
    private float stealthThreshold = 5f;  // minimum stealth to avoid detection

    [Header("Detection")]
    public LayerMask targetMask;         // which layers are detectable (e.g., Player, NPC)
    public LayerMask obstacleMask;       // layers that block view

    [Header("Debug")]
    public bool showGizmos = true;

    /// <summary>
    /// Returns a list of transforms currently visible in the FOV.
    /// </summary>
    public List<Transform> GetVisibleTargets()
    {
        List<Transform> visibleTargets = new List<Transform>();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        foreach (var col in targetsInViewRadius)
        {
            Transform target = col.transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // --- 1. Proximity override ---
            if (distanceToTarget <= proximityRadius)
            {
                if (target.TryGetComponent<ICharacterStats>(out var stats))
                {
                    if (stats != null && stats.StealthLevel <= stealthThreshold)
                    {
                        visibleTargets.Add(target);
                        continue; // skip further checks
                    }
                    else
                    {
                        visibleTargets.Add(target);
                        continue;
                    }
                }

            }

            // --- 2. Normal FOV cone ---
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2f)
            {
                // Raycast to check for obstacles
                if (!Physics.Raycast(transform.position + Vector3.up * 1.5f, dirToTarget, distanceToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }

        return visibleTargets;
    }

    /// <summary>
    /// Converts an angle to a direction relative to NPC forward.
    /// </summary>
    public Vector3 DirFromAngle(float angleInDegrees)
    {
        return Quaternion.Euler(0, angleInDegrees, 0) * transform.forward;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftBoundary = DirFromAngle(-viewAngle / 2f) * viewRadius;
        Vector3 rightBoundary = DirFromAngle(viewAngle / 2f) * viewRadius;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}
