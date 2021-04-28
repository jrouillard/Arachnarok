
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SingleTargetRangeChecker : MonoBehaviour {
    public Transform target;
    public float range = 120f;

    public bool IsTargetInRange()
    {
        if (!target)
        {
            return false;
        }
        Vector3 offset = target.position - transform.position;
        return offset.sqrMagnitude  < range * range;
    }
}
