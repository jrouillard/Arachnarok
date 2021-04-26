using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpiderAI : MonoBehaviour
{
    public Transform target;
    public int requiredDistance;
    public SpiderController controller;
    public UnityEvent targetReached;

    void Start()
    {
        controller = GetComponent<SpiderController>();
    }

    void Update()
    {
        Vector3 offset = transform.position - target.position;
        if (offset.sqrMagnitude > requiredDistance * requiredDistance)
        {
            controller.FaceTarget(target);
        }
        else if (targetReached != null)
        {
            targetReached.Invoke();
        }
    }
}
