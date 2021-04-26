
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangeChecker : MonoBehaviour {
    public Transform target;
    public float range = 120f;

    public bool IsTargetInRange()
    {
        Vector3 offset = target.position - transform.position;
        return offset.sqrMagnitude  < range * range;
    }

}
