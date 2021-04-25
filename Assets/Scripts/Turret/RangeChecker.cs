
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangeChecker : MonoBehaviour {
    public List<string> tags;

    List<GameObject> targets = new List<GameObject>();

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

        targets.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        foreach (GameObject target in targets)
        {
            if (other.gameObject == target)
            {
                targets.Remove(other.gameObject);
                return;
            }
        }
    }

    public List<GameObject> GetValidTargets()
    {
        return targets;
    }

    public bool InRange(GameObject go)
    {
        foreach (GameObject target in targets)
        {
            if (go == target)
                return true;
        }

        return false;
    }
}
