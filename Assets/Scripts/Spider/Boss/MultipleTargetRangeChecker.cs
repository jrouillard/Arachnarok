
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultipleTargetRangeChecker : MonoBehaviour {
    public List<string> tags;

    List<Transform> targets = new List<Transform>();

    void OnTriggerEnter(Collider other)
    {
        bool invalid = true;

        foreach (string tag in tags)
        {
            if (other.CompareTag(tag))
            {
                invalid = false;
                break;
            }
        }

        if (invalid)
        {
            return;
        }

        targets.Add(other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        foreach (Transform target in targets)
        {
            if (other.transform == target)
            {
                targets.Remove(other.transform);
                return;
            }
        }
    }

    public List<Transform> GetValidTargets()
    {
        return targets;
    }

    public bool InRange(Transform go)
    {
        foreach (Transform target in targets)
        {
            if (go == target)
                return true;
        }

        return false;
    }
}
