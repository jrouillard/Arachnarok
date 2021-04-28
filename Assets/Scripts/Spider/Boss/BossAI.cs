using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossAI : MonoBehaviour
{
    public int requiredDistance;
    private BossController controller;
    MultipleTargetRangeChecker rangeCheker;

    void Start()
    {
        controller = GetComponent<BossController>();
        rangeCheker = GetComponent<MultipleTargetRangeChecker>();
    }

    void Update()
    {
        TargetNearest();
    }

    void TargetNearest()
    {
        List<Transform> validTargets = rangeCheker.GetValidTargets();

        Transform curTarget = null;
        float closestDist = 0.0f;

        for(int i = 0; i < validTargets.Count; i++)
        {
            if (validTargets[i] != null) {
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
            Vector3 offset = transform.position - curTarget.position;
            if (offset.sqrMagnitude > requiredDistance * requiredDistance)
            {
                controller.FaceTarget(curTarget);
            }
        }
    }
}
