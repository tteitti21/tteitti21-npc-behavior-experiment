using UnityEngine;

public static class TargetingHelper
{
    public static GameObject FindClosestWithTag(Transform self, string tag)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (var obj in objs)
        {
            float dist = Vector3.Distance(self.position, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj;
            }
        }
        return closest;
    }

    public static void FaceTarget(Transform self, Transform target, float speed = 5f)
    {
        if (target == null) return;

        Vector3 direction = (target.position - self.position).normalized;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            self.rotation = Quaternion.Slerp(self.rotation, lookRotation, Time.deltaTime * speed);
        }
    }
}
