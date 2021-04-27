using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossAI : MonoBehaviour
{
    public Transform target;
    public int requiredDistance;
    private BossController controller;
    public UnityEvent targetReached;

    void Start()
    {
        controller = GetComponent<BossController>();
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
