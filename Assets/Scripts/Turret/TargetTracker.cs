using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTracker : MonoBehaviour
{
    public float trackingSpeed = 10f;
    public Vector2 extremumX;
    public Vector2 extremumY;

    Transform target = null;
    Vector3 lastKnownPosition = Vector3.zero;
    Quaternion lookAtRotation;

    private bool slow;

    void Update()
    {
        if (target)
        {
            if (lastKnownPosition != target.position)
            {
                lastKnownPosition = target.position;
                lastKnownPosition.y -= 1f;
                lookAtRotation = Quaternion.LookRotation(lastKnownPosition - transform.position, transform.parent.transform.up);
            }
            float speed = slow ? trackingSpeed / 3f : trackingSpeed;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAtRotation, speed * Time.deltaTime);
        }
    }

    public void FocusOn(Transform target)
    {
        this.target = target;
    }

    public void SetSlowSpeed(bool slowSpeed)
    {
        slow = slowSpeed;
    }
}
