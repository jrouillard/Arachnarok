using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAI : MonoBehaviour
{
    public Transform target;
    public int requiredDistance;
    public SpiderController controller;

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
    }
}
