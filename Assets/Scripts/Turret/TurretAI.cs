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
        if (rangeCheker.IsTargetInRange())
        {
            tracker.FocusOn(rangeCheker.target);
            shooter.SetTarget(rangeCheker.target);
        } else {
            tracker.FocusOn(null);
            shooter.SetTarget(null);
        }
    }
}
