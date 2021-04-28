using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTurretAI : MonoBehaviour
{
    TargetTracker tracker;
    ShootingSystem shooter;
    MultipleTargetRangeChecker rangeCheker;

    void Start()
    {
        tracker = GetComponent<TargetTracker>();
        shooter = GetComponent<ShootingSystem>();
        rangeCheker = GetComponent<MultipleTargetRangeChecker>();
    }

    void Update()
    {
        TargetNearest();
        tracker.SetSlowSpeed(shooter.isShooting());
    }

    void TargetNearest()
    {
        List<Transform> validTargets = rangeCheker.GetValidTargets();

        Transform curTarget = null;
        float closestDist = 0.0f;

        for(int i = 0; i < validTargets.Count; i++)
        {
            if (validTargets[i] != null)
            {
                float dist = Vector3.Distance(transform.position, validTargets[i].position);

                if(!curTarget || dist < closestDist)
                {
                    curTarget = validTargets[i];
                    closestDist = dist;
                }
            }
        }
        if (curTarget != null)
        {
            tracker.FocusOn(curTarget);
            shooter.SetTarget(curTarget);
        }
    }
}
