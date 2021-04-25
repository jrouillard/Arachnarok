using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    TargetTracker tracker;
    ShootingSystem shooter;
    RangeChecker rangeCheker;

    void Start()
    {
        tracker = GetComponent<TargetTracker>();
        shooter = GetComponent<ShootingSystem>();
        rangeCheker = GetComponent<RangeChecker>();
    }

    void Update()
    {
        TargetNearest();
        tracker.SetSlowSpeed(shooter.isShooting());
    }

    void TargetNearest()
    {
        List<GameObject> validTargets = rangeCheker.GetValidTargets();

        GameObject curTarget = null;
        float closestDist = 0.0f;

        for(int i = 0; i < validTargets.Count; i++)
        {
            float dist = Vector3.Distance(transform.position, validTargets[i].transform.position);

            if(!curTarget || dist < closestDist)
            {
                curTarget = validTargets[i];
                closestDist = dist;
            }
        }
        if (curTarget != null)
        {
            tracker.FocusOn(curTarget);
            shooter.SetTarget(curTarget);
        }
    }
}
